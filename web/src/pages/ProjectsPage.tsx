import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { useState } from 'react';
import { projectsApi } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';
import { Modal } from '../shared/ui/Modal';
import { Input } from '../shared/ui/Input';
import { Textarea } from '../shared/ui/Textarea';
import { Button } from '../shared/ui/Button';
import { ErrorMessage } from '../shared/ui/ErrorMessage';
import { EmptyState } from '../shared/ui/EmptyState';
import { Spinner } from '../shared/ui/Spinner';
import { Card } from '../shared/ui/Card';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function ProjectsPage() {
  const { workspaceId = '' } = useParams();
  const queryClient = useQueryClient();
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [color, setColor] = useState('#6366f1');
  const [icon, setIcon] = useState('folder');
  const [error, setError] = useState('');
  const projects = useQuery({ queryKey: ['projects', workspaceId], queryFn: () => projectsApi.getByWorkspace(workspaceId), enabled: !!workspaceId });
  const create = useMutation({
    mutationFn: () => projectsApi.create({ workspaceId, name, description, color, icon, status: 'Active', isArchived: false }),
    onSuccess: () => {
      setCreateOpen(false);
      setName('');
      setDescription('');
      queryClient.invalidateQueries({ queryKey: ['projects', workspaceId] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function submit() {
    if (!name.trim()) {
      setError('Введите название проекта.');
      return;
    }
    setError('');
    create.mutate();
  }

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Проекты</h1>
        <Button onClick={() => setCreateOpen(true)}>Создать проект</Button>
      </div>
      {projects.isLoading && <div className="mt-6"><Spinner /></div>}
      {!projects.isLoading && !projects.data?.items.length && (
        <div className="mt-6"><EmptyState>В этом workspace пока нет проектов.</EmptyState></div>
      )}
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {projects.data?.items.map((project) => (
          <Card key={project.id}>
            <div className="flex items-center gap-2">
              <span className="h-3 w-3 rounded-full" style={{ background: project.color ?? '#6366f1' }} />
              <h2 className="font-medium">{project.name}</h2>
            </div>
            <p className="mt-1 text-sm text-slate-500">{project.description}</p>
            <Link className="mt-4 inline-flex rounded bg-slate-900 px-3 py-2 text-sm text-white" to={`/projects/${project.id}`}>Открыть проект</Link>
          </Card>
        ))}
      </div>
      {isCreateOpen && (
        <Modal title="Создать проект" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Название" />
            <Textarea value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Описание" rows={3} />
            <Input value={color} onChange={(e) => setColor(e.target.value)} placeholder="#6366f1" />
            <Input value={icon} onChange={(e) => setIcon(e.target.value)} placeholder="folder" />
            <ErrorMessage message={error} />
            <Button onClick={submit} disabled={create.isPending}>{create.isPending ? 'Создание...' : 'Создать'}</Button>
          </div>
        </Modal>
      )}
    </Layout>
  );
}
