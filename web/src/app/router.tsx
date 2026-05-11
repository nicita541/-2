import { createBrowserRouter, Navigate } from 'react-router-dom';
import type { PropsWithChildren } from 'react';
import { LoginPage } from '../pages/LoginPage';
import { RegisterPage } from '../pages/RegisterPage';
import { WorkspacesPage } from '../pages/WorkspacesPage';
import { ProjectsPage } from '../pages/ProjectsPage';
import { ProjectPage } from '../pages/ProjectPage';
import { BoardPage } from '../pages/BoardPage';
import { LandingPage } from '../pages/LandingPage';
import { useAuthStore } from '../features/auth/authStore';

function ProtectedRoute({ children }: PropsWithChildren) {
  const isAuthenticated = useAuthStore((x) => x.isAuthenticated);
  return isAuthenticated ? children : <Navigate to="/login" replace />;
}

function PublicOnlyRoute({ children }: PropsWithChildren) {
  const isAuthenticated = useAuthStore((x) => x.isAuthenticated);
  return isAuthenticated ? <Navigate to="/workspaces" replace /> : children;
}

export const router = createBrowserRouter([
  { path: '/', element: <LandingPage /> },
  { path: '/login', element: <PublicOnlyRoute><LoginPage /></PublicOnlyRoute> },
  { path: '/register', element: <PublicOnlyRoute><RegisterPage /></PublicOnlyRoute> },
  { path: '/workspaces', element: <ProtectedRoute><WorkspacesPage /></ProtectedRoute> },
  { path: '/workspaces/:workspaceId/projects', element: <ProtectedRoute><ProjectsPage /></ProtectedRoute> },
  { path: '/projects/:projectId', element: <ProtectedRoute><ProjectPage /></ProtectedRoute> },
  { path: '/boards/:boardId', element: <ProtectedRoute><BoardPage /></ProtectedRoute> }
]);
