import { useContext } from "react";
import { AuthContext, } from "../api/authContext";
import type {
    AuthContextValue,
} from "../api/authContext";

export function useAuth() : AuthContextValue {
    const context = useContext(AuthContext);

    if (context === undefined) {
        throw new Error(
            "useAuth must be used inside AuthProvider.",
        );
    }

    return context;
}