import { createBrowserRouter, Navigate } from 'react-router-dom';
import { LoginPage } from '../pages/LoginPage';
import { RegisterPage } from '../pages/RegisterPage';
import { WorkspacesPage } from '../pages/WorkspacesPage';
import { ProjectsPage } from '../pages/ProjectsPage';
import { ProjectPage } from '../pages/ProjectPage';
import { BoardPage } from '../pages/BoardPage';

export const router = createBrowserRouter([
  { path: '/', element: <Navigate to="/workspaces" replace /> },
  { path: '/login', element: <LoginPage /> },
  { path: '/register', element: <RegisterPage /> },
  { path: '/workspaces', element: <WorkspacesPage /> },
  { path: '/workspaces/:workspaceId/projects', element: <ProjectsPage /> },
  { path: '/projects/:projectId', element: <ProjectPage /> },
  { path: '/boards/:boardId', element: <BoardPage /> }
]);
