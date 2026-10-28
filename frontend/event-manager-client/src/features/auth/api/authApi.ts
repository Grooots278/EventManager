import { apiRequest } from "../../../shared/api/httpClient";

import type {
    AuthTokenResponse,
    LoginRequest,
    RegisterRequest
} from "../../../entities/user/model/types";

export const authApi = {
    register(
        request: RegisterRequest,
    ) : Promise<AuthTokenResponse> {
        return apiRequest<AuthTokenResponse>(
            "/auth/register",
            {
                method: "POST",
                json: request,
            },
        );
    },

    login(
        request: LoginRequest,
    ) : Promise<AuthTokenResponse> {
        return apiRequest<AuthTokenResponse>(
            "/auth/login",
            {
                method: "POST",
                json: request,
            },
        );
    },

    refresh() : Promise<AuthTokenResponse> {
        return apiRequest<AuthTokenResponse>(
            "/auth/refresh",
            {
                method: "POST",
            },
        );
    },

    logout() : Promise<void> {
        return apiRequest<void>(
            "/auth/logout",
            {
                method: "POST",
            },
        );
    },
};