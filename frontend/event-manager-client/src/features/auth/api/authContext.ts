import { createContext } from "react";

import type {
    ChangePasswordRequest,
    CurrentUserResponse,
    LoginRequest,
    RegisterRequest,
    UpdateProfileRequest,
} from "../../../entities/user/model/types";

export type AuthStatus =
    | "loading"
    | "authenticated"
    | "anonymous";

export interface AuthContextValue {
    status: AuthStatus;
    isAuthenticated: boolean;
    accessToken: string | null;
    user: CurrentUserResponse | null;

    login(request: LoginRequest): Promise<void>;
    register(request: RegisterRequest): Promise<void>;
    logout(): Promise<void>;
    updateProfile(request: UpdateProfileRequest): Promise<void>;
    changePassword(request: ChangePasswordRequest): Promise<void>;
}

export const AuthContext = 
    createContext<AuthContextValue | undefined>(undefined,);