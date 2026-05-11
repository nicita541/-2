import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';
import { useAuthStore } from '../features/auth/authStore';

export function WorkspacesPage() {
  const queryClient = useQueryClient();
  const authWorkspaces = useAuthStore((x) => x.workspaces);
  const workspaces = useQuery({ queryKey: ['workspaces'], queryFn: endpoints.workspaces });
  const create = useMutation({
    mutationFn: () => endpoints.createWorkspace(`Workspace ${Date.now()}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['workspaces'] })
  });

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Рабочие пространства</h1>
        <button className="rounded bg-indigo-600 px-4 py-2 text-white disabled:opacity-60" onClick={() => create.mutate()} disabled={create.isPending}>
          {create.isPending ? 'Создание...' : 'Создать workspace'}
        </button>
      </div>
      {workspaces.isLoading && <p className="mt-6 text-slate-500">Загрузка...</p>}
      {!workspaces.isLoading && !workspaces.data?.items.length && (
        <div className="mt-6 rounded border bg-white p-6 text-slate-500">У вас пока нет рабочих пространств.</div>
      )}
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {workspaces.data?.items.map((workspace) => {
          const authWorkspace = authWorkspaces.find((x) => x.id === workspace.id);
          return (
            <div key={workspace.id} className="rounded border bg-white p-4 shadow-sm">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h2 className="font-medium">{workspace.name}</h2>
                  <p className="mt-1 text-sm text-slate-500">{workspace.description}</p>
                </div>
                <span className="rounded bg-slate-100 px-2 py-1 text-xs text-slate-600">{authWorkspace?.role ?? 'Member'}</span>
              </div>
              <p className="mt-3 text-sm text-slate-500">{authWorkspace?.type ?? 'Team'}</p>
              <Link className="mt-4 inline-flex rounded bg-slate-900 px-3 py-2 text-sm text-white" to={`/workspaces/${workspace.id}/projects`}>Открыть</Link>
            </div>
          );
        })}
      </div>
    </Layout>
  );
}
