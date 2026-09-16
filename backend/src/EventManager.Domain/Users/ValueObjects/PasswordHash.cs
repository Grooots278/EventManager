using EventManager.Domain.Common;

namespace EventManager.Domain.Users.ValueObjects;

public sealed class PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
        => Value = value;

    public static PasswordHash FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new DomainException(
                "Password hash is required.");

        return new PasswordHash(hash);
    }

    public override string ToString()
        => Value;
}
