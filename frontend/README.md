# SharedTodoLists API Provider

## Usage

```ts
import { ApiClient, TodoListsApi, ApiError } from "./api/index.js";

const client = new ApiClient({
  baseUrl: "https://localhost:5001",
  defaultHeaders: {
    "User-Id": "current-user-id",
  },
});

const api = new TodoListsApi(client);

// Get paged list
const page = await api.getTodoLists({ page: 1, pageSize: 20, onlyOwned: true });

// Get single
const list = await api.getTodoList("64f1a2b3c4d5e6f7a8b9c0d1");

// Create
const created = await api.createTodoList({ name: "Groceries" });

// Update
await api.updateTodoList(created.id, {
  name: "Groceries (updated)",
  items: [{ name: "Milk", isDone: false }],
});

// Delete
await api.deleteTodoList(created.id);

// User management
await api.addTodoListUser(created.id, { userId: "user-42" });
await api.removeTodoListUser(created.id, "user-42");

// Error handling
try {
  await api.getTodoList("missing-id");
} catch (err) {
  if (err instanceof ApiError) {
    console.error(err.status, err.problem.detail);
  }
}
```
