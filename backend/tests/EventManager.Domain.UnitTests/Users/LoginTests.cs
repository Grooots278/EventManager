using EventManager.Domain.Common;
using EventManager.Domain.Users.ValueObjects;

namespace EventManager.Domain.UnitTests.Users;

public sealed class LoginTests
{
    [Fact]
    public void Create_ShouldNormilizeLogin()
    {
        const string value = "Dmitry_123";

        var login = Login.Create(value);

        Assert.Equal(
            "dmitry_123",
            login.Value
        );
    }

    [Fact]
    public void Create_ShouldRejectEmptyLogin()
    {
        var action = () => 
            Login.Create(" ");

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldRejectShortLogin()
    {
        var action = () => 
            Login.Create("ab");

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldRejectInvalidCharacters()
    {
        var action = () => 
            Login.Create("dmitry@test");

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldCompareByValue()
    {
        var first = 
            Login.Create("Dmitry");

        var second = 
            Login.Create("dmitry");


        Assert.Equal(first, second);
    }
}