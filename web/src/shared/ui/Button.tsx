import type { ButtonHTMLAttributes } from 'react';

export function Button({ className = '', ...props }: ButtonHTMLAttributes<HTMLButtonElement>) {
  return <button className={`rounded bg-indigo-600 px-4 py-2 text-sm font-medium text-white disabled:opacity-60 ${className}`} {...props} />;
}
