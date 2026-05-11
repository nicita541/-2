import { create } from 'zustand';
import type { AuthResponse, AuthUser, AuthWorkspace } from '../../shared/types/api';
import { endpoints } from '../../shared/api/endpoints';

type AuthState = {
  accessToken: string | null;
  user: AuthUser | null;
  workspaces: AuthWorkspace[];
  isAuthenticated: boolean;
  setAuth: (response: AuthResponse) => void;
  login: (email: string, password: string) => Promise<void>;
  register: (displayName: string, email: string, password: string) => Promise<void>;
  loadFromStorage: () => void;
  logout: () => void;
};

const readJson = <T,>(key: string, fallback: T): T => {
  const value = localStorage.getItem(key);
  if (!value) return fallback;
  try {
    return JSON.parse(value) as T;
  } catch {
    return fallback;
  }
};

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: localStorage.getItem('accessToken'),
  user: readJson<AuthUser | null>('user', null),
  workspaces: readJson<AuthWorkspace[]>('workspaces', []),
  isAuthenticated: Boolean(localStorage.getItem('accessToken')),
  setAuth: (response) => {
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    localStorage.setItem('workspaces', JSON.stringify(response.workspaces));
    set({ accessToken: response.accessToken, user: response.user, workspaces: response.workspaces, isAuthenticated: true });
  },
  login: async (email, password) => {
    const response = await endpoints.login(email, password);
    useAuthStore.getState().setAuth(response);
  },
  register: async (displayName, email, password) => {
    const response = await endpoints.register(displayName, email, password);
    useAuthStore.getState().setAuth(response);
  },
  loadFromStorage: () => {
    set({
      accessToken: localStorage.getItem('accessToken'),
      user: readJson<AuthUser | null>('user', null),
      workspaces: readJson<AuthWorkspace[]>('workspaces', []),
      isAuthenticated: Boolean(localStorage.getItem('accessToken'))
    });
  },
  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
    localStorage.removeItem('workspaces');
    set({ accessToken: null, user: null, workspaces: [], isAuthenticated: false });
  }
}));
