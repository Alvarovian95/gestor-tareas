using System.Text.RegularExpressions;
using GestorTareas.Domain.Exceptions;

namespace GestorTareas.Domain.ValueObjects;
public sealed record Email
{
    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",RegexOptions.Compiled);
    private const int MaxLength = 254;
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw InvalidEmailException.Empty();

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            throw InvalidEmailException.InvalidFormat(value);

        if (!EmailRegex.IsMatch(normalized))
            throw InvalidEmailException.InvalidFormat(value);

        return new Email(normalized);
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
}