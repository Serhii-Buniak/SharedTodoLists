// ── Shared generics ─────────────────────────────────────────────────────────

export interface BatchResponse<T> {
  items: T[];
  total: number;
}

export interface PagedResponse<T> extends BatchResponse<T> {
  page: number;
  pageSize: number;
}

export interface CursorResponse<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
}

// ── Domain ───────────────────────────────────────────────────────────────────

export interface TodoItem {
  name: string;
  isDone: boolean;
}

export interface TodoListSummary {
  id: string;
  name: string;
}

export interface TodoListResponse {
  id: string;
  name: string;
  ownerId: string;
  createdAt: string; // ISO-8601
  sharedUserIds: string[];
  items: TodoItem[];
}

export interface TodoListUsersResponse {
  ownerId: string;
  sharedUserIds: string[];
}

// ── Requests ─────────────────────────────────────────────────────────────────

export interface CreateTodoListRequest {
  name: string;
}

export interface TodoItemRequest {
  name: string;
  isDone: boolean;
}

export interface UpdateTodoListRequest {
  name: string;
  items: TodoItemRequest[];
}

export interface AddTodoListUserRequest {
  userId: string;
}

// ── Query params ─────────────────────────────────────────────────────────────

export interface GetTodoListsParams {
  page?: number;
  pageSize?: number;
  onlyOwned?: boolean;
}

export interface GetTodoListsStreamParams {
  cursor?: string;
  limit?: number;
  onlyOwned?: boolean;
}

// ── Error contract (mirrors ASP.NET Core ProblemDetails) ────────────────────

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  [key: string]: unknown;
}
