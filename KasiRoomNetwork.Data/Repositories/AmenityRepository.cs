using KasiRoomNetwork.Common.Models;
using KasiRoomNetwork.Common.ViewModel.Properties;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly ApplicationDbContext _db;

        public AmenityRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AmenityModel>> GetAllAmenities() =>
            await _db.Amenities.AsNoTracking().Where(a => a.IsActive)
                .OrderBy(a => a.Category).ThenBy(a => a.Name)
                .Select(a => new AmenityModel
                {
                    AmenityId = a.AmenityId,
                    Name = a.Name,
                    Category = a.Category!,
                    Icon = a.Icon
                }).ToListAsync();

        public async Task AddPropertyAmenity(int propertyId, int amenityId, string landlordUserId)
        {
            if (!await OwnsProperty(propertyId, landlordUserId))
                return;
            if (!await _db.PropertyAmenities.AnyAsync(pa => pa.PropertyId == propertyId && pa.AmenityId == amenityId))
                _db.PropertyAmenities.Add(new Domain.Models.PropertyAmenity { PropertyId = propertyId, AmenityId = amenityId });
            await _db.SaveChangesAsync();
        }

        public async Task RemovePropertyAmenity(int propertyId, int amenityId)
        {
            var links = await _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId && pa.AmenityId == amenityId).ToListAsync();
            _db.PropertyAmenities.RemoveRange(links);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AmenityModel>> GetAmenitiesByPropertyId(int propertyId) =>
            await _db.PropertyAmenities.AsNoTracking().Where(pa => pa.PropertyId == propertyId && pa.Amenity.IsActive)
                .OrderBy(pa => pa.Amenity.Category).ThenBy(pa => pa.Amenity.Name)
                .Select(pa => new AmenityModel
                {
                    AmenityId = pa.Amenity.AmenityId,
                    Name = pa.Amenity.Name,
                    Category = pa.Amenity.Category!,
                    Icon = pa.Amenity.Icon
                }).ToListAsync();

        public async Task<EditPropertyAmenitiesViewModel?> GetPropertyAmenitiesForEditAsync(int propertyId, string landlordId)
        {
            if (!await OwnsProperty(propertyId, landlordId))
                return null;
            var selectedIds = await _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId)
                .Select(pa => pa.AmenityId).ToListAsync();
            var amenities = await _db.Amenities.AsNoTracking().Where(a => a.IsActive)
                .OrderBy(a => a.Category).ThenBy(a => a.Name)
                .Select(a => new AmenitySelectionViewModel
                {
                    AmenityId = a.AmenityId,
                    Name = a.Name,
                    Category = a.Category!,
                    Icon = a.Icon,
                    IsSelected = selectedIds.Contains(a.AmenityId)
                }).ToListAsync();
            return new EditPropertyAmenitiesViewModel { PropertyId = propertyId, Amenities = amenities };
        }

        public async Task UpdatePropertyAmenitiesAsync(int propertyId, List<int> amenityIds, string landlordId)
        {
            if (!await OwnsProperty(propertyId, landlordId))
                return;
            await using var transaction = await _db.Database.BeginTransactionAsync();
            var links = await _db.PropertyAmenities.Where(pa => pa.PropertyId == propertyId).ToListAsync();
            _db.PropertyAmenities.RemoveRange(links);
            var validIds = await _db.Amenities.Where(a => a.IsActive && amenityIds.Contains(a.AmenityId))
                .Select(a => a.AmenityId).Distinct().ToListAsync();
            foreach (var amenityId in validIds)
                _db.PropertyAmenities.Add(new Domain.Models.PropertyAmenity { PropertyId = propertyId, AmenityId = amenityId });
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private Task<bool> OwnsProperty(int propertyId, string landlordId) =>
            _db.Properties.AnyAsync(p => p.PropertyId == propertyId && p.LandlordId == landlordId);
    }
}
