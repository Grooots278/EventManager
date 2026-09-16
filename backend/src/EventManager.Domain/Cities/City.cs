using EventManager.Domain.Common;

namespace EventManager.Domain.Cities;

public sealed class City : Entity
{
    public string Name { get; private set; } = null!;

    private City() { }

    private City(string name) => Name = name;

    public static City Create(string name)
    {
        ValidateName(ref name);

        return new City(name);
    }

    public void Rename(string name)
    {
        if (Name == name) return;
        ValidateName(ref name);
        Name = name;
    }

    private static void ValidateName(ref string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(
                "City name is required.");

        name = name.Trim();

        if (name.Length > 100)
            throw new DomainException(
                "City name is too long.");
    }
}
