using KasiRoomNetwork.Common.DTOs;
using Kasi_Room_Network___KRN.Services;
using KasiRoomNetwork.Data.Interfaces;
using KasiRoomNetwork.Common.ViewModel.Properties;
using KasiRoomNetwork.Data.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Kasi_Room_Network___KRN.Services
{
    public class CreatePropertyService : ICreatePropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IPhotoStorageService _photoStorageService;
        private readonly ApplicationDbContext _dbContext;

        public CreatePropertyService(
            IPropertyRepository propertyRepository,
            IAmenityRepository amenityRepository,
            IPhotoStorageService photoStorageService,
            ApplicationDbContext dbContext)
        {
            _propertyRepository = propertyRepository;
            _amenityRepository = amenityRepository;
            _photoStorageService = photoStorageService;
            _dbContext = dbContext;
        }

        public async Task<int> CreatePropertyAsync(CreatePropertyWizardDto dto)
        {
            int propertyId = 0;
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;

            var PermanentPropertyPhotoPaths = new List<string>();

            try
            {
                // Establish a single connection for the transaction
                transaction = await _dbContext.Database.BeginTransactionAsync();

                // 1. Create Property
                var createPropertyViewModel = new CreatePropertyViewModel
                {
                    PropertyType = dto.PropertyType,
                    TotalRooms = dto.TotalRooms,
                    PropertyName = dto.PropertyName,
                    Street = dto.Street,
                    Province = dto.Province,
                    City = dto.City,
                    Suburb = dto.Suburb
                };
                propertyId = await _propertyRepository.CreateProperty(createPropertyViewModel, dto.LandlordUserId);

                // 2. Add Amenities
                foreach (var amenityId in dto.AmenityIds)
                {
                    await _amenityRepository.AddPropertyAmenity(propertyId, amenityId, dto.LandlordUserId);
                }

                // 3. Handle Photos (Move from temp to permanent storage and add metadata) 
                //(Only the Last photo is set to primary by stored procedure)
                foreach (var tempPhotoPath in dto.TemporaryPhotoPaths)
                {
                    string permanentPath = await _photoStorageService.CopyTemporaryPhotoToPermanentAsync(tempPhotoPath, "properties");
                    PermanentPropertyPhotoPaths.Add(permanentPath);
                    bool isPrimary = true; //Every photo is set primary, this is changed in db by Sp.
                    await _propertyRepository.AddPropertyPhoto(propertyId, permanentPath, isPrimary, dto.LandlordUserId);
                }

                // Commit the transaction if all database operations are successful
                await transaction.CommitAsync();

                // Clean up temporary wizard photos after successful commit
                _photoStorageService.DeleteTemporaryWizardFolder(dto.LandlordUserId);
            }
            catch (Exception ex)
            {
                // Rollback transaction on any database-related error
                if (transaction != null) await transaction.RollbackAsync();

                // Compensation: Delete any permanent blobs that were uploaded if the transaction failed
                foreach (var path in PermanentPropertyPhotoPaths)
                {
                    _photoStorageService.DeletePhoto(path);
                }

                // Re-throw the exception for upstream error handling
                throw;
            }
            finally
            {
                if (transaction != null) await transaction.DisposeAsync();
            }

            return propertyId;
        }
    }
}