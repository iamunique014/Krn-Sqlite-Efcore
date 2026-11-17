using KasiRoomNetwork.Common.ViewModel.Admin;
using KasiRoomNetwork.Common.ViewModel.Listings;
using KasiRoomNetwork.Common.ViewModel.Properties;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _db;

        public AdminRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<UnverifiedListingViewModel>> GetUnverifiedListingsAsync() =>
            await _db.Listings.AsNoTracking().Where(l => !l.IsVerified && l.IsActive)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new UnverifiedListingViewModel
                {
                    ListingId = l.ListingId, Title = l.Title, Price = l.Price, IsVerified = l.IsVerified,
                    VerifiedOn = l.VerifiedOn ?? default, PropertyName = l.Property.PropertyName ?? string.Empty,
                    Province = l.Property.Address.Province, City = l.Property.Address.City ?? string.Empty,
                    Suburb = l.Property.Address.Suburb ?? string.Empty, CreatedAt = l.CreatedAt
                }).ToListAsync();

        public async Task<IEnumerable<UnverifiedPropertyViewModel>> GetUnverifiedPropertiesAsync() =>
            await _db.Properties.AsNoTracking().Where(p => !p.IsVerified && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new UnverifiedPropertyViewModel
                {
                    PropertyId = p.PropertyId, PropertyName = p.PropertyName ?? string.Empty,
                    PropertyType = p.PropertyType ?? string.Empty, TotalRooms = p.TotalRooms ?? 0,
                    CreatedAt = p.CreatedAt, Province = p.Address.Province,
                    City = p.Address.City ?? string.Empty, Suburb = p.Address.Suburb ?? string.Empty,
                    LandlordUserId = p.LandlordId ?? string.Empty,
                    FullName = p.Landlord != null && p.Landlord.UserProfile != null ? p.Landlord.UserProfile.FullName : string.Empty,
                    IsVerified = p.Landlord != null && p.Landlord.LandlordProfile != null && p.Landlord.LandlordProfile.IsVerified,
                    VerifiedAt = p.VerifiedOn ?? default
                }).ToListAsync();

        public async Task<IEnumerable<UnverifiedLandlordViewModel>> GetUnverifiedLandlordsAsync()
        {
            var profiles = await _db.LandlordProfiles.AsNoTracking().Where(p => !p.IsVerified && p.User.UserProfile != null)
                .OrderByDescending(p => p.User.UserProfile!.CreatedAt ?? p.User.CreatedAt)
                .Select(p => new UnverifiedLandlordViewModel
                {
                    LandlordProfileId = p.LandlordProfileId, LandlordUserId = p.UserId,
                    FullName = p.User.UserProfile!.FullName, PhoneNumber = p.User.UserProfile.PhoneNumber ?? string.Empty,
                    City = p.User.UserProfile.City ?? string.Empty, Province = p.User.UserProfile.Province ?? string.Empty,
                    Bio = p.Bio ?? string.Empty, IsVerified = p.IsVerified,
                    VerificationStatus = p.VerifiedBy != null && p.VerifiedAt == null,
                    VerifiedAt = p.VerifiedAt ?? default
                }).ToListAsync();
            foreach (var item in profiles)
                item.MyProperty = await _db.Properties.CountAsync(p => p.LandlordId == item.LandlordUserId && p.IsActive);
            return profiles;
        }

        public async Task<AdminListingReviewViewModel> GetListingForVerificationAsync(int listingId)
        {
            var l = await _db.Listings.AsNoTracking().Include(x => x.Property).ThenInclude(p => p.Address)
                .Include(x => x.Landlord).ThenInclude(u => u!.UserProfile).Include(x => x.Landlord).ThenInclude(u => u!.LandlordProfile)
                .FirstOrDefaultAsync(x => x.ListingId == listingId);
            if (l == null) return null!;
            return new AdminListingReviewViewModel
            {
                ListingId = l.ListingId, Title = l.Title, Description = l.Description ?? string.Empty, Price = l.Price,
                IsVerified = l.IsVerified, CreatedAt = l.CreatedAt,
                LandlordName = l.Landlord?.UserProfile?.FullName ?? string.Empty,
                PhoneNumber = l.Landlord?.UserProfile?.PhoneNumber ?? string.Empty,
                WhatsAppNumber = l.Landlord?.LandlordProfile?.WhatsAppNumber,
                Province = l.Property.Address.Province, City = l.Property.Address.City ?? string.Empty,
                Suburb = l.Property.Address.Suburb ?? string.Empty, Street = l.Property.Address.Street ?? string.Empty,
                PropertyName = l.Property.PropertyName ?? string.Empty, PropertyType = l.Property.PropertyType ?? string.Empty,
                TotalRooms = l.Property.TotalRooms, PropertyVerified = l.Property.IsVerified,
                Photos = await GetListingPhotosAsync(listingId) as List<ListingPhotoViewModel> ?? (await GetListingPhotosAsync(listingId)).ToList()
            };
        }

        public async Task<AdminLandlordReviewViewModel> GetLandlordForVerificationAsync(string landlordUserId, int landlordProfileId)
        {
            var p = await _db.LandlordProfiles.AsNoTracking().Include(x => x.User).ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(x => x.UserId == landlordUserId || x.LandlordProfileId == landlordProfileId);
            if (p == null) return null!;
            return new AdminLandlordReviewViewModel
            {
                LandlordProfileId = p.LandlordProfileId, LandlordUserId = p.UserId, Email = p.User.Email ?? string.Empty,
                AccountPhoneNumber = p.User.PhoneNumber ?? string.Empty, AccountCreatedAt = p.User.CreatedAt,
                FullName = p.User.UserProfile?.FullName ?? string.Empty, PhoneNumber = p.User.UserProfile?.PhoneNumber ?? string.Empty,
                Province = p.User.UserProfile?.Province ?? string.Empty, City = p.User.UserProfile?.City ?? string.Empty,
                Bio = p.Bio ?? string.Empty, WhatsAppNumber = p.WhatsAppNumber, IsVerified = p.IsVerified, VerifiedAt = p.VerifiedAt,
                PropertyCount = await _db.Properties.CountAsync(x => x.LandlordId == p.UserId && x.IsActive),
                ListingCount = await _db.Listings.CountAsync(x => x.LandlordUserId == p.UserId && x.IsActive),
                VerifiedListingCount = await _db.Listings.CountAsync(x => x.LandlordUserId == p.UserId && x.IsActive && x.IsVerified)
            };
        }

        public async Task<AdminPropertyReviewViewModel> GetPropertyForVerificationAsync(int propertyId)
        {
            var p = await _db.Properties.AsNoTracking().Include(x => x.Address).Include(x => x.Landlord)
                .ThenInclude(u => u!.UserProfile).Include(x => x.Landlord).ThenInclude(u => u!.LandlordProfile)
                .FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.IsActive);
            if (p == null) return null!;
            return new AdminPropertyReviewViewModel
            {
                PropertyId = p.PropertyId, PropertyName = p.PropertyName ?? string.Empty, PropertyType = p.PropertyType ?? string.Empty,
                TotalRooms = p.TotalRooms ?? 0, CreatedAt = p.CreatedAt, IsVerified = p.IsVerified,
                VerificationNotes = p.VerificationNotes, VerifiedOn = p.VerifiedOn, LandlordUserId = p.LandlordId ?? string.Empty,
                FullName = p.Landlord?.UserProfile?.FullName ?? string.Empty, PhoneNumber = p.Landlord?.UserProfile?.PhoneNumber ?? string.Empty,
                WhatsAppNumber = p.Landlord?.LandlordProfile?.WhatsAppNumber, Bio = p.Landlord?.LandlordProfile?.Bio,
                LandlordVerified = p.Landlord?.LandlordProfile?.IsVerified ?? false,
                Province = p.Address.Province, City = p.Address.City ?? string.Empty, Suburb = p.Address.Suburb ?? string.Empty,
                Street = p.Address.Street ?? string.Empty,
                TotalPhotos = await _db.PropertyPhotos.CountAsync(x => x.PropertyId == propertyId && x.IsActive),
                TotalListings = await _db.Listings.CountAsync(x => x.PropertyId == propertyId && x.IsActive),
                Photos = (await GetPropertyPhotosAsync(propertyId)).ToList()
            };
        }

        public async Task<IEnumerable<ListingPhotoViewModel>> GetListingPhotosAsync(int listingId) =>
            await _db.ListingPhotos.AsNoTracking().Where(p => p.ListingId == listingId && p.IsActive)
                .OrderByDescending(p => p.IsPrimary).ThenBy(p => p.CreatedAt)
                .Select(p => new ListingPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToListAsync();

        public async Task<IEnumerable<PropertyPhotoViewModel>> GetPropertyPhotosAsync(int propertyId) =>
            await _db.PropertyPhotos.AsNoTracking().Where(p => p.PropertyId == propertyId && p.IsActive)
                .OrderByDescending(p => p.IsPrimary).ThenBy(p => p.CreatedAt)
                .Select(p => new PropertyPhotoViewModel { PhotoId = p.PhotoId, PhotoPath = p.PhotoPath, IsPrimary = p.IsPrimary }).ToListAsync();

        public async Task VerifyListingAsync(int listingId, string adminUserId, bool isApproved, string notes)
        {
            var l = await _db.Listings.FirstOrDefaultAsync(x => x.ListingId == listingId)
                ?? throw new InvalidOperationException("Listing not found.");
            if (l.IsVerified) throw new InvalidOperationException("Listing already verified.");
            l.IsVerified = isApproved; l.UpdatedAt = DateTime.UtcNow; l.VerifiedBy = isApproved ? adminUserId : null;
            l.VerifiedOn = DateTime.UtcNow; l.VerificationNotes = notes;
            AddLog(adminUserId, "Listing", listingId, isApproved, notes);
            await _db.SaveChangesAsync();
        }

        public async Task VerifyLandlordAsync(string landlordUserId, int landlordProfileId, string adminUserId, bool isApproved, string notes)
        {
            var p = await _db.LandlordProfiles.FirstOrDefaultAsync(x => x.UserId == landlordUserId)
                ?? throw new InvalidOperationException("Landlord profile not found.");
            if (isApproved && p.IsVerified) throw new InvalidOperationException("Landlord already verified.");
            p.IsVerified = isApproved; p.VerifiedAt = isApproved ? DateTime.UtcNow : null;
            p.VerifiedBy = adminUserId; p.VerificationNotes = notes;
            AddLog(adminUserId, "Landlord", landlordProfileId, isApproved, notes);
            await _db.SaveChangesAsync();
        }

        public async Task VerifyPropertyAsync(int propertyId, string adminUserId, bool isApproved, string notes)
        {
            var p = await _db.Properties.FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.IsActive)
                ?? throw new InvalidOperationException("Property not found.");
            p.IsVerified = isApproved; p.VerifiedBy = isApproved ? adminUserId : null;
            p.VerifiedOn = isApproved ? DateTime.UtcNow : null; p.VerificationNotes = notes;
            AddLog(adminUserId, "Property", propertyId, isApproved, notes);
            await _db.SaveChangesAsync();
        }

        private void AddLog(string adminId, string type, int id, bool approved, string notes) =>
            _db.AdminVerificationLogs.Add(new AdminVerificationLog
            {
                AdminUserId = adminId, EntityType = type, EntityId = id,
                Action = approved ? "Approved" : "Rejected", Notes = notes, CreatedOn = DateTime.UtcNow
            });

        public async Task<IEnumerable<AdminVerificationLogViewModel>> GetVerificationLogsAsync() =>
            await _db.AdminVerificationLogs.AsNoTracking().Where(l => l.AdminUser != null)
                .OrderByDescending(l => l.CreatedOn)
                .Select(l => new AdminVerificationLogViewModel
                {
                    LogId = l.LogId, EntityId = l.EntityId ?? 0, AdminUser = l.AdminUser!.UserName ?? string.Empty,
                    EntityType = l.EntityType ?? string.Empty, Action = l.Action ?? string.Empty,
                    Notes = l.Notes ?? string.Empty, CreatedOn = l.CreatedOn ?? default
                }).ToListAsync();

        public async Task<IEnumerable<ManageUsersViewModel>> GetUsersAsync()
        {
            var rows = await (from u in _db.Users.AsNoTracking()
                join ur in _db.UserRoles on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()
                join r in _db.Roles on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()
                select new { User = u, Role = r, Profile = u.UserProfile }).ToListAsync();
            return rows.OrderByDescending(x => x.Profile?.CreatedAt ?? x.User.CreatedAt).Select(x => new ManageUsersViewModel
            {
                Id = x.User.Id, Username = x.User.UserName ?? string.Empty, Email = x.User.Email ?? string.Empty,
                RoleName = x.Role == null ? string.Empty : x.Role.Name ?? string.Empty, CreatedAt = x.User.CreatedAt,
                FullName = x.Profile?.FullName ?? string.Empty, PhoneNumber = x.Profile?.PhoneNumber ?? string.Empty,
                IsBlocked = x.User.LockoutEnd.HasValue && x.User.LockoutEnd > DateTimeOffset.UtcNow
            }).ToList();
        }

        public async Task<AdminUserDetailsViewModel> GetUserDetailsAsync(string userId)
        {
            var u = await _db.Users.AsNoTracking().Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == userId);
            if (u == null) return null!;
            return new AdminUserDetailsViewModel
            {
                UserId = u.Id, FullName = u.UserProfile?.FullName ?? string.Empty, UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty, PhoneNumber = u.UserProfile?.PhoneNumber ?? string.Empty,
                RoleName = await (from ur in _db.UserRoles join r in _db.Roles on ur.RoleId equals r.Id
                    where ur.UserId == userId select r.Name ?? string.Empty).FirstOrDefaultAsync() ?? string.Empty,
                CreatedAt = u.CreatedAt, LockoutEnd = u.LockoutEnd,
                IsBlocked = u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow,
                PropertyCount = await _db.Properties.CountAsync(x => x.LandlordId == userId),
                ListingCount = await _db.Listings.CountAsync(x => x.LandlordUserId == userId),
                ConversationCount = await _db.Conversations.CountAsync(x => x.TenantId == userId || x.LandlordId == userId)
            };
        }
    }
}
