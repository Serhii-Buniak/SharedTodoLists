import type { ApiClient } from "./ApiClient.js";
import type {
  AddTodoListUserRequest,
  CreateTodoListRequest,
  CursorResponse,
  GetTodoListsParams,
  GetTodoListsStreamParams,
  PagedResponse,
  TodoListResponse,
  TodoListSummary,
  TodoListUsersResponse,
  UpdateTodoListRequest,
} from "./types.js";
import { buildQuery } from "./utils.js";

export class TodoListsApi {
  private static readonly BASE_PATH = "/api/todo-lists";

  constructor(private readonly client: ApiClient) {}

  getTodoLists(params: GetTodoListsParams = {}) {
    return this.client.get<PagedResponse<TodoListSummary>>(
      `${TodoListsApi.BASE_PATH}${buildQuery(params)}`,
    );
  }

  getTodoListsStream(params: GetTodoListsStreamParams = {}) {
    return this.client.get<CursorResponse<TodoListResponse>>(
      `${TodoListsApi.BASE_PATH}/stream${buildQuery(params)}`,
    );
  }

  getTodoList(id: string) {
    return this.client.get<TodoListResponse>(`${TodoListsApi.BASE_PATH}/${id}`);
  }

  createTodoList(request: CreateTodoListRequest) {
    return this.client.post<TodoListResponse>(TodoListsApi.BASE_PATH, request);
  }

  updateTodoList(id: string, request: UpdateTodoListRequest) {
    return this.client.put<TodoListResponse>(`${TodoListsApi.BASE_PATH}/${id}`, request);
  }

  deleteTodoList(id: string) {
    return this.client.delete(`${TodoListsApi.BASE_PATH}/${id}`);
  }

  // ── User management ───────────────────────────────────────────────────────

  getTodoListUsers(id: string) {
    return this.client.get<TodoListUsersResponse>(`${TodoListsApi.BASE_PATH}/${id}/users`);
  }

  addTodoListUser(id: string, request: AddTodoListUserRequest) {
    return this.client.post<TodoListUsersResponse>(`${TodoListsApi.BASE_PATH}/${id}/users`, request);
  }

  removeTodoListUser(id: string, userId: string) {
    return this.client.delete(`${TodoListsApi.BASE_PATH}/${id}/users/${userId}`);
  }
}
