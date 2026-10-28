import {
  Navigate,
  Outlet,
} from "react-router";
import { useAuth } from "../../features/auth/model/useAuth";
import { PageLoader } from "./PageLoader";

export function GuestOnlyRoute() {
    const { status, isAuthenticated } =useAuth();

    if (status === "loading") {
        return <PageLoader />;
    }

    if (isAuthenticated) {
        return (
            <Navigate
                to="/profile"
                replace
            />
        );
    }

    return <Outlet />;
}