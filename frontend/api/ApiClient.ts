import { ApiError } from "./ApiError.js";
import type { ProblemDetails } from "./types.js";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

const DEFAULT_HEADERS = {
  "Content-Type": "application/json",
};

export interface ApiClientOptions {
  baseUrl: string;
  defaultHeaders?: Record<string, string>;
}

export interface RequestOptions {
  headers?: Record<string, string>;
}

export class ApiClient {
  private readonly baseUrl: string;
  private readonly defaultHeaders: Record<string, string>;

  constructor({ baseUrl, defaultHeaders = {} }: ApiClientOptions) {
    this.baseUrl = baseUrl.replace(/\/$/, "");
    this.defaultHeaders = { ...DEFAULT_HEADERS, ...defaultHeaders };
  }

  get<T>(path: string, options: RequestOptions = {}) {
    return this.request<T>("GET", path, undefined, options);
  }

  post<T>(path: string, body: unknown, options: RequestOptions = {}) {
    return this.request<T>("POST", path, body, options);
  }

  put<T>(path: string, body: unknown, options: RequestOptions = {}) {
    return this.request<T>("PUT", path, body, options);
  }

  delete(path: string, options: RequestOptions = {}) {
    return this.request<void>("DELETE", path, undefined, options);
  }

  private async request<T>(
    method: HttpMethod,
    path: string,
    body: unknown,
    { headers = {} }: RequestOptions,
  ): Promise<T> {
    const url = `${this.baseUrl}${path}`;

    const response = await fetch(url, {
      method,
      headers: { ...this.defaultHeaders, ...headers },
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });

    if (!response.ok) {
      const problem = await ApiClient.parseProblemDetails(response);
      throw new ApiError(response.status, problem, url);
    }

    if (response.status === 204) {
      return undefined as T;
    }

    return (await response.json()) as T;
  }

  private static async parseProblemDetails(response: Response): Promise<ProblemDetails> {
    try {
      return (await response.json()) as ProblemDetails;
    } catch {
      return { status: response.status, title: response.statusText };
    }
  }
}
