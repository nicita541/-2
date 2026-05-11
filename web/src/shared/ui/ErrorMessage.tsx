export function ErrorMessage({ message }: { message?: string }) {
  if (!message) return null;
  return <p className="whitespace-pre-line rounded bg-red-50 p-3 text-sm text-red-700">{message}</p>;
}
