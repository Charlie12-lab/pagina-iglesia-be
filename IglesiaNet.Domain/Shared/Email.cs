using System.Text.RegularExpressions;

namespace IglesiaNet.Domain.Shared;

public sealed class Email : ValueObject
{
    private static readonly Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El correo electrónico es requerido");
        if (!Pattern.IsMatch(value))
            throw new DomainException($"'{value}' no es un correo electrónico válido");
        return new Email(value.ToLowerInvariant().Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
    public static implicit operator string(Email email) => email.Value;
}
