import { apiRequest } from "../../../shared/api/httpClient";

import type {
    ChangePasswordRequest,
    CurrentUserResponse,
    UpdateProfileRequest,
} from "../../../entities/user/model/types";

export const usersApi = {
    getMe(
        accessToken: string,
    ) : Promise<CurrentUserResponse> {
        return apiRequest<CurrentUserResponse>(
            "/users/me",
            {
                method: "GET",
                accessToken
            },
        );
    },

    updateProfile(
        accessToken: string,
        request: UpdateProfileRequest,
    ) : Promise<void> {
        return apiRequest<void> (
            "/users/me/profile",
            {
                method: "PUT",
                json: request,
                accessToken,
            },
        );
    },

    changePassword(
        accessToken: string,
        request: ChangePasswordRequest,
    ) : Promise<void> {
        return apiRequest<void>(
            "/users/me/password",
            {
                method: "PUT",
                json: request,
                accessToken,
            },
        );
    },
};