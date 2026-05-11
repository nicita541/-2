import type { PropsWithChildren } from 'react';

export function Modal({ title, onClose, children }: PropsWithChildren<{ title: string; onClose: () => void }>) {
  return (
    <div className="fixed inset-0 z-40 flex items-center justify-center bg-slate-950/40 p-4">
      <div className="w-full max-w-lg rounded border bg-white p-5 shadow-xl">
        <div className="mb-4 flex items-center justify-between">
          <h2 className="text-lg font-semibold">{title}</h2>
          <button className="rounded px-2 py-1 text-slate-500 hover:bg-slate-100" onClick={onClose}>Close</button>
        </div>
        {children}
      </div>
    </div>
  );
}
