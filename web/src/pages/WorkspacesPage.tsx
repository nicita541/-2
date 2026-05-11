import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { metadataApi, workspacesApi } from '../shared/api/endpoints';
import { Layout } from '../shared/ui/Layout';
import { useAuthStore } from '../features/auth/authStore';
import { Modal } from '../shared/ui/Modal';
import { Input } from '../shared/ui/Input';
import { Select } from '../shared/ui/Select';
import { Button } from '../shared/ui/Button';
import { ErrorMessage } from '../shared/ui/ErrorMessage';
import { EmptyState } from '../shared/ui/EmptyState';
import { Spinner } from '../shared/ui/Spinner';
import { Card } from '../shared/ui/Card';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function WorkspacesPage() {
  const queryClient = useQueryClient();
  const authWorkspaces = useAuthStore((x) => x.workspaces);
  const [isCreateOpen, setCreateOpen] = useState(false);
  const [name, setName] = useState('');
  const [type, setType] = useState<'Personal' | 'Team'>('Personal');
  const [error, setError] = useState('');
  const workspaces = useQuery({ queryKey: ['workspaces'], queryFn: workspacesApi.getAll });
  const metadata = useQuery({ queryKey: ['metadata'], queryFn: metadataApi.taskOptions });
  const create = useMutation({
    mutationFn: () => workspacesApi.create({ name, type }),
    onSuccess: () => {
      setCreateOpen(false);
      setName('');
      queryClient.invalidateQueries({ queryKey: ['workspaces'] });
    },
    onError: (error) => setError(getApiErrorMessage(error))
  });

  function submit() {
    if (!name.trim()) {
      setError('Введите название workspace.');
      return;
    }
    setError('');
    create.mutate();
  }

  return (
    <Layout>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Рабочие пространства</h1>
        <Button onClick={() => setCreateOpen(true)}>Создать workspace</Button>
      </div>
      {workspaces.isLoading && <div className="mt-6"><Spinner /></div>}
      {!workspaces.isLoading && !workspaces.data?.items.length && (
        <div className="mt-6"><EmptyState>У вас пока нет рабочих пространств.</EmptyState></div>
      )}
      <div className="mt-6 grid gap-3 md:grid-cols-3">
        {workspaces.data?.items.map((workspace) => {
          const authWorkspace = authWorkspaces.find((x) => x.id === workspace.id);
          return (
            <Card key={workspace.id}>
              <div className="flex items-start justify-between gap-3">
                <div>
                  <h2 className="font-medium">{workspace.name}</h2>
                  <p className="mt-1 text-sm text-slate-500">{workspace.description}</p>
                </div>
                <span className="rounded bg-slate-100 px-2 py-1 text-xs text-slate-600">{authWorkspace?.role ?? 'Member'}</span>
              </div>
              <p className="mt-3 text-sm text-slate-500">{authWorkspace?.type ?? 'Team'}</p>
              <Link className="mt-4 inline-flex rounded bg-slate-900 px-3 py-2 text-sm text-white" to={`/workspaces/${workspace.id}/projects`}>Открыть</Link>
            </Card>
          );
        })}
      </div>
      {isCreateOpen && (
        <Modal title="Создать workspace" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Название" />
            <Select value={type} onChange={(e) => setType(e.target.value as 'Personal' | 'Team')}>
              {(metadata.data?.workspaceTypes ?? [{ value: 'Personal', label: 'Личное' }, { value: 'Team', label: 'Команда' }]).map((option) => (
                <option key={option.value} value={option.value}>{option.label}</option>
              ))}
            </Select>
            <ErrorMessage message={error} />
            <Button onClick={submit} disabled={create.isPending}>{create.isPending ? 'Создание...' : 'Создать'}</Button>
          </div>
        </Modal>
      )}
    </Layout>
  );
}
