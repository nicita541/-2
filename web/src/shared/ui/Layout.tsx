import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../features/auth/authStore';
import type { PropsWithChildren } from 'react';

export function Layout({ children }: PropsWithChildren) {
  const { user, workspaces, logout } = useAuthStore();
  const navigate = useNavigate();
  const firstWorkspace = workspaces[0];
  return (
    <div className="min-h-screen">
      <aside className="fixed inset-y-0 left-0 hidden w-64 border-r bg-white p-5 md:block">
        <Link to="/workspaces" className="text-xl font-semibold">Task Manager</Link>
        <nav className="mt-8 space-y-2 text-sm">
          <Link className="block rounded px-3 py-2 hover:bg-slate-100" to="/workspaces">Workspaces</Link>
          <Link className="block rounded px-3 py-2 hover:bg-slate-100" to={firstWorkspace ? `/workspaces/${firstWorkspace.id}/projects` : '/workspaces'}>Projects</Link>
          <Link className="block rounded px-3 py-2 text-slate-400" to="/workspaces">Board</Link>
        </nav>
      </aside>
      <main className="md:pl-64">
        <header className="flex h-16 items-center justify-between border-b bg-white px-6">
          <span className="font-medium">Workspace console</span>
          <div className="flex items-center gap-3">
            <span className="hidden text-sm text-slate-600 sm:inline">{user?.displayName || user?.email}</span>
            <button className="rounded bg-slate-900 px-3 py-2 text-sm text-white" onClick={() => { logout(); navigate('/login'); }}>Logout</button>
          </div>
        </header>
        <section className="p-6">{children}</section>
      </main>
    </div>
  );
}
