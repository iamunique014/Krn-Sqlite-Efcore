using KasiRoomNetwork.Common.ViewModel.Home;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<FeaturedPropertyCardViewModel>> GetFeaturedPropertiesAsync() =>
            await _db.Properties.AsNoTracking()
                .Where(p => p.IsActive && p.IsVerified)
                .Select(p => new FeaturedPropertyCardViewModel
                {
                    PropertyId = p.PropertyId,
                    PropertyName = p.PropertyName ?? string.Empty,
                    PropertyType = p.PropertyType ?? string.Empty,
                    City = p.Address.City ?? string.Empty,
                    Suburb = p.Address.Suburb ?? string.Empty,
                    PrimaryPhoto = p.Photos.Where(photo => photo.IsActive && photo.IsPrimary)
                        .OrderBy(photo => photo.CreatedAt).Select(photo => photo.PhotoPath).FirstOrDefault(),
                    AvailableRooms = p.Listings.Where(listing => listing.IsActive && listing.IsVerified && listing.IsAvailable)
                        .Sum(listing => (int?)listing.AvailableUnits) ?? 0,
                    StartingPrice = p.Listings.Where(listing => listing.IsActive && listing.IsVerified && listing.IsAvailable)
                        .Select(listing => (decimal?)listing.Price).Min() ?? 0m
                })
                .Where(p => p.AvailableRooms > 0)
                .OrderBy(p => p.StartingPrice)
                .Take(6)
                .ToListAsync();
    }
}
