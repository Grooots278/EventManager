using EventManager.Domain.Common;

namespace EventManager.Domain.Users.ValueObjects;

public sealed class Login : IEquatable<Login>
{
    public string Value { get; }

    private Login(string value) => 
        Value = value;

    public static Login Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                "Login is required.");

        value = value.Trim();

        if (value.Length is < 3 or > 32)
            throw new DomainException(
                "Login length must be between 3 and 32 characters.");

        if (!value.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.'))
            throw new DomainException(
                "Login contains invalid characters.");

        return new Login(value);
    }

    public bool Equals(Login? other)
        => other is not null &&
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj)
        => obj is Login login && Equals(login);

    public override int GetHashCode()
        => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    public static implicit operator string(Login login) => login.Value;
}
