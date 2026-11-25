using System.Collections.Generic;
using System.Linq;

namespace KasiRoomNetwork.Common.ViewModel.Landlord
{
    public class PropertyManagementDashboardViewModel
    {
        public bool ProfileComplete { get; set; }
        public bool ShowWhatsAppPrompt { get; set; }
        public int UnreadMessages { get; set; }

        public IReadOnlyList<PropertyCardViewModel> Properties { get; set; } = new List<PropertyCardViewModel>();
        public IReadOnlyList<LandlordListingViewModel> Listings { get; set; } = new List<LandlordListingViewModel>();

        public int PropertyCount => Properties.Count;
        public int ListingCount => Listings.Count;
        public int AvailableListingCount => Listings.Count(listing => listing.IsAvailable);
        public int VerifiedListingCount => Listings.Count(listing => listing.IsVerified);
        public int PendingListingCount => Listings.Count(listing => !listing.IsVerified);
        public int AvailableUnitCount => Listings
            .Where(listing => listing.IsAvailable)
            .Sum(listing => listing.AvailableUnits);
    }
}
