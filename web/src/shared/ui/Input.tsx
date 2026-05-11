import type { InputHTMLAttributes } from 'react';

export function Input({ className = '', ...props }: InputHTMLAttributes<HTMLInputElement>) {
  return <input className={`w-full rounded border px-3 py-2 outline-none focus:border-indigo-500 ${className}`} {...props} />;
}
