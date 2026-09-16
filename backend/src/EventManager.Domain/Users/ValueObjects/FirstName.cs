using EventManager.Domain.Common;

namespace EventManager.Domain.Users.ValueObjects;

public sealed class FirstName
{
    public string Value { get; }

    public FirstName(string value)
        => Value = value;

    public static FirstName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                "First name is required.");

        value = value.Trim();

        if (value.Length > 100)
            throw new DomainException(
                "First name is too long.");

        return new FirstName(value);
    }

    public override string ToString()
        => Value;
}
