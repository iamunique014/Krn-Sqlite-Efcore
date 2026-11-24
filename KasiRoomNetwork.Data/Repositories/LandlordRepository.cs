using KasiRoomNetwork.Common.ViewModel.Landlord;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class LandlordRepository : ILandlordRepository
    {
        private readonly ApplicationDbContext _db;

        public LandlordRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<LandlordListingViewModel>> GetAllLandlordListings(string landlordId) =>
            await _db.Listings.AsNoTracking()
                .Where(l => l.LandlordUserId == landlordId && l.IsActive)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LandlordListingViewModel
                {
                    ListingId = l.ListingId,
                    Title = l.Title,
                    Price = l.Price,
                    IsVerified = l.IsVerified,
                    AvailableUnits = l.AvailableUnits,
                    IsAvailable = l.IsAvailable,
                    Province = l.Property.Address.Province,
                    City = l.Property.Address.City ?? string.Empty,
                    Suburb = l.Property.Address.Suburb ?? string.Empty,
                    PrimaryPhoto = l.Photos.Where(p => p.IsPrimary && p.IsActive)
                        .OrderBy(p => p.CreatedAt).Select(p => p.PhotoPath).FirstOrDefault()
                }).ToListAsync();

        public async Task<IEnumerable<PropertyCardViewModel>> GetAllPropertiesByLandlord(string landlordId) =>
            await _db.Properties.AsNoTracking()
                .Where(p => p.LandlordId == landlordId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PropertyCardViewModel
                {
                    PropertyId = p.PropertyId,
                    PropertyName = p.PropertyName ?? string.Empty,
                    PropertyType = p.PropertyType ?? string.Empty,
                    TotalRooms = p.TotalRooms,
                    Street = p.Address.Street ?? string.Empty,
                    Province = p.Address.Province,
                    City = p.Address.City ?? string.Empty,
                    Suburb = p.Address.Suburb ?? string.Empty,
                    PrimaryPhoto = p.Photos.Where(photo => photo.IsPrimary && photo.IsActive)
                        .OrderBy(photo => photo.CreatedAt).Select(photo => photo.PhotoPath).FirstOrDefault() ?? string.Empty,
                    TotalListings = p.Listings.Count(listing => listing.IsActive),
                    CreatedAt = p.CreatedAt
                }).ToListAsync();
    }
}
