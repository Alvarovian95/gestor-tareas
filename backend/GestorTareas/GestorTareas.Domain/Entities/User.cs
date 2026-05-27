using GestorTareas.Domain.Common;
using GestorTareas.Domain.Exceptions;
using GestorTareas.Domain.ValueObjects;

namespace GestorTareas.Domain.Entities;
public sealed class User : Entity
{
    private const int MaxNameLength = 100;
    public Email Email { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private User(Guid id, Email email, string name, string passwordHash, DateTime createdAt) : base(id)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    private User() : base()
    {
        Email = null!;
        Name = null!;
        PasswordHash = null!;
    }

    public static User Create(string email, string name, string passwordHash)
    {
        ValidateName(name);
        ValidatePasswordHash(passwordHash);

        var emailVo = Email.Create(email);
        var trimmedName = name.Trim();

        return new User(
            id: Guid.NewGuid(),
            email: emailVo,
            name: trimmedName,
            passwordHash: passwordHash,
            createdAt: DateTime.UtcNow);
    }


    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    public void ChangePassword(string newPasswordHash)
    {
        ValidatePasswordHash(newPasswordHash);
        PasswordHash = newPasswordHash;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw InvalidUserException.EmptyName();

        if (name.Trim().Length > MaxNameLength)
            throw InvalidUserException.NameTooLong(MaxNameLength);
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw InvalidUserException.InvalidPasswordHash();
    }
}