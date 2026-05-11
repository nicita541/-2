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
        <h1 className="text-2xl font-semibold">Projects</h1>
        <button className="rounded bg-indigo-600 px-4 py-2 text-white" onClick={() => create.mutate()}>Create</button>
      </div>
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {projects.data?.items.map((project) => (
          <Link key={project.id} className="rounded border bg-white p-4 shadow-sm hover:border-indigo-300" to={`/projects/${project.id}`}>
            <h2 className="font-medium">{project.name}</h2>
            <p className="mt-1 text-sm text-slate-500">{project.description}</p>
          </Link>
        ))}
      </div>
    </Layout>
  );
}
