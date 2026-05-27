using System.Text.RegularExpressions;
using GestorTareas.Domain.Common;
using GestorTareas.Domain.Exceptions;

namespace GestorTareas.Domain.Entities;

public sealed class Category : Entity
{
    private const int MaxNameLength = 50;
    private static readonly Regex HexColorRegex = new(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$",RegexOptions.Compiled);
    private const string DefaultColor = "#808080";
 
    public string Name { get; private set; }
    public string Color { get; private set; }  
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Category(Guid id, string name, string color, Guid userId, DateTime createdAt) : base(id)
    {
        Name = name;
        Color = color;
        UserId = userId;
        CreatedAt = createdAt;
    }
    private Category() : base()
    {
        Name = null!;
        Color = null!;
    }
    public static Category Create(string name, Guid userId, string? color = null)
    {
        ValidateName(name);
        ValidateUserId(userId);

        var finalColor = string.IsNullOrWhiteSpace(color) ? DefaultColor : color;
        ValidateColor(finalColor);

        return new Category(
            id: Guid.NewGuid(),
            name: name.Trim(),
            color: finalColor.ToUpperInvariant(),
            userId: userId,
            createdAt: DateTime.UtcNow);
    }

    public void Rename(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    public void ChangeColor(string newColor)
    {
        ValidateColor(newColor);
        Color = newColor.ToUpperInvariant();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw InvalidCategoryException.EmptyName();

        if (name.Trim().Length > MaxNameLength)
            throw InvalidCategoryException.NameTooLong(MaxNameLength);
    }

    private static void ValidateColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color) || !HexColorRegex.IsMatch(color))
            throw InvalidCategoryException.InvalidColor(color);
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw InvalidCategoryException.MissingOwner();
    }
}