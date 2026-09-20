using EventManager.Domain.Abstractions;
using EventManager.Domain.Enums;
using EventManager.Domain.Users.ValueObjects;

namespace EventManager.Domain.Users
{
    public sealed class UserProfile : IAuditable
    {
        public Guid UserId { get; private set; }
        public FirstName FirstName { get; private set; } = null!;
        public LastName LastName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public DateOnly BirthDate { get; private set; }
        public Guid? CityId { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        private UserProfile() { }

        internal UserProfile(
            Guid userId,
            FirstName firstName,
            LastName lastName,
            Email email,
            DateOnly birthDate,
            Guid? cityId)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            CityId = cityId;
            Role = UserRole.User;
            IsActive = true;
        }

        public void UpdatePersonalInformation(
            FirstName firstName,
            LastName lastName,
            DateOnly birtDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birtDate;
        }

        public void ChangeName(
            FirstName firstName,
            LastName lastName)
        {
            if (firstName == FirstName && LastName == LastName) return;

            FirstName = firstName;
            LastName = lastName;
        }

        public void ChangeEmail(
            Email email)
        {
            if (Email == email) return;
            Email = email;
        }

        public void ChangeCity(Guid? cityId)
        {
            if (cityId == CityId) return;
            CityId = cityId;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

        public void ChangeRole(
            UserRole role)
        {
            if (Role == role) return;
            Role = role;
        }
    }
}
