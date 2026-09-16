using EventManager.Domain.Common;
using System.Net.Mail;

namespace EventManager.Domain.Users.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
        => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                "Email is required.");

        value = value.Trim().ToLowerInvariant();

        try
        {
            var address = new MailAddress(value);

            if (!string.Equals(address.Address, value,
                StringComparison.OrdinalIgnoreCase))
                throw new DomainException(
                    "Invalid  email.");
        }
        catch
        {
            throw new DomainException(
                "Invalid email.");
        }

        if (value.Length > 254)
            throw new DomainException(
                "Email is too long.");

        return new Email(value);
    }

    public bool Equals(Email? other)
        => other is not null &&
        string.Equals(Value, other.Value,
            StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj)
        => obj is Email email && Equals(email);

    public override int GetHashCode()
        => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString()
        => Value;
}
