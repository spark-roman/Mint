using Mint.Database;
using Mint.Database.Entities.Users;

namespace Mint.Database.Seeding;

/// <summary>
/// Seeder for user entities
/// </summary>
public static class UsersSeeder
{
    /// <summary>
    /// Seed test users into database context
    /// </summary>
    /// <param name="context">Database context to seed</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            },
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Johnson",
                UserName = "bob.johnson",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 2,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie.brown",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            });
    }
}
