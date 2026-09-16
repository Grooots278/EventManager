using EventManager.Domain.Common;

namespace EventManager.Domain.Users.ValueObjects;

public sealed class LastName
{
    public string Value { get; }

    public LastName(string value) =>
        Value = value;

    public static LastName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                "Last name is required.");

        value = value.Trim();

        if (value.Length > 100)
            throw new DomainException(
                "Last name is too long");

        return new LastName(value);
    }

    public override string ToString()
        => Value;
}
