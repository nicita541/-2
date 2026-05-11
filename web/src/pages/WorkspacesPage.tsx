import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';

export function WorkspacesPage() {
  const queryClient = useQueryClient();
  const workspaces = useQuery({ queryKey: ['workspaces'], queryFn: endpoints.workspaces });
  const create = useMutation({
    mutationFn: () => endpoints.createWorkspace(`Workspace ${Date.now()}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['workspaces'] })
  });

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Workspaces</h1>
        <button className="rounded bg-indigo-600 px-4 py-2 text-white" onClick={() => create.mutate()}>Create</button>
      </div>
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {workspaces.data?.items.map((workspace) => (
          <Link key={workspace.id} className="rounded border bg-white p-4 shadow-sm hover:border-indigo-300" to={`/workspaces/${workspace.id}/projects`}>
            <h2 className="font-medium">{workspace.name}</h2>
            <p className="mt-1 text-sm text-slate-500">{workspace.description}</p>
          </Link>
        ))}
      </div>
    </Layout>
  );
}
