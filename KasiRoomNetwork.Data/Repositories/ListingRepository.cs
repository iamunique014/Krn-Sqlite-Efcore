using KasiRoomNetwork.Common.Models;
using KasiRoomNetwork.Common.ViewModel.Listings;
using KasiRoomNetwork.Common.ViewModel.Properties;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly ApplicationDbContext _context;

        public ListingRepository(ApplicationDbContext context) => _context = context;

        public async Task<int> CreateListing(CreateListingViewModel model, string landlordUserId)
        {
            var ownsProperty = await _context.Properties.AnyAsync(p =>
                p.PropertyId == model.PropertyId && p.LandlordId == landlordUserId && p.IsActive);
            if (!ownsProperty)
            {
                throw new UnauthorizedAccessException("The landlord does not own the selected property.");
            }

            var listing = new Listing
            {
                LandlordUserId = landlordUserId,
                PropertyId = model.PropertyId,
                Title = model.Title,
                Description = model.Description,
                AvailableUnits = model.AvailableUnits,
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                Status = "Active",
                IsActive = true
            };
            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
            return listing.ListingId;
        }

        public async Task DeleteListing(int listingId)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(x => x.ListingId == listingId && x.IsActive);
            if (listing == null) return;
            listing.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddListingPhoto(int listingId, string photoPath, bool isPrimary, string landlordUserId)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(x => x.ListingId == listingId && x.LandlordUserId == landlordUserId && x.IsActive);
            if (listing == null) return false;
            if (isPrimary)
            {
                var primaryPhotos = await _context.ListingPhotos.Where(x => x.ListingId == listingId && x.IsActive && x.IsPrimary).ToListAsync();
                primaryPhotos.ForEach(x => x.IsPrimary = false);
            }
            _context.ListingPhotos.Add(new ListingPhoto { ListingId = listingId, PhotoPath = photoPath, IsPrimary = isPrimary, IsActive = true });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ListingDetailsViewModel?> GetListingById(int listingId) => await GetListingDetailsById(listingId);

        public async Task<ListingDetailsViewModel?> GetListingDetailsById(int listingId)
        {
            var listing = await _context.Listings.AsNoTracking().Include(x => x.Property).ThenInclude(x => x.Address)
                .Include(x => x.Property).ThenInclude(x => x.PropertyAmenities).ThenInclude(x => x.Amenity)
                .Include(x => x.Property).ThenInclude(x => x.Landlord).ThenInclude(x => x!.LandlordProfile)
                .Include(x => x.Property).ThenInclude(x => x.Landlord).ThenInclude(x => x!.UserProfile)
                .Include(x => x.Photos).FirstOrDefaultAsync(x => x.ListingId == listingId && x.IsActive);
            if (listing == null) return null;
            var landlord = listing.Property.Landlord;
            return new ListingDetailsViewModel
            {
                ListingId = listing.ListingId, Title = listing.Title, Description = listing.Description ?? string.Empty,
                Price = listing.Price, IsAvailable = listing.IsAvailable, AvailableUnits = listing.AvailableUnits,
                IsVerified = listing.IsVerified, VerificationNotes = listing.VerificationNotes ?? string.Empty,
                VerifiedOn = listing.VerifiedOn ?? default, CreatedAt = listing.CreatedAt,
                PropertyId = listing.PropertyId, PropertyName = listing.Property.PropertyName ?? string.Empty,
                PropertyType = listing.Property.PropertyType ?? string.Empty, TotalRooms = listing.Property.TotalRooms ?? 0,
                PropertyVerified = listing.Property.IsVerified, Province = listing.Property.Address.Province,
                City = listing.Property.Address.City ?? string.Empty, Suburb = listing.Property.Address.Suburb ?? string.Empty,
                Street = listing.Property.Address.Street, LandlordUserId = listing.LandlordUserId ?? string.Empty,
                FullName = landlord?.UserProfile?.FullName ?? string.Join(" ", new[] { landlord?.FirstName, landlord?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                PhoneNumber = landlord?.PhoneNumber ?? string.Empty, WhatsAppNumber = landlord?.LandlordProfile?.WhatsAppNumber,
                LandlordBio = landlord?.LandlordProfile?.Bio ?? string.Empty, LandlordVerified = landlord?.LandlordProfile?.IsVerified ?? false,
                Photos = listing.Photos.Where(p => p.IsActive).OrderByDescending(p => p.IsPrimary).ThenBy(p => p.PhotoId)
                    .Select(p => new ListingPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToList(),
                Amenities = listing.Property.PropertyAmenities.Where(a => a.Amenity.IsActive).Select(a => new AmenityModel
                { AmenityId = a.Amenity.AmenityId, Name = a.Amenity.Name, Category = a.Amenity.Category ?? string.Empty, Icon = a.Amenity.Icon }).ToList()
            };
        }

        public async Task<List<ListingPhotoViewModel>> GetListingPhotos(int listingId) => await _context.ListingPhotos.AsNoTracking()
            .Where(p => p.ListingId == listingId && p.IsActive).OrderByDescending(p => p.IsPrimary).ThenBy(p => p.PhotoId)
            .Select(p => new ListingPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToListAsync();

        public async Task<List<ListingSearchResultViewModel>> SearchListings(ListingSearchViewModel searchModel)
        {
            var query = _context.Listings.AsNoTracking().Where(l => l.IsActive && l.IsAvailable && l.IsVerified && l.Property.IsActive && l.Property.Address.IsActive != false);
            if (!string.IsNullOrWhiteSpace(searchModel.Province)) query = query.Where(l => l.Property.Address.Province == searchModel.Province);
            if (!string.IsNullOrWhiteSpace(searchModel.City)) query = query.Where(l => l.Property.Address.City == searchModel.City);
            if (!string.IsNullOrWhiteSpace(searchModel.Suburb)) query = query.Where(l => l.Property.Address.Suburb == searchModel.Suburb);
            if (searchModel.MinPrice.HasValue) query = query.Where(l => l.Price >= searchModel.MinPrice.Value);
            if (searchModel.MaxPrice.HasValue) query = query.Where(l => l.Price <= searchModel.MaxPrice.Value);
            return await query.OrderByDescending(l => l.CreatedAt).Select(l => new ListingSearchResultViewModel
            {
                ListingId = l.ListingId, Title = l.Title, Price = l.Price, CreatedAt = l.CreatedAt,
                Province = l.Property.Address.Province, City = l.Property.Address.City ?? string.Empty,
                Suburb = l.Property.Address.Suburb ?? string.Empty,
                PrimaryPhoto = l.Photos.Where(p => p.IsActive && p.IsPrimary).Select(p => p.PhotoPath).FirstOrDefault()
            }).ToListAsync();
        }

        public Task<int> GetListingPhotoCount(int listingId) => _context.ListingPhotos.CountAsync(p => p.ListingId == listingId && p.IsActive);

        public async Task<EditListingViewModel?> GetListingForEdit(int listingId, string landlordUserId)
        {
            var listing = await _context.Listings.AsNoTracking().Include(l => l.Property).ThenInclude(p => p.Address)
                .FirstOrDefaultAsync(l => l.ListingId == listingId && l.LandlordUserId == landlordUserId && l.IsActive);
            return listing == null ? null : new EditListingViewModel
            {
                ListingId = listing.ListingId, PropertyId = listing.PropertyId, Title = listing.Title,
                Description = listing.Description ?? string.Empty, Price = listing.Price, AvailableUnits = listing.AvailableUnits,
                IsAvailable = listing.IsAvailable, PropertyName = listing.Property.PropertyName,
                City = listing.Property.Address.City, Suburb = listing.Property.Address.Suburb
            };
        }

        public async Task<bool> UpdateListing(EditListingViewModel model, string landlordUserId)
        {
            var listing = await _context.Listings.FirstOrDefaultAsync(l =>
                l.ListingId == model.ListingId && l.LandlordUserId == landlordUserId && l.IsActive &&
                l.PropertyId == model.PropertyId && l.Property.LandlordId == landlordUserId && l.Property.IsActive);
            if (listing == null) return false;
            listing.Title = model.Title; listing.Price = model.Price; listing.AvailableUnits = model.AvailableUnits;
            listing.Description = model.Description; listing.IsAvailable = model.IsAvailable; listing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(); return true;
        }

        public async Task<bool> DeleteListingPhoto(int photoId, int listingId, string landlordUserId)
        {
            var photo = await _context.ListingPhotos.Include(p => p.Listing).FirstOrDefaultAsync(p => p.PhotoId == photoId && p.ListingId == listingId && p.IsActive && p.Listing.LandlordUserId == landlordUserId);
            if (photo == null) return false;
            photo.IsActive = false; photo.IsPrimary = false;
            await _context.SaveChangesAsync(); return true;
        }

        public async Task<bool> SetPrimaryListingPhoto(int listingId, int photoId, string landlordUserId)
        {
            var target = await _context.ListingPhotos.Include(p => p.Listing).FirstOrDefaultAsync(p => p.PhotoId == photoId && p.ListingId == listingId && p.IsActive && p.Listing.LandlordUserId == landlordUserId);
            if (target == null) return false;
            var photos = await _context.ListingPhotos.Where(p => p.ListingId == listingId && p.IsActive).ToListAsync();
            photos.ForEach(p => p.IsPrimary = p.PhotoId == photoId);
            await _context.SaveChangesAsync(); return true;
        }
    }
}
