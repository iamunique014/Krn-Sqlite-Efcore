using KasiRoomNetwork.Common.Models;
using KasiRoomNetwork.Common.Models.Enums;
using KasiRoomNetwork.Common.ViewModel.Properties;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context) => _context = context;

        public async Task<int> CreateProperty(CreatePropertyViewModel model, string landlordUserId)
        {
            var address = new Address
            {
                Street = model.Street,
                Province = model.Province,
                City = model.City,
                Suburb = model.Suburb,
                IsActive = true
            };
            var property = new Property
            {
                LandlordId = landlordUserId,
                PropertyType = model.PropertyType?.ToString() ?? string.Empty,
                TotalRooms = model.TotalRooms,
                PropertyName = model.PropertyName,
                IsActive = true,
                Address = address
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property.PropertyId;
        }

        public async Task<EditPropertyViewModel?> GetPropertyForEditAsync(int propertyId, string landlordId)
        {
            var property = await _context.Properties.AsNoTracking().Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.LandlordId == landlordId && p.IsActive);
            if (property == null) return null;
            return new EditPropertyViewModel
            {
                PropertyId = property.PropertyId,
                PropertyName = property.PropertyName ?? string.Empty,
                PropertyType = Enum.TryParse<PropertyType>(property.PropertyType, out var type) ? type : null,
                TotalRooms = property.TotalRooms,
                Province = property.Address.Province,
                City = property.Address.City ?? string.Empty,
                Suburb = property.Address.Suburb,
                Street = property.Address.Street
            };
        }

        public async Task UpdatePropertyAsync(EditPropertyViewModel model, string landlordId)
        {
            var property = await _context.Properties.Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.PropertyId == model.PropertyId && p.LandlordId == landlordId && p.IsActive);
            if (property == null) return;
            property.PropertyName = model.PropertyName;
            property.PropertyType = model.PropertyType?.ToString() ?? string.Empty;
            property.TotalRooms = model.TotalRooms;
            property.Address.Province = model.Province; property.Address.City = model.City;
            property.Address.Suburb = model.Suburb; property.Address.Street = model.Street;
            property.Address.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePropertyAsync(int propertyId, string landlordId)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.LandlordId == landlordId && p.IsActive);
            if (property == null) return;
            property.IsActive = false; property.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<List<LandlordPropertyViewModel>> GetPropertiesByUser(string landlordUserId) => await _context.Properties.AsNoTracking()
            .Where(p => p.LandlordId == landlordUserId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new LandlordPropertyViewModel
            {
                PropertyId = p.PropertyId,
                PropertyName = p.PropertyName ?? string.Empty,
                PropertyType = p.PropertyType ?? string.Empty,
                TotalRooms = p.TotalRooms ?? 0,
                CreatedAt = p.CreatedAt,
                Street = p.Address.Street ?? string.Empty,
                Suburb = p.Address.Suburb ?? string.Empty,
                City = p.Address.City ?? string.Empty,
                Province = p.Address.Province
            }).ToListAsync();

        public async Task<bool> AddPropertyPhoto(int propertyId, string dbPath, bool isPrimary, string landlordUserId)
        {
            var property = await _context.Properties.FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.LandlordId == landlordUserId && p.IsActive);
            if (property == null) return false;
            if (isPrimary)
            {
                var primaryPhotos = await _context.PropertyPhotos.Where(p => p.PropertyId == propertyId && p.IsActive && p.IsPrimary).ToListAsync();
                primaryPhotos.ForEach(p => p.IsPrimary = false);
            }
            _context.PropertyPhotos.Add(new PropertyPhoto { PropertyId = propertyId, PhotoPath = dbPath, IsPrimary = isPrimary, IsActive = true });
            await _context.SaveChangesAsync(); return true;
        }

        public async Task<PropertyDetailsViewModel?> GetPropertyById(int propertyId)
        {
            var property = await _context.Properties.AsNoTracking().Include(p => p.Address)
                .Include(p => p.Landlord).ThenInclude(u => u!.UserProfile)
                .Include(p => p.Landlord).ThenInclude(u => u!.LandlordProfile)
                .Include(p => p.Photos)
                .Include(p => p.Listings).ThenInclude(l => l.Photos)
                .Include(p => p.PropertyAmenities).ThenInclude(a => a.Amenity)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.IsActive);
            if (property == null) return null;
            var landlord = property.Landlord;
            return new PropertyDetailsViewModel
            {
                PropertyId = property.PropertyId,
                PropertyName = property.PropertyName ?? string.Empty,
                PropertyType = property.PropertyType ?? string.Empty,
                TotalRooms = property.TotalRooms ?? 0,
                IsVerified = property.IsVerified,
                VerificationNotes = property.VerificationNotes ?? string.Empty,
                VerifiedOn = property.VerifiedOn ?? default,
                CreatedAt = property.CreatedAt,

                //Address
                Province = property.Address.Province,
                City = property.Address.City ?? string.Empty,
                Suburb = property.Address.Suburb ?? string.Empty,
                Street = property.Address.Street,

                //Landlord
                FullName = landlord?.UserProfile?.FullName ?? string.Join(" ", new[] { landlord?.FirstName, landlord?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PhoneNumber = landlord?.PhoneNumber ?? string.Empty,
                LandlordUserId = property.LandlordId ?? string.Empty,

                //Photos
                Photos = property.Photos.Where(p => p.IsActive).OrderByDescending(p => p.IsPrimary).ThenBy(p => p.PhotoId)
                    .Select(p => new PropertyPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToList(),

                //Room listings
                Listings = property.Listings.Where(l => l.IsActive).OrderByDescending(l => l.CreatedAt)
                    .Select(l => new PropertyListingViewModel
                    {
                        ListingId = l.ListingId,
                        Title = l.Title,
                        Price = l.Price,
                        IsAvailable = l.IsAvailable,
                        AvailableUnits = l.AvailableUnits,
                        IsVerified = l.IsVerified,
                        PrimaryPhoto = l.Photos.Where(p => p.IsActive && p.IsPrimary)
                            .OrderBy(p => p.CreatedAt)
                            .Select(p => p.PhotoPath)
                            .FirstOrDefault() ?? string.Empty
                    }).ToList(),

                //Amenities
                Amenities = property.PropertyAmenities.Where(a => a.Amenity.IsActive).Select(a => new AmenityModel
                { AmenityId = a.Amenity.AmenityId, Name = a.Amenity.Name, Category = a.Amenity.Category ?? string.Empty, Icon = a.Amenity.Icon }).ToList()
            };
        }

        public Task<int> GetPropertyPhotoCount(int propertyId) => _context.PropertyPhotos.CountAsync(p => p.PropertyId == propertyId && p.IsActive);

        public async Task<List<PropertyPhotoViewModel>> GetPropertyPhotos(int propertyId) => await _context.PropertyPhotos.AsNoTracking()
            .Where(p => p.PropertyId == propertyId && p.IsActive).OrderByDescending(p => p.IsPrimary).ThenBy(p => p.PhotoId)
            .Select(p => new PropertyPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToListAsync();

        public async Task DeletePropertyPhoto(int photoId, int propertyId, string landlordUserId)
        {
            var photo = await _context.PropertyPhotos.Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.PhotoId == photoId && p.PropertyId == propertyId && p.IsActive && p.Property.LandlordId == landlordUserId);
            if (photo == null) return;
            photo.IsActive = false; photo.IsPrimary = false;
            await _context.SaveChangesAsync();
        }

        public async Task SetPrimaryPropertyPhoto(int propertyId, int photoId, string landlordUserId)
        {
            var target = await _context.PropertyPhotos.Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.PhotoId == photoId && p.PropertyId == propertyId && p.IsActive && p.Property.LandlordId == landlordUserId);
            if (target == null) return;
            var photos = await _context.PropertyPhotos.Where(p => p.PropertyId == propertyId && p.IsActive).ToListAsync();
            photos.ForEach(p => p.IsPrimary = p.PhotoId == photoId);
            await _context.SaveChangesAsync();
        }
    }
}
