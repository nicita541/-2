import { DndContext, useDraggable, useDroppable, type DragEndEvent } from '@dnd-kit/core';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { boardsApi, columnsApi, metadataApi, taskItemsApi } from '../shared/api/endpoints';
import { getApiErrorMessage } from '../shared/api/apiClient';
import { Layout } from '../shared/ui/Layout';
import { Button } from '../shared/ui/Button';
import { Input } from '../shared/ui/Input';
import { Textarea } from '../shared/ui/Textarea';
import { Select } from '../shared/ui/Select';
import { Modal } from '../shared/ui/Modal';
import { SidePanel } from '../shared/ui/SidePanel';
import { ErrorMessage } from '../shared/ui/ErrorMessage';
import type { KanbanBoard, KanbanColumn as KanbanColumnType, KanbanTask, MetadataOptions, TaskDetails, TaskPriority, TaskStatus } from '../shared/types/api';

export function BoardPage() {
  const { boardId = '' } = useParams();
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);
  const [createColumnOpen, setCreateColumnOpen] = useState(false);
  const [createTaskColumn, setCreateTaskColumn] = useState<KanbanColumnType | null>(null);
  const [error, setError] = useState('');
  const queryClient = useQueryClient();
  const board = useQuery({ queryKey: ['kanban', boardId], queryFn: () => boardsApi.getKanban(boardId), enabled: !!boardId });
  const metadata = useQuery({ queryKey: ['metadata'], queryFn: metadataApi.taskOptions });
  const move = useMutation({
    mutationFn: ({ taskId, columnId, order }: { taskId: string; columnId: string; order: number }) => taskItemsApi.move(taskId, { targetBoardColumnId: columnId, newOrder: order, targetParentTaskItemId: null }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['kanban', boardId] }),
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function onDragEnd(event: DragEndEvent) {
    const taskId = String(event.active.id);
    const columnId = event.over?.id ? String(event.over.id) : undefined;
    const targetColumn = board.data?.columns.find((x) => x.id === columnId);
    if (taskId && targetColumn) move.mutate({ taskId, columnId: targetColumn.id, order: targetColumn.tasks.length });
  }

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">{board.data?.name ?? 'Board'}</h1>
        <Button onClick={() => setCreateColumnOpen(true)}>Добавить колонку</Button>
      </div>
      <ErrorMessage message={error} />
      <DndContext onDragEnd={onDragEnd}>
        <div className="mt-6 flex gap-4 overflow-x-auto pb-4">
          {board.data?.columns.map((column) => (
            <KanbanColumn key={column.id} column={column} onOpenTask={setSelectedTaskId} onCreateTask={setCreateTaskColumn} />
          ))}
        </div>
      </DndContext>
      {createColumnOpen && board.data && <CreateColumnModal board={board.data} onClose={() => setCreateColumnOpen(false)} />}
      {createTaskColumn && board.data && <CreateTaskModal board={board.data} column={createTaskColumn} metadata={metadata.data} onClose={() => setCreateTaskColumn(null)} />}
      {selectedTaskId && board.data && <TaskDetailsPanel board={board.data} taskId={selectedTaskId} metadata={metadata.data} onClose={() => setSelectedTaskId(null)} />}
    </Layout>
  );
}

function CreateColumnModal({ board, onClose }: { board: KanbanBoard; onClose: () => void }) {
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const queryClient = useQueryClient();
  const create = useMutation({
    mutationFn: () => columnsApi.create({ boardId: board.id, name, position: board.columns.length }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      onClose();
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function submit() {
    if (!name.trim()) {
      setError('Введите название колонки.');
      return;
    }
    setError('');
    create.mutate();
  }

  return (
    <Modal title="Добавить колонку" onClose={onClose}>
      <div className="space-y-3">
        <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Название" />
        <ErrorMessage message={error} />
        <Button onClick={submit} disabled={create.isPending}>{create.isPending ? 'Создание...' : 'Создать'}</Button>
      </div>
    </Modal>
  );
}

function CreateTaskModal({ board, column, metadata, onClose }: { board: KanbanBoard; column: KanbanColumnType; metadata?: MetadataOptions; onClose: () => void }) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<TaskPriority>('Medium');
  const [deadline, setDeadline] = useState('');
  const [error, setError] = useState('');
  const queryClient = useQueryClient();
  const create = useMutation({
    mutationFn: () => taskItemsApi.create({
      projectId: board.projectId,
      boardColumnId: column.id,
      parentTaskItemId: null,
      title,
      description,
      priority,
      status: 'Todo',
      deadlineUtc: deadline ? new Date(deadline).toISOString() : null,
      position: column.tasks.length,
      order: column.tasks.length,
      assigneeId: null
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      onClose();
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function submit() {
    if (!title.trim()) {
      setError('Введите название задачи.');
      return;
    }
    setError('');
    create.mutate();
  }

  return (
    <Modal title="Добавить задачу" onClose={onClose}>
      <div className="space-y-3">
        <Input value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Название" />
        <Textarea value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Описание" rows={3} />
        <Select value={priority} onChange={(e) => setPriority(e.target.value as TaskPriority)}>
          {(metadata?.taskPriorities ?? [{ value: 'Low', label: 'Low' }, { value: 'Medium', label: 'Medium' }, { value: 'High', label: 'High' }, { value: 'Critical', label: 'Critical' }]).map((option) => (
            <option key={option.value} value={option.value}>{option.label}</option>
          ))}
        </Select>
        <Input type="date" value={deadline} onChange={(e) => setDeadline(e.target.value)} />
        <ErrorMessage message={error} />
        <Button onClick={submit} disabled={create.isPending}>{create.isPending ? 'Создание...' : 'Создать'}</Button>
      </div>
    </Modal>
  );
}

function TaskDetailsPanel({ board, taskId, metadata, onClose }: { board: KanbanBoard; taskId: string; metadata?: MetadataOptions; onClose: () => void }) {
  const queryClient = useQueryClient();
  const taskDetails = useQuery({ queryKey: ['task-details', taskId], queryFn: () => taskItemsApi.getDetails(taskId) });
  const task = taskDetails.data;
  const [form, setForm] = useState<{ title: string; description: string; priority: TaskPriority; status: TaskStatus; deadline: string }>({ title: '', description: '', priority: 'Medium', status: 'Todo', deadline: '' });
  const [subtaskTitle, setSubtaskTitle] = useState('');
  const [commentBody, setCommentBody] = useState('');
  const [checklistText, setChecklistText] = useState('');
  const [labelName, setLabelName] = useState('');
  const [labelColor, setLabelColor] = useState('#22c55e');
  const [error, setError] = useState('');

  useEffect(() => {
    if (!task) return;
    setForm({
      title: task.title,
      description: task.description ?? '',
      priority: task.priority ?? 'Medium',
      status: task.status ?? 'Todo',
      deadline: task.deadlineUtc ? task.deadlineUtc.slice(0, 10) : ''
    });
  }, [task]);

  const update = useMutation({
    mutationFn: () => taskItemsApi.update(taskId, {
      projectId: task!.projectId,
      boardColumnId: task!.boardColumnId,
      parentTaskItemId: task!.parentTaskItemId ?? null,
      title: form.title,
      description: form.description,
      priority: form.priority,
      status: form.status,
      deadlineUtc: form.deadline ? new Date(form.deadline).toISOString() : null,
      position: task!.order,
      order: task!.order,
      assigneeId: null
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  const remove = useMutation({
    mutationFn: () => taskItemsApi.delete(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      onClose();
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  const createSubtask = useMutation({
    mutationFn: () => taskItemsApi.createSubtask(taskId, {
      projectId: task!.projectId,
      boardColumnId: task!.boardColumnId,
      parentTaskItemId: taskId,
      title: subtaskTitle,
      description: '',
      status: 'Todo',
      priority: 'Medium',
      deadlineUtc: null,
      position: task!.subtasks.length,
      order: task!.subtasks.length,
      assigneeId: null
    }),
    onSuccess: () => {
      setSubtaskTitle('');
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });
  const addComment = useMutation({
    mutationFn: () => taskItemsApi.addComment(taskId, commentBody),
    onSuccess: () => {
      setCommentBody('');
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });
  const addChecklist = useMutation({
    mutationFn: () => taskItemsApi.addChecklist(taskId, checklistText, task!.checklistItems.length),
    onSuccess: () => {
      setChecklistText('');
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });
  const toggleChecklist = useMutation({
    mutationFn: (id: string) => taskItemsApi.toggleChecklist(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    }
  });
  const createAndAddLabel = useMutation({
    mutationFn: async () => {
      const label = await taskItemsApi.createLabel(task!.projectId, board.columns[0].id, labelName, labelColor);
      await taskItemsApi.addLabel(taskId, label.id);
    },
    onSuccess: () => {
      setLabelName('');
      queryClient.invalidateQueries({ queryKey: ['kanban', board.id] });
      queryClient.invalidateQueries({ queryKey: ['task-details', taskId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function save() {
    if (!form.title.trim()) {
      setError('Введите название задачи.');
      return;
    }
    setError('');
    update.mutate();
  }

  function addSubtask() {
    if (!subtaskTitle.trim()) {
      setError('Введите название подзадачи.');
      return;
    }
    setError('');
    createSubtask.mutate();
  }

  return (
    <SidePanel title={task?.title ?? 'Task'} onClose={onClose}>
      {!task && <div className="text-sm text-slate-500">Загрузка...</div>}
      {task && (
        <div className="space-y-4">
          <Input value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} />
          <Textarea value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} rows={4} />
          <div className="grid grid-cols-2 gap-3">
            <Select value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value as TaskStatus })}>
              {(metadata?.taskStatuses ?? [{ value: 'Todo', label: 'Todo' }, { value: 'InProgress', label: 'InProgress' }, { value: 'Review', label: 'Review' }, { value: 'Done', label: 'Done' }, { value: 'Archived', label: 'Archived' }]).map((option) => (
                <option key={option.value} value={option.value}>{option.label}</option>
              ))}
            </Select>
            <Select value={form.priority} onChange={(e) => setForm({ ...form, priority: e.target.value as TaskPriority })}>
              {(metadata?.taskPriorities ?? [{ value: 'Low', label: 'Low' }, { value: 'Medium', label: 'Medium' }, { value: 'High', label: 'High' }, { value: 'Critical', label: 'Critical' }]).map((option) => (
                <option key={option.value} value={option.value}>{option.label}</option>
              ))}
            </Select>
          </div>
          <Input type="date" value={form.deadline} onChange={(e) => setForm({ ...form, deadline: e.target.value })} />
          <ErrorMessage message={error} />
          <div className="flex gap-2">
            <Button onClick={save} disabled={update.isPending}>{update.isPending ? 'Сохранение...' : 'Сохранить'}</Button>
            <Button className="bg-red-600" onClick={() => remove.mutate()} disabled={remove.isPending}>{remove.isPending ? 'Удаление...' : 'Удалить'}</Button>
          </div>
          <div>
            <h3 className="font-medium">Labels</h3>
            <div className="mt-2 flex flex-wrap gap-2">
              {task.labels.map((label) => <span key={label.id} className="rounded px-2 py-1 text-xs text-white" style={{ background: label.color }}>{label.name}</span>)}
            </div>
            <div className="mt-3 flex gap-2">
              <Input value={labelName} onChange={(e) => setLabelName(e.target.value)} placeholder="Label" />
              <Input className="w-28" value={labelColor} onChange={(e) => setLabelColor(e.target.value)} />
              <Button onClick={() => createAndAddLabel.mutate()} disabled={!labelName.trim() || createAndAddLabel.isPending}>Add</Button>
            </div>
          </div>
          <div>
            <h3 className="font-medium">Checklist</h3>
            <div className="mt-2 space-y-2">
              {task.checklistItems.map((item) => (
                <label key={item.id} className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={item.isCompleted} onChange={() => toggleChecklist.mutate(item.id)} />
                  {item.text}
                </label>
              ))}
            </div>
            <div className="mt-3 flex gap-2">
              <Input value={checklistText} onChange={(e) => setChecklistText(e.target.value)} placeholder="Checklist item" />
              <Button onClick={() => addChecklist.mutate()} disabled={!checklistText.trim() || addChecklist.isPending}>Add</Button>
            </div>
          </div>
          <div>
            <h3 className="font-medium">Comments</h3>
            <div className="mt-2 space-y-2">
              {task.comments.map((comment) => <div key={comment.id} className="rounded bg-slate-50 p-2 text-sm">{comment.body}</div>)}
            </div>
            <div className="mt-3 flex gap-2">
              <Input value={commentBody} onChange={(e) => setCommentBody(e.target.value)} placeholder="Comment" />
              <Button onClick={() => addComment.mutate()} disabled={!commentBody.trim() || addComment.isPending}>Add</Button>
            </div>
          </div>
          <div>
            <h3 className="font-medium">Подзадачи</h3>
            <div className="mt-2 space-y-2">
              {task.subtasks.map((subtask) => <div key={subtask.id} className="rounded bg-slate-50 p-2 text-sm">{subtask.title}</div>)}
            </div>
            <div className="mt-3 flex gap-2">
              <Input value={subtaskTitle} onChange={(e) => setSubtaskTitle(e.target.value)} placeholder="Название подзадачи" />
              <Button onClick={addSubtask} disabled={createSubtask.isPending}>Добавить</Button>
            </div>
          </div>
        </div>
      )}
    </SidePanel>
  );
}

function KanbanColumn({ column, onOpenTask, onCreateTask }: { column: KanbanColumnType; onOpenTask: (id: string) => void; onCreateTask: (column: KanbanColumnType) => void }) {
  const { setNodeRef, isOver } = useDroppable({ id: column.id });
  return (
    <div ref={setNodeRef} className={`min-h-[500px] w-80 shrink-0 rounded border p-3 ${isOver ? 'bg-indigo-50' : 'bg-slate-100'}`}>
      <div className="mb-3 flex items-center justify-between gap-2">
        <h2 className="font-medium">{column.name}</h2>
        <button className="rounded border bg-white px-2 py-1 text-xs" onClick={() => onCreateTask(column)}>+ Добавить задачу</button>
      </div>
      <div className="space-y-3">
        {column.tasks.map((task) => <TaskCard key={task.id} task={task} onOpenTask={onOpenTask} />)}
      </div>
    </div>
  );
}

function TaskCard({ task, onOpenTask }: { task: KanbanTask; onOpenTask: (id: string) => void }) {
  const { attributes, listeners, setNodeRef, transform } = useDraggable({ id: task.id });
  return (
    <div
      ref={setNodeRef}
      {...listeners}
      {...attributes}
      className="cursor-grab rounded border bg-white p-3 shadow-sm"
      style={{ transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined }}
      onDoubleClick={() => onOpenTask(task.id)}
    >
      <div className="font-medium">{task.title}</div>
      <p className="mt-1 line-clamp-2 text-sm text-slate-500">{task.description}</p>
      <div className="mt-3 flex flex-wrap gap-1">
        {task.labels.map((label) => (
          <span key={label.id} className="rounded px-2 py-1 text-xs text-white" style={{ background: label.color }}>{label.name}</span>
        ))}
      </div>
      <div className="mt-3 text-xs text-slate-500">
        {task.checklist.completed}/{task.checklist.total} checklist · {task.commentsCount} comments
      </div>
    </div>
  );
}
