export type UserRole =
    | "User"
    | "Admin";

export interface AuthTokenResponse {
    accessToken: string;
    accessTokenExpiresAtUtc: string;
}

export interface LoginRequest {
    login: string;
    password: string;
}

export interface RegisterRequest {
    login: string;
    password: string;
    firstName: string;
    lastName: string;
    email: string;
    birthDate: string;
    cityId: string | null;
}

export interface CurrentUserResponse {
    id: string;
    login: string;
    firstName: string;
    lastName: string;
    email: string;
    birthDate: string;
    cityId: string | null;
    cityName: string | null;
    role: UserRole;
    isActive: boolean;
}

export interface UpdateProfileRequest {
    firstName: string;
    lastName: string;
    birthDate: string;
    cityId: string | null;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export interface CityResponse {
    id: string;
    name: string;
}