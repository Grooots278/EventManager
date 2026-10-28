import {
    Navigate,
    Outlet,
    useLocation,
} from "react-router";
import { useAuth } from "../../features/auth/model/useAuth";
import { PageLoader } from "./PageLoader";

export function ProtectedRoute() {
    const { status, isAuthenticated } = useAuth();
    const location = useLocation();

    if (status === "loading") {
        return <PageLoader />;
    }

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/login"
                replace
                state={{
                    from: location,
                }}
            />
        );
    }

    return <Outlet />;
}