import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../features/auth/authStore';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function LoginPage() {
  const [email, setEmail] = useState('demo@taskmanager.local');
  const [password, setPassword] = useState('Demo12345!');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const login = useAuthStore((x) => x.login);

  async function submit(event?: FormEvent) {
    event?.preventDefault();
    setLoading(true);
    setError('');
    try {
      await login(email, password);
      navigate('/workspaces');
    } catch (error) {
      setError(getApiErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }

  async function demoLogin() {
    setEmail('demo@taskmanager.local');
    setPassword('Demo12345!');
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
    <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
      <div className="w-full max-w-md rounded border bg-white p-6 shadow-sm">
        <Link className="text-sm text-slate-500" to="/">← На главную</Link>
        <h1 className="mt-4 text-2xl font-semibold">Войти</h1>
        <form className="mt-6 space-y-4" onSubmit={submit}>
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" type="password" />
          {error && <p className="whitespace-pre-line rounded bg-red-50 p-3 text-sm text-red-700">{error}</p>}
          <button className="w-full rounded bg-indigo-600 px-4 py-2 text-white disabled:opacity-60" disabled={loading}>{loading ? 'Вход...' : 'Войти'}</button>
          <button className="w-full rounded border px-4 py-2 text-slate-700 disabled:opacity-60" type="button" onClick={demoLogin} disabled={loading}>Войти как demo</button>
        </form>
        <Link className="mt-4 block text-sm text-indigo-700" to="/register">Создать аккаунт</Link>
      </div>
    </div>
  );
}
