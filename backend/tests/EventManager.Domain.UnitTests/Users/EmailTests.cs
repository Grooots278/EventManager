using EventManager.Domain.Common;
using EventManager.Domain.Users.ValueObjects;

namespace EventManager.Domain.UnitTests.Users;

public sealed class EmailTests
{
    [Fact]
    public void Create_ShouldNormalizeEmail()
    {
        var email =
            Email.Create(
                "Dmitry@example.COM");

        Assert.Equal(
            "dmitry@example.com",
            email.Value);
    }

    [Fact]
    public void Create_ShouldRejectEmptyEmail()
    {
        Assert.Throws<DomainException>(
            () => Email.Create(""));
    }

    [Fact]
    public void Create_ShouldRejectInvalidEmail()
    {
        Assert.Throws<DomainException>(
            () => Email.Create("not-an-email"));
    }

    [Fact]
    public void Create_ShouldCompareByValue()
    {
        var first = 
            Email.Create("DMITRY@EXAMPLE.COM");

        var second = 
            Email.Create("dmitry@example.com");

        Assert.Equal(first, second);
    }
}