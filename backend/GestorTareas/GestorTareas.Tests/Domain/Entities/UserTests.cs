using FluentAssertions;
using GestorTareas.Domain.Entities;
using GestorTareas.Domain.Exceptions;
using Xunit;

namespace GestorTareas.Tests.Domain.Entities;
public class UserTests
{
    private const string ValidEmail = "juan@correo.com";
    private const string ValidName = "Juan Pérez";
    private const string ValidPasswordHash = "$2a$11$abcdefghijklmnopqrstuvwxyz123456";

    private static User CreateValidUser(string? email = null, string? name = null, string? passwordHash = null)
    {
        return User.Create(email ?? ValidEmail, name ?? ValidName, passwordHash ?? ValidPasswordHash);
    }


    // CREACIÓN: CASOS DE ÉXITO
    [Fact]
    public void Create_WithValidData_ShouldReturnUser()
    {
        var user = User.Create(ValidEmail, ValidName, ValidPasswordHash);

        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Value.Should().Be(ValidEmail);
        user.Name.Should().Be(ValidName);
        user.PasswordHash.Should().Be(ValidPasswordHash);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentUtcTime()
    {
        var before = DateTime.UtcNow;
             
        var user = CreateValidUser();
        var after = DateTime.UtcNow;

        user.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        user.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Create_TwoUsers_ShouldHaveDifferentIds()
    {
        var user1 = CreateValidUser();
        var user2 = CreateValidUser();

        user1.Id.Should().NotBe(user2.Id);
    }

    [Fact]
    public void Create_WithNameContainingSpaces_ShouldTrimName()
    {
        var user = CreateValidUser(name: "   Juan Pérez   ");
        user.Name.Should().Be("Juan Pérez");
    }


    // CREACIÓN: CASOS DE ERROR (EMAIL)
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ShouldThrowInvalidEmailException(string email)
    {
        var action = () => CreateValidUser(email: email);
        action.Should().Throw<InvalidEmailException>();
    }

    [Theory]
    [InlineData("sin-arroba")]
    [InlineData("@sin-local.com")]
    [InlineData("sin-dominio@")]
    public void Create_WithInvalidEmailFormat_ShouldThrowInvalidEmailException(string email)
    {
        var action = () => CreateValidUser(email: email);
        action.Should().Throw<InvalidEmailException>();
    }


    // CREACIÓN: CASOS DE ERROR (NAME)
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrowInvalidUserException(string name)
    {
        var action = () => CreateValidUser(name: name);
        action.Should().Throw<InvalidUserException>().WithMessage("*vacío*");
    }

    [Fact]
    public void Create_WithNameExceedingMaxLength_ShouldThrowInvalidUserException()
    {
        var tooLongName = new string('a', 101);
        var action = () => CreateValidUser(name: tooLongName);
        action.Should().Throw<InvalidUserException>().WithMessage("*más de 100*");
    }

    [Fact]
    public void Create_WithNameAtMaxLength_ShouldSucceed()
    {
        var name = new string('a', 100);
        var action = () => CreateValidUser(name: name);
        action.Should().NotThrow();
    }


    // CREACIÓN: CASOS DE ERROR (PASSWORD HASH)
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyPasswordHash_ShouldThrowInvalidUserException(string hash)
    {
        var action = () => CreateValidUser(passwordHash: hash);     
        action.Should().Throw<InvalidUserException>().WithMessage("*contraseña*");
    }

    // UPDATENAME
    [Fact]
    public void UpdateName_WithValidName_ShouldChangeName()
    {
        var user = CreateValidUser();
        var newName = "Pedro García";
        user.UpdateName(newName);
        user.Name.Should().Be(newName);
    }

    [Fact]
    public void UpdateName_WithSpaces_ShouldTrimName()
    {
        var user = CreateValidUser();
        user.UpdateName("   Pedro   ");
        user.Name.Should().Be("Pedro");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithEmptyName_ShouldThrowInvalidUserException(string newName)
    {
        var user = CreateValidUser();
        var action = () => user.UpdateName(newName);
        action.Should().Throw<InvalidUserException>();
    }

    [Fact]
    public void UpdateName_WithTooLongName_ShouldThrowInvalidUserException()
    {
        var user = CreateValidUser();
        var tooLong = new string('a', 101);
        var action = () => user.UpdateName(tooLong);
        action.Should().Throw<InvalidUserException>();
    }

    [Fact]
    public void UpdateName_WhenValidationFails_ShouldNotChangeOriginalName()
    {
        var user = CreateValidUser(name: "Original");
        var action = () => user.UpdateName("");
    
        action.Should().Throw<InvalidUserException>();
        user.Name.Should().Be("Original");
    }

    // CHANGEPASSWORD
    [Fact]
    public void ChangePassword_WithValidHash_ShouldChangePasswordHash()
    {
        var user = CreateValidUser();
        var newHash = "$2a$11$nuevohashabcdefghijklmnopqrstuv";
        user.ChangePassword(newHash);
        user.PasswordHash.Should().Be(newHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePassword_WithEmptyHash_ShouldThrowInvalidUserException(string hash)
    {
        var user = CreateValidUser();
        var action = () => user.ChangePassword(hash);
        action.Should().Throw<InvalidUserException>();
    }

    [Fact]
    public void ChangePassword_WhenValidationFails_ShouldNotChangeOriginalHash()
    {
        var user = CreateValidUser();
        var originalHash = user.PasswordHash;

        var action = () => user.ChangePassword("");

        action.Should().Throw<InvalidUserException>();
        user.PasswordHash.Should().Be(originalHash);
    }
}