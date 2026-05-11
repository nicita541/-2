import { apiClient } from './apiClient';
import type {
  AuthResponse,
  Board,
  BoardCreateRequest,
  Column,
  ColumnCreateRequest,
  KanbanBoard,
  MetadataOptions,
  PagedResult,
  Project,
  ProjectCreateRequest,
  ProjectNote,
  ProjectOverview,
  TaskDetails,
  TaskItem,
  TaskItemCreateRequest,
  Workspace,
  WorkspaceCreateRequest
} from '../types/api';

export const authApi = {
  login: async (email: string, password: string) => (await apiClient.post<AuthResponse>('/api/auth/login', { email, password })).data,
  register: async (displayName: string, email: string, password: string) =>
    (await apiClient.post<AuthResponse>('/api/auth/register', { displayName, email, password })).data,
  me: async () => (await apiClient.get('/api/auth/me')).data
};

export const workspacesApi = {
  getAll: async () => (await apiClient.get<PagedResult<Workspace>>('/api/workspaces')).data,
  create: async (request: WorkspaceCreateRequest) => (await apiClient.post<Workspace>('/api/workspaces', request)).data,
  update: async (id: string, request: WorkspaceCreateRequest) => (await apiClient.put<Workspace>(`/api/workspaces/${id}`, request)).data,
  delete: async (id: string) => apiClient.delete(`/api/workspaces/${id}`)
};

export const metadataApi = {
  taskOptions: async () => (await apiClient.get<MetadataOptions>('/api/metadata/task-options')).data
};

export const projectsApi = {
  getByWorkspace: async (workspaceId: string) => (await apiClient.get<PagedResult<Project>>('/api/projects', { params: { workspaceId } })).data,
  create: async (request: ProjectCreateRequest) => (await apiClient.post<Project>('/api/projects', request)).data,
  update: async (id: string, request: Omit<ProjectCreateRequest, 'workspaceId'>) => (await apiClient.put<Project>(`/api/projects/${id}`, request)).data,
  delete: async (id: string) => apiClient.delete(`/api/projects/${id}`),
  overview: async (projectId: string) => (await apiClient.get<ProjectOverview>(`/api/projects/${projectId}/overview`)).data,
  notes: async (projectId: string) => (await apiClient.get<ProjectNote[]>(`/api/projects/${projectId}/notes`)).data,
  createNote: async (projectId: string, request: { title: string; contentMarkdown: string }) => (await apiClient.post<ProjectNote>(`/api/projects/${projectId}/notes`, request)).data,
  updateNote: async (noteId: string, request: { title: string; contentMarkdown: string }) => (await apiClient.put<ProjectNote>(`/api/project-notes/${noteId}`, request)).data,
  deleteNote: async (noteId: string) => apiClient.delete(`/api/project-notes/${noteId}`)
};

export const boardsApi = {
  getByProject: async (projectId: string) => (await apiClient.get<PagedResult<Board>>('/api/boards', { params: { projectId } })).data,
  create: async (request: BoardCreateRequest) => (await apiClient.post<Board>('/api/boards', request)).data,
  update: async (id: string, request: { name: string }) => (await apiClient.put<Board>(`/api/boards/${id}`, request)).data,
  delete: async (id: string) => apiClient.delete(`/api/boards/${id}`),
  getKanban: async (boardId: string) => (await apiClient.get<KanbanBoard>(`/api/boards/${boardId}/kanban`)).data
};

export const columnsApi = {
  getByBoard: async (boardId: string) => (await apiClient.get<PagedResult<Column>>('/api/columns', { params: { boardId } })).data,
  create: async (request: ColumnCreateRequest) => (await apiClient.post<Column>('/api/columns', { ...request, position: request.position ?? request.order ?? 0 })).data,
  update: async (id: string, request: { name: string; position: number }) => (await apiClient.put<Column>(`/api/columns/${id}`, request)).data,
  delete: async (id: string) => apiClient.delete(`/api/columns/${id}`)
};

export const taskItemsApi = {
  create: async (request: TaskItemCreateRequest) => (await apiClient.post<TaskItem>('/api/taskitems', normalizeTaskRequest(request))).data,
  update: async (id: string, request: TaskItemCreateRequest) => (await apiClient.put<TaskItem>(`/api/taskitems/${id}`, normalizeTaskRequest(request))).data,
  delete: async (id: string) => apiClient.delete(`/api/taskitems/${id}`),
  getDetails: async (id: string) => (await apiClient.get<TaskDetails>(`/api/taskitems/${id}/details`)).data,
  move: async (id: string, request: { targetBoardColumnId: string; targetParentTaskItemId?: string | null; newOrder: number }) =>
    (await apiClient.post<TaskItem>(`/api/taskitems/${id}/move`, request)).data,
  createSubtask: async (id: string, request: TaskItemCreateRequest) =>
    (await apiClient.post<TaskItem>(`/api/taskitems/${id}/subtasks`, normalizeTaskRequest(request))).data,
  addComment: async (id: string, body: string) => (await apiClient.post(`/api/taskitems/${id}/comments`, { body })).data,
  addChecklist: async (id: string, text: string, position: number) => (await apiClient.post(`/api/taskitems/${id}/checklist`, { text, position })).data,
  toggleChecklist: async (id: string) => (await apiClient.post(`/api/checklist/${id}/toggle`)).data,
  createLabel: async (projectId: string, boardId: string, name: string, colorHex: string) => (await apiClient.post(`/api/projects/${projectId}/labels`, { boardId, name, colorHex })).data,
  addLabel: async (taskId: string, labelId: string) => apiClient.post(`/api/taskitems/${taskId}/labels/${labelId}`),
  removeLabel: async (taskId: string, labelId: string) => apiClient.delete(`/api/taskitems/${taskId}/labels/${labelId}`)
};

function normalizeTaskRequest(request: TaskItemCreateRequest) {
  return {
    ...request,
    dueDateUtc: request.dueDateUtc ?? request.deadlineUtc ?? null,
    position: request.position ?? request.order ?? 0
  };
}

export const endpoints = {
  login: authApi.login,
  register: authApi.register,
  me: authApi.me,
  workspaces: workspacesApi.getAll,
  createWorkspace: async (name: string) => workspacesApi.create({ name, description: '' }),
  projects: projectsApi.getByWorkspace,
  createProject: async (workspaceId: string, name: string) => projectsApi.create({ workspaceId, name, description: '' }),
  overview: projectsApi.overview,
  boards: boardsApi.getByProject,
  kanban: boardsApi.getKanban,
  taskDetails: taskItemsApi.getDetails,
  moveTask: async (taskId: string, targetBoardColumnId: string, newOrder: number, targetParentTaskItemId: string | null = null) =>
    taskItemsApi.move(taskId, { targetBoardColumnId, targetParentTaskItemId, newOrder })
};
