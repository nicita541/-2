import { create } from 'zustand';
import type { AuthUser } from '../../shared/types/api';

type AuthState = {
  accessToken: string | null;
  user: AuthUser | null;
  setAuth: (accessToken: string, user: AuthUser) => void;
  logout: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: localStorage.getItem('accessToken'),
  user: null,
  setAuth: (accessToken, user) => {
    localStorage.setItem('accessToken', accessToken);
    set({ accessToken, user });
  },
  logout: () => {
    localStorage.removeItem('accessToken');
    set({ accessToken: null, user: null });
  }
}));
