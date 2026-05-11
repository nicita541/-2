import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';

export function ProjectsPage() {
  const { workspaceId = '' } = useParams();
  const queryClient = useQueryClient();
  const projects = useQuery({ queryKey: ['projects', workspaceId], queryFn: () => endpoints.projects(workspaceId), enabled: !!workspaceId });
  const create = useMutation({
    mutationFn: () => endpoints.createProject(workspaceId, `Project ${Date.now()}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['projects', workspaceId] })
  });

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Проекты</h1>
        <button className="rounded bg-indigo-600 px-4 py-2 text-white disabled:opacity-60" onClick={() => create.mutate()} disabled={create.isPending}>
          {create.isPending ? 'Создание...' : 'Создать проект'}
        </button>
      </div>
      {projects.isLoading && <p className="mt-6 text-slate-500">Загрузка...</p>}
      {!projects.isLoading && !projects.data?.items.length && (
        <div className="mt-6 rounded border bg-white p-6 text-slate-500">В этом workspace пока нет проектов.</div>
      )}
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {projects.data?.items.map((project) => (
          <div key={project.id} className="rounded border bg-white p-4 shadow-sm">
            <h2 className="font-medium">{project.name}</h2>
            <p className="mt-1 text-sm text-slate-500">{project.description}</p>
            <Link className="mt-4 inline-flex rounded bg-slate-900 px-3 py-2 text-sm text-white" to={`/projects/${project.id}`}>Открыть проект</Link>
          </div>
        ))}
      </div>
    </Layout>
  );
}
