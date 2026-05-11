import type { TextareaHTMLAttributes } from 'react';

export function Textarea({ className = '', ...props }: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea className={`w-full rounded border px-3 py-2 outline-none focus:border-indigo-500 ${className}`} {...props} />;
}
