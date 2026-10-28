import {
    useCallback,
    useEffect,
    useMemo,
    useRef,
    useState,
} from "react";

import type { ReactNode } from "react";

import { authApi } from "../../features/auth/api/authApi";
import { usersApi } from "../../features/auth/api/usersApi";

import { AuthContext, } from "../../features/auth/api/authContext";

import type { AuthContextValue, AuthStatus, } from "../../features/auth/api/authContext";
import { registerUnauthorizedHandler } from "../../shared/api/httpClient";

import type {
    AuthTokenResponse,
    ChangePasswordRequest,
    CurrentUserResponse,
    LoginRequest,
    RegisterRequest,
    UpdateProfileRequest,
} from "../../entities/user/model/types";

interface AuthProviderProps {
    children: ReactNode;
}

export function AuthProvider({
    children,
} : AuthProviderProps) {
    const [accessToken, setAccessTokenState] =
        useState<string | null>(null);

    const [user, setUser] = 
        useState<CurrentUserResponse | null>(null);

    const [status, setStatus] =
        useState<AuthStatus>("loading");

    const accessTokenRef = 
        useRef<string | null>(null);

    const refreshPromiseRef =
        useRef<Promise<string | null> | null>(null);

    const setAccessToken = useCallback(
        (token: string | null) => {
            accessTokenRef.current = token;
            setAccessTokenState(token);
        },
        [],
    );

    const clearSession = useCallback(() => {
        setAccessToken(null);
        setUser(null);
        setStatus("anonymous");
    }, [setAccessToken]);

    const renewAccessToken = useCallback(
        () : Promise<string| null> => {
            if (refreshPromiseRef.current) {
                return refreshPromiseRef.current;
            }

            const refreshPromise = authApi
                .refresh()
                .then((response) => {
                    setAccessToken(response.accessToken);
                    return response.accessToken;
                })
                .catch(() => {
                    clearSession();
                    return null;
                });

            refreshPromiseRef.current = refreshPromise;

            void refreshPromise.then(() => {
                if (refreshPromiseRef.current === refreshPromise) {
                    refreshPromiseRef.current = null;
                }
            });

            return refreshPromise;
        },
        [clearSession, setAccessToken],
    );

    const completeAuthentication = useCallback(
        async (response: AuthTokenResponse): Promise<void> => {
            setAccessToken(response.accessToken);

            try {
                const currentUser = 
                    await usersApi.getMe(response.accessToken);

                setUser(currentUser);
                setStatus("authenticated");
            } catch (error: unknown) {
                try {
                    await authApi.logout();
                } catch { }
                clearSession();
                throw error;
            }
        },
        [clearSession, setAccessToken],
    );

    const login = useCallback(
        async (request: LoginRequest): Promise<void> => {
            const response = await authApi.login(request);

            await completeAuthentication(response);
        },
        [completeAuthentication],
    );

    const register = useCallback(
        async (request: RegisterRequest): Promise<void> => {
            const response = await authApi.register(request);

            await completeAuthentication(response);
        },
        [completeAuthentication],
    );

    const logout = useCallback(
        async (): Promise<void> => {
            try {
                await authApi.logout();
            } finally {
                clearSession();
            }
        },
        [clearSession],
    );

    const updateProfile = useCallback(
        async (
            request: UpdateProfileRequest,
        ): Promise<void> => {
            const currentToken = accessTokenRef.current;

            if (!currentToken) {
                throw new Error("You must be authenticated.");
            }

            await usersApi.updateProfile(
                currentToken,
                request,
            );

        const latestToken = accessTokenRef.current;

        if (!latestToken) {
            clearSession();

            throw new Error("Your session has expired.");
        }

        const updatedUser =
            await usersApi.getMe(latestToken);

            setUser(updatedUser);
        },
        [clearSession],
    );

    const changePassword = useCallback(
        async (
            request: ChangePasswordRequest,
        ): Promise<void> => {
            const currentToken = accessTokenRef.current;

            if (!currentToken) {
                throw new Error("You must be authenticated.");
            }

            await usersApi.changePassword(
                currentToken,
                request,
            );

        try {
            await authApi.logout();
        } finally {
            clearSession();
        }
        },
        [clearSession],
    );

  
    useEffect(() => {
        return registerUnauthorizedHandler(
            renewAccessToken,
        );
        }, [renewAccessToken]);

  
    useEffect(() => {
        let active = true;

        const initializeSession = async () => {
            const token = await renewAccessToken();

            if (!active) {
                return;
            }

            if (!token) {
                setStatus("anonymous");
                return;
            }

            try {
                const currentUser =
                await usersApi.getMe(token);

                if (!active) {
                    return;
                }

                setUser(currentUser);
                setStatus("authenticated");
            } catch {
                if (active) {
                    clearSession();
                }
            }
        };

        void initializeSession();

        return () => {
            active = false;
        };
        }, [clearSession, renewAccessToken]);

    const contextValue =
        useMemo<AuthContextValue>(
        () => ({
            status,

            isAuthenticated:
                status === "authenticated" &&
                accessToken !== null &&
                user !== null,

                accessToken,

                user,

                login,
                register,
                logout,
                updateProfile,
                changePassword,
            }),
            [
                status,
                accessToken,
                user,
                login,
                register,
                logout,
                updateProfile,
                changePassword,
            ],
        );

    return (
        <AuthContext.Provider value={contextValue}>
        {children}
        </AuthContext.Provider>
    );
}