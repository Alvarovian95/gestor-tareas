using FluentAssertions;
using GestorTareas.Domain.Entities;
using GestorTareas.Domain.Exceptions;
using Xunit;

namespace GestorTareas.Tests.Domain.Entities;
public class CategoryTests
{
    // Datos válidos por defecto.
    private const string ValidName = "Trabajo";
    private const string ValidColor = "#3498DB";
    private static readonly Guid ValidUserId = Guid.NewGuid();

    /// <summary>
    /// Helper para crear categorías válidas en los tests.
    /// </summary>
    private static Category CreateValidCategory(
        string? name = null,
        Guid? userId = null,
        string? color = null)
    {
        return Category.Create(
            name ?? ValidName,
            userId ?? ValidUserId,
            color ?? ValidColor);
    }

    // ──────────────────────────────────────────────────────
    // CREACIÓN: CASOS DE ÉXITO
    // ──────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ShouldReturnCategory()
    {
        // Act
        var category = Category.Create(ValidName, ValidUserId, ValidColor);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().NotBe(Guid.Empty);
        category.Name.Should().Be(ValidName);
        category.Color.Should().Be(ValidColor);
        category.UserId.Should().Be(ValidUserId);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentUtcTime()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var category = CreateValidCategory();
        var after = DateTime.UtcNow;

        // Assert
        category.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        category.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Create_WithNameContainingSpaces_ShouldTrimName()
    {
        // Act
        var category = CreateValidCategory(name: "   Trabajo   ");

        // Assert
        category.Name.Should().Be("Trabajo");
    }

    [Fact]
    public void Create_WithLowercaseColor_ShouldNormalizeToUppercase()
    {
        // Act
        var category = CreateValidCategory(color: "#3498db");

        // Assert
        category.Color.Should().Be("#3498DB");
    }

    [Fact]
    public void Create_WithoutColor_ShouldUseDefaultColor()
    {
        // Act
        var category = Category.Create(ValidName, ValidUserId, color: null);

        // Assert
        category.Color.Should().Be("#808080"); // gris por defecto
    }

    [Fact]
    public void Create_WithEmptyColor_ShouldUseDefaultColor()
    {
        // Act
        var category = Category.Create(ValidName, ValidUserId, color: "");

        // Assert
        category.Color.Should().Be("#808080");
    }

    [Theory]
    [InlineData("#FFF")]      // formato corto
    [InlineData("#fff")]      // formato corto minúsculas
    [InlineData("#FF5733")]   // formato largo
    [InlineData("#ff5733")]   // formato largo minúsculas
    public void Create_WithValidColorFormats_ShouldSucceed(string color)
    {
        // Act
        var action = () => CreateValidCategory(color: color);

        // Assert
        action.Should().NotThrow();
    }

    // ──────────────────────────────────────────────────────
    // CREACIÓN: CASOS DE ERROR
    // ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldThrowInvalidCategoryException(string name)
    {
        // Act
        var action = () => CreateValidCategory(name: name);

        // Assert
        action.Should()
            .Throw<InvalidCategoryException>()
            .WithMessage("*vacío*");
    }

    [Fact]
    public void Create_WithNameExceedingMaxLength_ShouldThrowInvalidCategoryException()
    {
        // Arrange: 51 caracteres, el máximo es 50
        var tooLongName = new string('a', 51);

        // Act
        var action = () => CreateValidCategory(name: tooLongName);

        // Assert
        action.Should()
            .Throw<InvalidCategoryException>()
            .WithMessage("*más de 50*");
    }

    [Fact]
    public void Create_WithNameAtMaxLength_ShouldSucceed()
    {
        // Arrange: 50 caracteres exactos
        var name = new string('a', 50);

        // Act
        var action = () => CreateValidCategory(name: name);

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("no-empieza-con-almohadilla")]
    [InlineData("#XYZ")]
    [InlineData("#12")]              // demasiado corto (necesita 3 o 6)
    [InlineData("#12345")]           // 5 chars, no válido
    [InlineData("#1234567")]         // 7 chars, no válido
    [InlineData("#GGGGGG")]          // G no es hexadecimal
    public void Create_WithInvalidColor_ShouldThrowInvalidCategoryException(string color)
    {
        // Act
        var action = () => CreateValidCategory(color: color);

        // Assert
        action.Should()
            .Throw<InvalidCategoryException>()
            .WithMessage("*hexadecimal*");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowInvalidCategoryException()
    {
        // Act
        var action = () => CreateValidCategory(userId: Guid.Empty);

        // Assert
        action.Should()
            .Throw<InvalidCategoryException>()
            .WithMessage("*propietario*");
    }

    // ──────────────────────────────────────────────────────
    // RENAME
    // ──────────────────────────────────────────────────────

    [Fact]
    public void Rename_WithValidName_ShouldChangeName()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.Rename("Personal");

        // Assert
        category.Name.Should().Be("Personal");
    }

    [Fact]
    public void Rename_WithSpaces_ShouldTrimName()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.Rename("   Personal   ");

        // Assert
        category.Name.Should().Be("Personal");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_WithEmptyName_ShouldThrowInvalidCategoryException(string newName)
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        var action = () => category.Rename(newName);

        // Assert
        action.Should().Throw<InvalidCategoryException>();
    }

    [Fact]
    public void Rename_WhenValidationFails_ShouldNotChangeOriginalName()
    {
        // Arrange
        var category = CreateValidCategory(name: "Original");

        // Act
        var action = () => category.Rename("");

        // Assert
        action.Should().Throw<InvalidCategoryException>();
        category.Name.Should().Be("Original");
    }

    // ──────────────────────────────────────────────────────
    // CHANGECOLOR
    // ──────────────────────────────────────────────────────

    [Fact]
    public void ChangeColor_WithValidColor_ShouldChangeColor()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.ChangeColor("#FF0000");

        // Assert
        category.Color.Should().Be("#FF0000");
    }

    [Fact]
    public void ChangeColor_WithLowercase_ShouldNormalizeToUppercase()
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        category.ChangeColor("#ff0000");

        // Assert
        category.Color.Should().Be("#FF0000");
    }

    [Theory]
    [InlineData("no-es-color")]
    [InlineData("#XYZ")]
    [InlineData("#12345")]
    public void ChangeColor_WithInvalidFormat_ShouldThrowInvalidCategoryException(string color)
    {
        // Arrange
        var category = CreateValidCategory();

        // Act
        var action = () => category.ChangeColor(color);

        // Assert
        action.Should().Throw<InvalidCategoryException>();
    }

    [Fact]
    public void ChangeColor_WhenValidationFails_ShouldNotChangeOriginalColor()
    {
        // Arrange
        var category = CreateValidCategory(color: "#FFFFFF");

        // Act
        var action = () => category.ChangeColor("invalido");

        // Assert
        action.Should().Throw<InvalidCategoryException>();
        category.Color.Should().Be("#FFFFFF");
    }
}