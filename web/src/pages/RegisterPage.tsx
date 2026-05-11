import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../features/auth/authStore';
import { getApiErrorMessage } from '../shared/api/apiClient';

export function RegisterPage() {
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const register = useAuthStore((x) => x.register);

  async function submit(event: FormEvent) {
    event.preventDefault();
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setLoading(true);
    setError('');
    try {
      await register(displayName, email, password);
      navigate('/workspaces');
    } catch (error) {
      setError(getApiErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }

  function validate() {
    if (!displayName.trim()) return 'Введите имя.';
    if (!email.trim()) return 'Введите email.';
    if (password.length < 8) return 'Пароль должен быть не короче 8 символов.';
    if (password !== confirmPassword) return 'Пароли не совпадают.';
    return '';
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100 px-4">
      <div className="w-full max-w-md rounded border bg-white p-6 shadow-sm">
        <Link className="text-sm text-slate-500" to="/">← На главную</Link>
        <h1 className="mt-4 text-2xl font-semibold">Создать аккаунт</h1>
        <form className="mt-6 space-y-4" onSubmit={submit}>
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={displayName} onChange={(e) => setDisplayName(e.target.value)} placeholder="Display name" />
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" type="password" />
          <input className="w-full rounded border px-3 py-2 outline-none focus:border-indigo-500" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} placeholder="Confirm password" type="password" />
          {error && <p className="whitespace-pre-line rounded bg-red-50 p-3 text-sm text-red-700">{error}</p>}
          <button className="w-full rounded bg-indigo-600 px-4 py-2 text-white disabled:opacity-60" disabled={loading}>{loading ? 'Создание...' : 'Создать аккаунт'}</button>
        </form>
        <Link className="mt-4 block text-sm text-indigo-700" to="/login">Уже есть аккаунт</Link>
      </div>
    </div>
  );
}
