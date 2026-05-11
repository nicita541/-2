import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { endpoints } from '../shared/api/endpoints';
import { useAuthStore } from '../features/auth/authStore';

export function RegisterPage() {
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();
  const setAuth = useAuthStore((x) => x.setAuth);

  async function submit(event: FormEvent) {
    event.preventDefault();
    const result = await endpoints.register(displayName, email, password);
    setAuth(result.accessToken, result.user);
    navigate('/workspaces');
  }

  return (
    <div className="mx-auto mt-20 max-w-md rounded border bg-white p-6 shadow-sm">
      <h1 className="text-2xl font-semibold">Register</h1>
      <form className="mt-6 space-y-4" onSubmit={submit}>
        <input className="w-full rounded border px-3 py-2" value={displayName} onChange={(e) => setDisplayName(e.target.value)} placeholder="Display name" />
        <input className="w-full rounded border px-3 py-2" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input className="w-full rounded border px-3 py-2" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" type="password" />
        <button className="w-full rounded bg-indigo-600 px-4 py-2 text-white">Register</button>
      </form>
      <Link className="mt-4 block text-sm text-indigo-700" to="/login">Back to login</Link>
    </div>
  );
}
