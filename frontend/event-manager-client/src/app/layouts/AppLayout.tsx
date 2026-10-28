import {
  Link,
  NavLink,
  Outlet,
  useNavigate,
} from "react-router";
import { useAuth } from "../../features/auth/model/useAuth";

export function AppLayout() {
    const {
        status,
        user,
        logout,
    } = useAuth();

    const navigate = useNavigate();

    async function handleLogout() {
        try {
            await logout();
        } finally {
            navigate("/", { replace: true });
        }
    }

    return (
        <div className="app-shell">
            <header className="site-header">
                <div className="site-header__inner">
                    <Link
                        to="/"
                        className="brand"
                        aria-label="EventManager - main">
                        <span className="brand__mark">E</span>
                        <span>EventManager</span>
                    </Link>
                    <nav className="site-nav"
                        aria-label="Basic navigation">
                        <NavLink
                            to="/"
                            end
                            className={({ isActive }: {isActive: boolean}) => 
                                `site-nav__link ${
                                    isActive ? "is-active" : ""
                                }`}>
                            Main
                        </NavLink>
                        {status === "authenticated" && (
                            <NavLink
                                to="/profile"
                                className={({ isActive }: {isActive: boolean}) =>
                                    `site-nav__link ${
                                    isActive ? "is-active" : ""
                                    }`
                                }>
                                 Profile
                            </NavLink>
                        )}
                    </nav>

                    <div className="site-header__actions">
                        {status === "loading" ? (
                            <span className="header-status">
                                Loading...
                            </span>
                        ) : status === "authenticated" ? (
                            <>
                                <span className="header-user">
                                    {user?.firstName}
                                </span>

                                <button
                                    type="button"
                                    className="button button--secondary button--small"
                                    onClick={handleLogout}
                                >
                                    Exit
                                </button>
                            </>
                        ) : (
                            <>
                                <Link
                                    to="/login"
                                    className="button button--secondary button--small"
                                >
                                    Enter
                                </Link>

                                <Link
                                    to="/register"
                                    className="button button--primary button--small"
                                >
                                    Registration
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </header>

            <main className="main-content">
                <Outlet />
            </main>

            <footer className="site-footer">
                <div className="site-footer__inner">
                    <span>EventManager</span>
                    <span>Find events based on your interests</span>
                </div>
            </footer>
        </div>
    );
}