namespace IglesiaNet.Domain.Churches;

public interface IChurchRepository
{
    Task<Church?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Church>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Church church, CancellationToken ct = default);
    Task UpdateAsync(Church church, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
