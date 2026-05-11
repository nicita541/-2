import { DndContext, useDraggable, useDroppable, type DragEndEvent } from '@dnd-kit/core';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';
import type { KanbanTask } from '../shared/types/api';

export function BoardPage() {
  const { boardId = '' } = useParams();
  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const board = useQuery({ queryKey: ['kanban', boardId], queryFn: () => endpoints.kanban(boardId), enabled: !!boardId });
  const taskDetails = useQuery({ queryKey: ['task-details', selectedTaskId], queryFn: () => endpoints.taskDetails(selectedTaskId!), enabled: !!selectedTaskId });
  const move = useMutation({
    mutationFn: ({ taskId, columnId, order }: { taskId: string; columnId: string; order: number }) => endpoints.moveTask(taskId, columnId, order),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['kanban', boardId] })
  });

  function onDragEnd(event: DragEndEvent) {
    const taskId = String(event.active.id);
    const columnId = event.over?.id ? String(event.over.id) : undefined;
    if (taskId && columnId) move.mutate({ taskId, columnId, order: 0 });
  }

  return (
    <Layout>
      <h1 className="text-2xl font-semibold">{board.data?.name ?? 'Board'}</h1>
      <DndContext onDragEnd={onDragEnd}>
        <div className="mt-6 flex gap-4 overflow-x-auto pb-4">
          {board.data?.columns.map((column) => <KanbanColumn key={column.id} column={column} onOpenTask={setSelectedTaskId} />)}
        </div>
      </DndContext>
      {selectedTaskId && (
        <div className="fixed inset-y-0 right-0 w-full max-w-xl border-l bg-white p-6 shadow-xl">
          <button className="mb-4 rounded border px-3 py-2 text-sm" onClick={() => setSelectedTaskId(null)}>Close</button>
          <h2 className="text-xl font-semibold">{taskDetails.data?.title ?? 'Task'}</h2>
          <p className="mt-2 text-sm text-slate-600">{taskDetails.data?.description}</p>
          <h3 className="mt-6 font-medium">Checklist</h3>
          <div className="mt-2 space-y-2">
            {taskDetails.data?.checklistItems.map((item) => (
              <label key={item.id} className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={item.isCompleted} readOnly />
                {item.text}
              </label>
            ))}
          </div>
          <h3 className="mt-6 font-medium">Comments</h3>
          <div className="mt-2 space-y-2 text-sm text-slate-600">
            {taskDetails.data?.comments.map((comment) => <div key={comment.id} className="rounded bg-slate-50 p-2">{comment.body}</div>)}
          </div>
        </div>
      )}
    </Layout>
  );
}

function KanbanColumn({ column, onOpenTask }: { column: { id: string; name: string; tasks: KanbanTask[] }; onOpenTask: (id: string) => void }) {
  const { setNodeRef, isOver } = useDroppable({ id: column.id });
  return (
    <div ref={setNodeRef} className={`min-h-[500px] w-80 shrink-0 rounded border p-3 ${isOver ? 'bg-indigo-50' : 'bg-slate-100'}`}>
      <h2 className="mb-3 font-medium">{column.name}</h2>
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
