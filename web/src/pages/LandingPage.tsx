import { Link, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { useAuthStore } from '../features/auth/authStore';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function LandingPage() {
  const navigate = useNavigate();
  const login = useAuthStore((x) => x.login);
  const isAuthenticated = useAuthStore((x) => x.isAuthenticated);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function demoLogin() {
    setLoading(true);
    setError('');
    try {
      await login('demo@taskmanager.local', 'Demo12345!');
      navigate('/workspaces');
    } catch (error) {
      const message = getApiErrorMessage(error);
      setError(message.includes('Неверный') ? 'Демо-пользователь не найден. Сначала вызови POST /api/dev/seed в Swagger.' : message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="min-h-screen bg-slate-950 text-white">
      <div className="mx-auto flex min-h-screen max-w-6xl flex-col justify-center px-6 py-12">
        <div className="max-w-3xl">
          <div className="mb-4 inline-flex rounded-full border border-white/15 px-3 py-1 text-sm text-indigo-200">Task Manager</div>
          <h1 className="text-5xl font-semibold tracking-tight md:text-7xl">Task Manager</h1>
          <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
            Проекты, доски, задачи, подзадачи, заметки и командная работа в одном месте.
          </p>
          <div className="mt-10 flex flex-wrap gap-3">
            {isAuthenticated ? (
              <Link className="rounded bg-white px-5 py-3 font-medium text-slate-950" to="/workspaces">Открыть приложение</Link>
            ) : (
              <>
                <Link className="rounded bg-white px-5 py-3 font-medium text-slate-950" to="/login">Войти</Link>
                <Link className="rounded border border-white/20 px-5 py-3 font-medium text-white hover:bg-white/10" to="/register">Создать аккаунт</Link>
                <button className="rounded border border-indigo-300/50 px-5 py-3 font-medium text-indigo-100 hover:bg-indigo-500/20 disabled:opacity-60" onClick={demoLogin} disabled={loading}>
                  {loading ? 'Вход...' : 'Демо-вход'}
                </button>
              </>
            )}
          </div>
          {error && <div className="mt-5 rounded border border-red-400/30 bg-red-500/10 p-3 text-sm text-red-100">{error}</div>}
        </div>
      </div>
    </main>
  );
}
