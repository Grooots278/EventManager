import { Link } from "react-router";

import { useAuth } from "../features/auth/model/useAuth";

export function HomePage() {
  const { isAuthenticated, user } = useAuth();

  return (
    <section className="hero">
      <div className="hero__content">
        <span className="eyebrow">
          YOUR INTERESTS. YOUR EVENTS.
        </span>

        <h1 className="hero__title">
          Find events,
          <br />
          which inspire.
        </h1>

        <p className="hero__description">
          EventManager will help you find interesting events.
          events and discover new opportunities
          for socializing and relaxing.
        </p>

        <div className="hero__actions">
          {isAuthenticated ? (
            <Link
              to="/profile"
              className="button button--primary"
            >
              Go to Profile
            </Link>
          ) : (
            <>
              <Link
                to="/register"
                className="button button--primary"
              >
                Create an account
              </Link>

              <Link
                to="/login"
                className="button button--secondary"
              >
                I already have an account
              </Link>
            </>
          )}
        </div>

        {isAuthenticated && user && (
          <p className="hero__welcome">
            Glad to see you, {user.firstName}.
          </p>
        )}
      </div>

      <div
        className="hero__visual"
        aria-hidden="true"
      >
        <div className="visual-orbit visual-orbit--outer" />
        <div className="visual-orbit visual-orbit--inner" />

        <div className="visual-center">
          <span className="visual-center__symbol">E</span>
          <span className="visual-center__caption">
            FIND YOUR NEXT
          </span>
          <span className="visual-center__caption">
            EXPERIENCE
          </span>
        </div>

        <span className="visual-tag visual-tag--one">
          Music
        </span>

        <span className="visual-tag visual-tag--two">
          Art
        </span>

        <span className="visual-tag visual-tag--three">
          People
        </span>
      </div>
    </section>
  );
}