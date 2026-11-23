using KasiRoomNetwork.Common.ViewModel.Profiles;
using KasiRoomNetwork.Data.DataAccess;
using KasiRoomNetwork.Data.Domain.Models;
using KasiRoomNetwork.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _db;

        public ProfileRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProfilePageViewModel?> GetByUserId(string userId)
        {
            var user = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);
            if (user == null)
                return null;

            var landlord = await _db.LandlordProfiles.AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return new ProfilePageViewModel
            {
                UserProfile = new ProfileViewModel
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    City = user.City ?? string.Empty,
                    Province = user.Province ?? string.Empty
                },
                LandlordProfile = landlord == null ? new LandlordPublicProfileViewModel() : new LandlordPublicProfileViewModel
                {
                    UserId = landlord.UserId,
                    Bio = landlord.Bio,
                    WhatsAppNumber = landlord.WhatsAppNumber,
                    IsVerified = landlord.IsVerified,
                    VerifiedAt = landlord.VerifiedAt,
                    VerificationNotes = landlord.VerificationNotes
                }
            };
        }

        public async Task SaveProfile(ProfilePageViewModel profile, string userId)
        {
            var entity = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (entity == null)
            {
                entity = new UserProfile { UserId = userId, FullName = profile.UserProfile.FullName };
                _db.UserProfiles.Add(entity);
            }
            entity.FullName = profile.UserProfile.FullName;
            entity.PhoneNumber = profile.UserProfile.PhoneNumber;
            entity.City = profile.UserProfile.City;
            entity.Province = profile.UserProfile.Province;
            await _db.SaveChangesAsync();
        }

        public async Task SaveLandlordProfile(ProfilePageViewModel profile, string userId)
        {
            var userProfile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (userProfile == null)
            {
                userProfile = new UserProfile { UserId = userId, FullName = profile.UserProfile.FullName };
                _db.UserProfiles.Add(userProfile);
            }
            userProfile.FullName = profile.UserProfile.FullName;
            userProfile.PhoneNumber = profile.UserProfile.PhoneNumber;
            userProfile.City = profile.UserProfile.City;
            userProfile.Province = profile.UserProfile.Province;

            var landlord = await _db.LandlordProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (landlord == null)
            {
                landlord = new LandlordProfile { UserId = userId };
                _db.LandlordProfiles.Add(landlord);
            }
            landlord.WhatsAppNumber = profile.LandlordProfile.WhatsAppNumber;
            landlord.Bio = profile.LandlordProfile.Bio;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsComplete(string userId)
        {
            var profile = await GetByUserId(userId);
            return profile != null
                && !string.IsNullOrWhiteSpace(profile.UserProfile.FullName)
                && !string.IsNullOrWhiteSpace(profile.UserProfile.PhoneNumber)
                && !string.IsNullOrWhiteSpace(profile.UserProfile.City)
                && !string.IsNullOrWhiteSpace(profile.UserProfile.Province);
        }
    }
}
