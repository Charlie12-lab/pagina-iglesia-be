using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Domain.Churches;

public class Church : Entity<int>
{
    public string Name { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    // Nuevos campos
    public string? Denomination { get; private set; }
    public string Status { get; private set; }   // "Active" | "Pending" | "Inactive"
    public string? PastorName { get; private set; }
    public string? PastorEmail { get; private set; }

    // Para EF Core
    private Church() : base() { Name = string.Empty; Status = "Active"; }

    private Church(string name, string? address, string? city, string? phone,
        string? email, string? description, string? logoUrl, string? websiteUrl,
        string? denomination, string status, string? pastorName, string? pastorEmail)
        : base()
    {
        Name = name;
        Address = address;
        City = city;
        Phone = phone;
        Email = email;
        Description = description;
        LogoUrl = logoUrl;
        WebsiteUrl = websiteUrl;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        Denomination = denomination;
        Status = status;
        PastorName = pastorName;
        PastorEmail = pastorEmail;
    }

    public static Church Create(string name, string? address, string? city, string? phone,
        string? email, string? description, string? logoUrl, string? websiteUrl,
        string? denomination = null, string? status = null,
        string? pastorName = null, string? pastorEmail = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la iglesia es requerido");
        if (name.Length > 200)
            throw new DomainException("El nombre no puede superar los 200 caracteres");
        if (email is not null)
            Shared.Email.Create(email); // Valida formato

        return new Church(name.Trim(), address?.Trim(), city?.Trim(),
            phone?.Trim(), email?.Trim().ToLowerInvariant(),
            description?.Trim(), logoUrl?.Trim(), websiteUrl?.Trim(),
            denomination?.Trim(), status ?? "Active",
            pastorName?.Trim(), pastorEmail?.Trim().ToLowerInvariant());
    }

    public void Update(string? name, string? address, string? city, string? phone,
        string? email, string? description, string? logoUrl, string? websiteUrl, bool? isActive,
        string? denomination = null, string? status = null,
        string? pastorName = null, string? pastorEmail = null)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("El nombre no puede estar vacío");
            Name = name.Trim();
        }
        if (email is not null) Shared.Email.Create(email);

        if (address is not null) Address = address.Trim();
        if (city is not null) City = city.Trim();
        if (phone is not null) Phone = phone.Trim();
        if (email is not null) Email = email.Trim().ToLowerInvariant();
        if (description is not null) Description = description.Trim();
        if (logoUrl is not null) LogoUrl = logoUrl.Trim();
        if (websiteUrl is not null) WebsiteUrl = websiteUrl.Trim();
        if (isActive.HasValue) IsActive = isActive.Value;
        if (denomination is not null) Denomination = denomination.Trim();
        if (status is not null) Status = status;
        if (pastorName is not null) PastorName = pastorName.Trim();
        if (pastorEmail is not null) PastorEmail = pastorEmail.Trim().ToLowerInvariant();
    }

    public void Deactivate() => IsActive = false;
}
