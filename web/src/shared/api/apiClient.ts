import axios from 'axios';
import { useAuthStore } from '../../features/auth/authStore';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5088'
});

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { error?: { message?: string; details?: string[] } } | undefined;
    if (data?.error?.details?.length) return data.error.details.join('\n');
    if (data?.error?.message) return data.error.message;
    if (error.response?.status === 401) return 'Неверный email или пароль.';
    if (error.response?.status === 403) return 'Нет доступа к этому действию.';
  }
  return 'Не удалось выполнить запрос. Проверьте подключение к API.';
}

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) useAuthStore.getState().logout();
    return Promise.reject(error);
  }
);
