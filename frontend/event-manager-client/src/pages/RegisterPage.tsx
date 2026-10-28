import {
  useState,
  type SubmitEvent,
} from "react";

import {
  Link,
  useNavigate,
} from "react-router";

import { useAuth } from "../features/auth/model/useAuth";
import { ApiError } from "../shared/api/httpClient";

export function RegisterPage() {
  const { register } = useAuth();

  const navigate = useNavigate();

  const [loginValue, setLoginValue] =
    useState("");

  const [password, setPassword] =
    useState("");

  const [confirmPassword, setConfirmPassword] =
    useState("");

  const [firstName, setFirstName] =
    useState("");

  const [lastName, setLastName] =
    useState("");

  const [email, setEmail] =
    useState("");

  const [birthDate, setBirthDate] =
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

    if (password !== confirmPassword) {
      setError("Passwords don't match.");
      return;
    }

    if (password.length < 12) {
      setError(
        "The password must contain at least 12 characters.",
      );
      return;
    }

    if (!birthDate) {
      setError("Specify the date of birth.");
      return;
    }

    if (birthDate > new Date().toISOString().slice(0, 10)) {
      setError(
        "The date of birth cannot be in the future.",
      );
      return;
    }

    setIsSubmitting(true);

    try {
      await register({
        login: loginValue.trim(),
        password,
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        birthDate,
        cityId: null,
      });

      navigate("/profile", {
        replace: true,
      });
    } catch (exception: unknown) {
      if (exception instanceof ApiError) {
        setError(exception.message);
      } else {
        setError(
          "Failed to create an account. Try again.",
        );
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  const today =
    new Date().toISOString().slice(0, 10);

  return (
    <section className="auth-page">
      <div className="auth-card auth-card--wide">
        <div className="auth-card__heading">
          <span className="eyebrow">
            FIRST STEP
          </span>

          <h1>Create an account</h1>

          <p>
            Fill in the details and you will be able to use
            EventManager.
          </p>
        </div>

        <form
          className="auth-form"
          onSubmit={handleSubmit}
        >
          <div className="form-grid">
            <label
              className="form-field"
              htmlFor="firstName"
            >
              <span>Name</span>

              <input
                id="firstName"
                name="firstName"
                autoComplete="given-name"
                value={firstName}
                onChange={(event) =>
                  setFirstName(event.target.value)
                }
                maxLength={100}
                required
                placeholder="Name"
              />
            </label>

            <label
              className="form-field"
              htmlFor="lastName"
            >
              <span>Last name</span>

              <input
                id="lastName"
                name="lastName"
                autoComplete="family-name"
                value={lastName}
                onChange={(event) =>
                  setLastName(event.target.value)
                }
                maxLength={100}
                required
                placeholder="Last name"
              />
            </label>
          </div>

          <label
            className="form-field"
            htmlFor="registerLogin"
          >
            <span>Login</span>

            <input
              id="registerLogin"
              name="login"
              autoComplete="username"
              value={loginValue}
              onChange={(event) =>
                setLoginValue(event.target.value)
              }
              minLength={3}
              maxLength={32}
              pattern="[\p{L}\p{N}_.]+"
              title="Use letters, numbers, dots, and underscores."
              required
              placeholder="Example, dmitry_01"
            />
          </label>

          <label
            className="form-field"
            htmlFor="email"
          >
            <span>Email</span>

            <input
              id="email"
              name="email"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(event) =>
                setEmail(event.target.value)
              }
              maxLength={254}
              required
              placeholder="you@example.com"
            />
          </label>

          <label
            className="form-field"
            htmlFor="birthDate"
          >
            <span>BirthDate</span>

            <input
              id="birthDate"
              name="birthDate"
              type="date"
              autoComplete="bday"
              value={birthDate}
              onChange={(event) =>
                setBirthDate(event.target.value)
              }
              max={today}
              required
            />
          </label>

          <div className="form-grid">
            <label
              className="form-field"
              htmlFor="registerPassword"
            >
              <span>Password</span>

              <input
                id="registerPassword"
                name="password"
                type="password"
                autoComplete="new-password"
                value={password}
                onChange={(event) =>
                  setPassword(event.target.value)
                }
                minLength={12}
                maxLength={128}
                required
                placeholder="Minimum of 12 characters"
              />
            </label>

            <label
              className="form-field"
              htmlFor="confirmPassword"
            >
              <span>Password Replay</span>

              <input
                id="confirmPassword"
                name="confirmPassword"
                type="password"
                autoComplete="new-password"
                value={confirmPassword}
                onChange={(event) =>
                  setConfirmPassword(event.target.value)
                }
                minLength={12}
                maxLength={128}
                required
                placeholder="Password Replay"
              />
            </label>
          </div>

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
              ? "Creating an account..."
              : "Registration"}
          </button>
        </form>

        <p className="auth-card__footer">
          I already have an account?{" "}
          <Link to="/login">
            Enter
          </Link>
        </p>
      </div>
    </section>
  );
}