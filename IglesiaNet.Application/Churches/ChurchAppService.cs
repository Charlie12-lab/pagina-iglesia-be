using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Shared;

namespace IglesiaNet.Application.Churches;

public class ChurchAppService
{
    private readonly IChurchRepository _churches;

    public ChurchAppService(IChurchRepository churches) => _churches = churches;

    public async Task<List<ChurchDto>> GetAllAsync(CancellationToken ct = default)
    {
        var churches = await _churches.GetAllActiveAsync(ct);
        return churches.Select(ChurchDto.From).ToList();
    }

    public async Task<ChurchDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var church = await _churches.GetByIdAsync(id, ct);
        return church is null ? null : ChurchDto.From(church);
    }

    public async Task<ChurchDto> CreateAsync(CreateChurchRequest request, CancellationToken ct = default)
    {
        var church = Church.Create(
            request.Name, request.Address, request.City,
            request.Phone, request.Email, request.Description,
            request.LogoUrl, request.WebsiteUrl);

        await _churches.AddAsync(church, ct);
        await _churches.SaveChangesAsync(ct);
        return ChurchDto.From(church);
    }

    public async Task<ChurchDto?> UpdateAsync(int id, UpdateChurchRequest request, CancellationToken ct = default)
    {
        var church = await _churches.GetByIdAsync(id, ct);
        if (church is null) return null;

        church.Update(request.Name, request.Address, request.City,
            request.Phone, request.Email, request.Description,
            request.LogoUrl, request.WebsiteUrl, request.IsActive);

        await _churches.UpdateAsync(church, ct);
        await _churches.SaveChangesAsync(ct);
        return ChurchDto.From(church);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var church = await _churches.GetByIdAsync(id, ct);
        if (church is null) return false;

        church.Deactivate();
        await _churches.UpdateAsync(church, ct);
        await _churches.SaveChangesAsync(ct);
        return true;
    }
}
