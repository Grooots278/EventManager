import {
  useState,
  type SubmitEvent,
} from "react";

import {
  Link,
  useLocation,
  useNavigate,
} from "react-router";

import { useAuth } from "../features/auth/model/useAuth";
import { ApiError } from "../shared/api/httpClient";

export function LoginPage() {
  const { login } = useAuth();

  const navigate = useNavigate();
  const location = useLocation();

  const [loginValue, setLoginValue] =
    useState("");

  const [password, setPassword] =
    useState("");

  const [error, setError] =
    useState<string | null>(null);

  const [isSubmitting, setIsSubmitting] =
    useState(false);

  async function handleSubmit(
    event: SubmitEvent<HTMLFormElement>,
  ) {
    event.preventDefault();

    setError(null);
    setIsSubmitting(true);

    try {
      await login({
        login: loginValue.trim(),
        password,
      });

      const state = location.state as
        | { from?: { pathname?: string } }
        | null;

      const destination =
        state?.from?.pathname ?? "/profile";

      const safeDestination =
        destination.startsWith("/") &&
        !destination.startsWith("//")
          ? destination
          : "/profile";

      navigate(safeDestination, {
        replace: true,
      });
    } catch (exception: unknown) {
      if (exception instanceof ApiError) {
        setError(
          exception.status === 401
            ? "Invalid username or password."
            : exception.message,
        );
      } else {
        setError(
          "Failed to log in. Check the connection and try again.",
        );
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="auth-page">
      <div className="auth-card">
        <div className="auth-card__heading">
          <span className="eyebrow">
            welcome back
          </span>

          <h1>Log in to your account</h1>

          <p>
            Log in to manage your profile.
          </p>
        </div>

        <form
          className="auth-form"
          onSubmit={handleSubmit}
        >
          <label
            className="form-field"
            htmlFor="login"
          >
            <span>Login</span>

            <input
              id="login"
              name="login"
              type="text"
              autoComplete="username"
              value={loginValue}
              onChange={(event) =>
                setLoginValue(event.target.value)
              }
              minLength={3}
              maxLength={32}
              required
              autoFocus
              placeholder="Your login"
            />
          </label>

          <label
            className="form-field"
            htmlFor="password"
          >
            <span>Password</span>

            <input
              id="password"
              name="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(event) =>
                setPassword(event.target.value)
              }
              required
              placeholder="Input password"
            />
          </label>

          {error && (
            <div
              className="form-error"
              role="alert"
            >
              {error}
            </div>
          )}

          <button
            className="button button--primary button--full"
            type="submit"
            disabled={isSubmitting}
          >
            {isSubmitting
              ? "Logging in..."
              : "Enter"}
          </button>
        </form>

        <p className="auth-card__footer">
          There is no account yet?{" "}
          <Link to="/register">
            Register
          </Link>
        </p>
      </div>
    </section>
  );
}