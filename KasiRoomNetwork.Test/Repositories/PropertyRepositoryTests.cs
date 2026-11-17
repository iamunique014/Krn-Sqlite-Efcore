using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KasiRoomNetwork.Test.Repositories;

public class PropertyRepositoryTests
{
    [Fact]
    public async Task DeletePropertyAsync_SoftDeletesPropertyOwnedByLandlord()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var db = new ApplicationDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var user = new ApplicationUser { Id = "landlord-123", UserName = "landlord", NormalizedUserName = "LANDLORD" };
        db.Users.Add(user);
        var address = new Address { Province = "North West", IsActive = true };
        var property = new Property { LandlordId = user.Id, PropertyName = "Test property", PropertyType = "Apartment", IsActive = true, Address = address };
        db.Properties.Add(property);
        await db.SaveChangesAsync();

        var repository = new PropertyRepository(db);
        await repository.DeletePropertyAsync(property.PropertyId, user.Id);

        var deleted = await db.Properties.AsNoTracking().SingleAsync(p => p.PropertyId == property.PropertyId);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.DeletedAt);
    }

    [Fact]
    public async Task DeletePropertyAsync_DoesNotDeletePropertyOwnedByAnotherLandlord()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;
        await using var db = new ApplicationDbContext(options);
        await db.Database.EnsureCreatedAsync();
        db.Users.AddRange(
            new ApplicationUser { Id = "owner", UserName = "owner", NormalizedUserName = "OWNER" },
            new ApplicationUser { Id = "other", UserName = "other", NormalizedUserName = "OTHER" });
        var property = new Property
        {
            LandlordId = "owner", PropertyName = "Test property", PropertyType = "Apartment", IsActive = true,
            Address = new Address { Province = "North West", IsActive = true }
        };
        db.Properties.Add(property);
        await db.SaveChangesAsync();

        await new PropertyRepository(db).DeletePropertyAsync(property.PropertyId, "other");

        var unchanged = await db.Properties.AsNoTracking().SingleAsync(p => p.PropertyId == property.PropertyId);
        Assert.True(unchanged.IsActive);
        Assert.Null(unchanged.DeletedAt);
    }
}
