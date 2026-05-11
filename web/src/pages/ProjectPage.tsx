import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { useState } from 'react';
import { boardsApi, projectsApi } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';
import { Modal } from '../shared/ui/Modal';
import { Input } from '../shared/ui/Input';
import { Button } from '../shared/ui/Button';
import { EmptyState } from '../shared/ui/EmptyState';
import { ErrorMessage } from '../shared/ui/ErrorMessage';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function ProjectPage() {
  const { projectId = '' } = useParams();
  const queryClient = useQueryClient();
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [boardName, setBoardName] = useState('Основная доска');
  const [error, setError] = useState('');
  const overview = useQuery({ queryKey: ['overview', projectId], queryFn: () => projectsApi.overview(projectId), enabled: !!projectId });
  const createBoard = useMutation({
    mutationFn: () => boardsApi.create({ projectId, name: boardName }),
    onSuccess: () => {
      setCreateOpen(false);
      queryClient.invalidateQueries({ queryKey: ['overview', projectId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function submitBoard() {
    if (!boardName.trim()) {
      setError('Введите название доски.');
      return;
    }
    setError('');
    createBoard.mutate();
  }

  return (
    <Layout>
      <h1 className="text-2xl font-semibold">{overview.data?.name ?? 'Project'}</h1>
      <p className="mt-1 text-slate-500">{overview.data?.description}</p>
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Tasks</div><div className="text-3xl font-semibold">{overview.data?.stats.totalTasks ?? 0}</div></div>
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Completed</div><div className="text-3xl font-semibold">{overview.data?.stats.completedTasks ?? 0}</div></div>
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Overdue</div><div className="text-3xl font-semibold">{overview.data?.stats.overdueTasks ?? 0}</div></div>
      </div>
      <div className="mt-8 flex items-center justify-between">
        <h2 className="text-lg font-semibold">Boards</h2>
        <Button onClick={() => setCreateOpen(true)}>Создать доску</Button>
      </div>
      {!overview.data?.boards.length && <div className="mt-3"><EmptyState>В проекте пока нет досок. Создайте первую доску.</EmptyState></div>}
      <div className="mt-3 grid gap-3 md:grid-cols-3">
        {overview.data?.boards.map((board) => (
          <Link key={board.id} className="rounded border bg-white p-4 shadow-sm hover:border-indigo-300" to={`/boards/${board.id}`}>
            <h3 className="font-medium">{board.name}</h3>
            <p className="mt-1 text-sm text-slate-500">{board.columnsCount} columns · {board.tasksCount} tasks</p>
          </Link>
        ))}
      </div>
      <h2 className="mt-8 text-lg font-semibold">Notes</h2>
      <div className="mt-3 rounded border bg-white p-4 text-sm text-slate-500">No notes yet.</div>
      {isCreateOpen && (
        <Modal title="Создать доску" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <Input value={boardName} onChange={(e) => setBoardName(e.target.value)} placeholder="Название доски" />
            <ErrorMessage message={error} />
            <Button onClick={submitBoard} disabled={createBoard.isPending}>{createBoard.isPending ? 'Создание...' : 'Создать'}</Button>
          </div>
        </Modal>
      )}
    </Layout>
  );
}
