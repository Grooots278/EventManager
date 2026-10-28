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