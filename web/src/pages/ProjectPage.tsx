import { useQuery } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';

export function ProjectPage() {
  const { projectId = '' } = useParams();
  const overview = useQuery({ queryKey: ['overview', projectId], queryFn: () => endpoints.overview(projectId), enabled: !!projectId });

  return (
    <Layout>
      <h1 className="text-2xl font-semibold">{overview.data?.name ?? 'Project'}</h1>
      <p className="mt-1 text-slate-500">{overview.data?.description}</p>
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Tasks</div><div className="text-3xl font-semibold">{overview.data?.stats.totalTasks ?? 0}</div></div>
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Completed</div><div className="text-3xl font-semibold">{overview.data?.stats.completedTasks ?? 0}</div></div>
        <div className="rounded border bg-white p-4"><div className="text-sm text-slate-500">Overdue</div><div className="text-3xl font-semibold">{overview.data?.stats.overdueTasks ?? 0}</div></div>
      </div>
      <h2 className="mt-8 text-lg font-semibold">Boards</h2>
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
    </Layout>
  );
}
