namespace EventManager.Infrastructure.Bootstrap;

public sealed class BootrstrapAdminOptions
{
    public const string SectionName =
        "BootstrapAdmin";

    public bool Enabled { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly BirthDate { get; init; }
}