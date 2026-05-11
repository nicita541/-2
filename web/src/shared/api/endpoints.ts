import { apiClient } from './apiClient';
import type { AuthResponse, Board, KanbanBoard, PagedResult, Project, ProjectOverview, TaskDetails, Workspace } from '../types/api';

export const endpoints = {
  login: async (email: string, password: string) => (await apiClient.post<AuthResponse>('/api/auth/login', { email, password })).data,
  register: async (displayName: string, email: string, password: string) =>
    (await apiClient.post<AuthResponse>('/api/auth/register', { displayName, email, password })).data,
  me: async () => (await apiClient.get('/api/auth/me')).data,
  workspaces: async () => (await apiClient.get<PagedResult<Workspace>>('/api/workspaces')).data,
  createWorkspace: async (name: string) => (await apiClient.post<Workspace>('/api/workspaces', { name, description: '' })).data,
  projects: async (workspaceId: string) => (await apiClient.get<PagedResult<Project>>('/api/projects', { params: { workspaceId } })).data,
  createProject: async (workspaceId: string, name: string) => (await apiClient.post<Project>('/api/projects', { workspaceId, name, description: '' })).data,
  overview: async (projectId: string) => (await apiClient.get<ProjectOverview>(`/api/projects/${projectId}/overview`)).data,
  boards: async (projectId: string) => (await apiClient.get<PagedResult<Board>>('/api/boards', { params: { projectId } })).data,
  kanban: async (boardId: string) => (await apiClient.get<KanbanBoard>(`/api/boards/${boardId}/kanban`)).data,
  taskDetails: async (taskId: string) => (await apiClient.get<TaskDetails>(`/api/taskitems/${taskId}/details`)).data,
  moveTask: async (taskId: string, targetBoardColumnId: string, newOrder: number, targetParentTaskItemId: string | null = null) =>
    (await apiClient.post(`/api/taskitems/${taskId}/move`, { targetBoardColumnId, targetParentTaskItemId, newOrder })).data
};
