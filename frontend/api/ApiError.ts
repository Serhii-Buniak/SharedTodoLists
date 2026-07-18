import type { ProblemDetails } from "./types.js";

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem: ProblemDetails,
    public readonly url: string,
  ) {
    super(problem.detail ?? problem.title ?? `HTTP ${status}`);
    this.name = "ApiError";
  }

  get isNotFound() {
    return this.status === 404;
  }

  get isForbidden() {
    return this.status === 403;
  }

  get isBadRequest() {
    return this.status === 400;
  }
}
