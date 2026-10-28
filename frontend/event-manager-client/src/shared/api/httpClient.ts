const API_BASE_URL = (
    import.meta.env.VITE_API_BASE_URL || "/api"
).replace(/\/+$/, "");

export interface ApiProblemDetails {
    title?: string,
    detail?: string,
    status?: number,
    errors?: Record<string, string[]>;
}

export interface ApiRequestOptions
    extends Omit<RequestInit, "body" | "headers"> {
    json?: unknown;
    accessToken?: string | null;
    headers?: HeadersInit;
}

export class ApiError extends Error {
    public readonly status: number;
    public readonly title?: string;
    public readonly errors?: Record<string, string[]>;
    constructor(
        status: number,
        message: string,
        title?: string,
        errors?: Record<string, string[]>,
    ) {
        super(message);
        this.name = "ApiError";
        this.status = status;
        this.title = title;
        this.errors = errors;
    }
}

type UnauthorizedHandler =
    () => Promise<string | null>;

let unauthorizedHandler : UnauthorizedHandler | null = null;

export function registerUnauthorizedHandler(
    handler: UnauthorizedHandler,
) : () => void {
    unauthorizedHandler = handler;

    return () => {
        if (unauthorizedHandler === handler) {
            unauthorizedHandler = null;
        }
    };
}

function isRecord(
    value: unknown,
) : value is Record<string, unknown>{
    return (
        typeof value === "object" &&
        value !== null &&
        !Array.isArray(value)
    );
}

function parseProblemDetails(
    value: unknown,
) : ApiProblemDetails {
    if (!isRecord(value))
    {
        return {};
    }

    const errorsValue = value.errors;
    let errors: Record<string, string[]> | undefined;

    if (isRecord(errorsValue)) {
        errors = {};

        for (const [key, messages] of Object.entries(errorsValue)) {
            if (
                Array.isArray(messages) &&
                messages.every(
                    (message) => typeof message === "string",)
            )
            {
                errors[key] = messages;
            }
        }
    }

    return {
        title:
            typeof value.title === "string"
                ? value.title
                : undefined,

        detail:
            typeof value.detail === "string"
                ? value.detail
                : undefined,

        status:
            typeof value.status === "number"
                ? value.status
                : undefined,

        errors,
    };
}

async function throwApiError(
    response: Response,
) : Promise<never> {
    const contentType = 
        response.headers.get("content-type") ?? "";

    let body : unknown;

    if (contentType.includes("application/json")) {
        body = await response.json().catch(() => undefined);
    } else {
        body = await response.text().catch(() => "");
    }

    const problem = parseProblemDetails(body);

    const message = 
        problem.detail ||
        problem.title ||
        (typeof body === "string" && body.length > 0
            ? body
            : `Request failed with status ${response.status}.`);

    throw new ApiError(
        response.status,
        message,
        problem.title,
        problem.errors
    );
}

export async function apiRequest<T>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<T> {
    const {
        json,
        accessToken,
        headers: customHeaders,
        ...requestOptions
    } = options;

    const normalizedPath = path.startsWith("/")
        ? path
        : `/${path}`;

    const url = `${API_BASE_URL}${normalizedPath}`;

    const body =
        json === undefined
        ? undefined
        : JSON.stringify(json);

    const sendRequest = (
        token: string | null | undefined,
    ): Promise<Response> => {
        const headers = new Headers(customHeaders);

        headers.set("Accept", "application/json");

        if (json !== undefined) {
            headers.set("Content-Type", "application/json");
        }

        if (token) {
            headers.set("Authorization", `Bearer ${token}`);
        }

        return fetch(url, {
            ...requestOptions,
            headers,
            credentials: "include",
            body,
        });
    };

    let response = await sendRequest(accessToken);

  
    if (
        response.status === 401 &&
        accessToken &&
        unauthorizedHandler &&
        !requestOptions.signal?.aborted
    ) {
        const refreshedToken =
        await unauthorizedHandler();

    
        if (
            refreshedToken &&
            refreshedToken !== accessToken &&
            !requestOptions.signal?.aborted
        ) {
            response = await sendRequest(refreshedToken);
        }
    }

    if (!response.ok) {
        await throwApiError(response);
    }

    if (
        response.status === 204 ||
        response.status === 205
    ) {
        return undefined as T;
    }

    const contentType =
        response.headers.get("content-type") ?? "";

    if (contentType.includes("application/json")) {
        return (await response.json()) as T;
    }

    return (await response.text()) as T;
}