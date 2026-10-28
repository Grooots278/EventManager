import {
  useState,
  type FormEvent,
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
    event: FormEvent<HTMLFormElement>,
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

      // Не разрешаем redirect на произвольный внешний URL.
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
            ? "Неверный логин или пароль."
            : exception.message,
        );
      } else {
        setError(
          "Не удалось выполнить вход. Проверь подключение и попробуй ещё раз.",
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
            С ВОЗВРАЩЕНИЕМ
          </span>

          <h1>Войти в аккаунт</h1>

          <p>
            Войди, чтобы управлять своим профилем.
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
            <span>Логин</span>

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
              placeholder="Твой логин"
            />
          </label>

          <label
            className="form-field"
            htmlFor="password"
          >
            <span>Пароль</span>

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
              placeholder="Введи пароль"
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
              ? "Выполняем вход..."
              : "Войти"}
          </button>
        </form>

        <p className="auth-card__footer">
          Ещё нет аккаунта?{" "}
          <Link to="/register">
            Зарегистрироваться
          </Link>
        </p>
      </div>
    </section>
  );
}