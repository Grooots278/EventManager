import { Link } from "react-router";

import { useAuth } from "../features/auth/model/useAuth";

export function ProfilePage() {
  const { user } = useAuth();

  if (!user) {
    return null;
  }

  return (
    <section className="profile-page">
      <div className="page-heading">
        <span className="eyebrow">
          personal account
        </span>

        <h1>My profile</h1>

        <p>
          Manage your personal data and account settings.
        </p>
      </div>

      <div className="profile-card">
        <div className="profile-card__avatar">
          {user.firstName.charAt(0).toUpperCase()}
        </div>

        <div className="profile-card__identity">
          <h2>
            {user.firstName} {user.lastName}
          </h2>

          <p>{user.email}</p>
        </div>

        <span className="role-badge">
          {user.role}
        </span>
      </div>

      <div className="profile-details">
        <div className="profile-detail">
          <span>Login</span>
          <strong>{user.login}</strong>
        </div>

        <div className="profile-detail">
          <span>Date of birth</span>
          <strong>{user.birthDate}</strong>
        </div>

        <div className="profile-detail">
          <span>City</span>
          <strong>{user.cityName ?? "Not specified"}</strong>
        </div>

        <div className="profile-detail">
          <span>Status</span>
          <strong>
            {user.isActive ? "Active" : "Inactive"}
          </strong>
        </div>
      </div>

      <div className="profile-actions">
        <Link
          to="/profile/edit"
          className="button button--primary"
        >
          Edit Profile
        </Link>

        <Link
          to="/profile/password"
          className="button button--secondary"
        >
          Change the password
        </Link>
      </div>
    </section>
  );
}