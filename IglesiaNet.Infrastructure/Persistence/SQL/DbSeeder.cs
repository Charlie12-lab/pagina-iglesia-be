using IglesiaNet.Application.Common;
using IglesiaNet.Domain.Churches;
using IglesiaNet.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IglesiaNet.Infrastructure.Persistence.SQL;

/// <summary>
/// Siembra datos iniciales de prueba si la base de datos está vacía.
/// Usuarios creados:
///   SuperAdmin  → admin@iglesianet.com     / Admin123!
///   ChurchAdmin → pastor@iglesiacristo.com / Pastor123!  (Iglesia Cristo Centro, Quito)
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher)
    {
        // ─── 1. Iglesia de prueba ─────────────────────────────────────────────
        if (!await db.Set<Church>().AnyAsync())
        {
            var church = Church.Create(
                name:        "Iglesia Cristo Centro",
                address:     "Av. Amazonas N23-45",
                city:        "Quito",
                phone:       "+593-2-555-0100",
                email:       "contacto@iglesiacristo.com",
                description: "Iglesia de prueba para desarrollo",
                logoUrl:     null,
                websiteUrl:  null
            );
            db.Set<Church>().Add(church);
            await db.SaveChangesAsync();
        }

        var churchId = await db.Set<Church>()
            .Where(c => c.IsActive)
            .Select(c => c.Id)
            .FirstAsync();

        // ─── 2. SuperAdmin ────────────────────────────────────────────────────
        if (!await db.Set<User>().AnyAsync(u => u.Role == UserRole.SuperAdmin))
        {
            var superAdmin = User.Create(
                username:     "Admin IglesiaNet",
                email:        "admin@iglesianet.com",
                passwordHash: hasher.Hash("Admin123!"),
                role:         UserRole.SuperAdmin,
                churchId:     null
            );
            db.Set<User>().Add(superAdmin);
        }

        // ─── 3. ChurchAdmin ───────────────────────────────────────────────────
        if (!await db.Set<User>().AnyAsync(u => u.Role == UserRole.ChurchAdmin))
        {
            var pastor = User.Create(
                username:     "Pastor Juan Torres",
                email:        "pastor@iglesiacristo.com",
                passwordHash: hasher.Hash("Pastor123!"),
                role:         UserRole.ChurchAdmin,
                churchId:     churchId
            );
            db.Set<User>().Add(pastor);
        }

        await db.SaveChangesAsync();
    }
}
