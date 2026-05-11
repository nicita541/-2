import type { PropsWithChildren } from 'react';

export function Badge({ children }: PropsWithChildren) {
  return <span className="rounded bg-slate-100 px-2 py-1 text-xs text-slate-600">{children}</span>;
}
