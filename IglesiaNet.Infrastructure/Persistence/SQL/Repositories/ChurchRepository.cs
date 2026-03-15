using IglesiaNet.Domain.Churches;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.Infrastructure.Persistence.SQL.Repositories;

public class ChurchRepository : IChurchRepository
{
    private readonly AppDbContext _db;
    public ChurchRepository(AppDbContext db) => _db = db;

    public Task<Church?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Churches.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Church>> GetAllActiveAsync(CancellationToken ct = default) =>
        _db.Churches.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(ct);

    public async Task AddAsync(Church church, CancellationToken ct = default) =>
        await _db.Churches.AddAsync(church, ct);

    public Task UpdateAsync(Church church, CancellationToken ct = default)
    {
        _db.Churches.Update(church);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var church = await GetByIdAsync(id, ct);
        if (church is not null) church.Deactivate();
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
