import type { SelectHTMLAttributes } from 'react';

export function Select({ className = '', ...props }: SelectHTMLAttributes<HTMLSelectElement>) {
  return <select className={`w-full rounded border px-3 py-2 outline-none focus:border-indigo-500 ${className}`} {...props} />;
}
