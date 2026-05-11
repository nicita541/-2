import type { PropsWithChildren } from 'react';

export function SidePanel({ title, onClose, children }: PropsWithChildren<{ title: string; onClose: () => void }>) {
  return (
    <div className="fixed inset-y-0 right-0 z-30 w-full max-w-xl overflow-y-auto border-l bg-white p-6 shadow-xl">
      <button className="mb-4 rounded border px-3 py-2 text-sm" onClick={onClose}>Close</button>
      <h2 className="text-xl font-semibold">{title}</h2>
      <div className="mt-5">{children}</div>
    </div>
  );
}
