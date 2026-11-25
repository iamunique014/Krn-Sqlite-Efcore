namespace KasiRoomNetwork.Common.ViewModel.Properties
{
    public class PropertyListingViewModel
    {
        public int ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableUnits { get; set; }
        public bool IsVerified { get; set; }
        public string PrimaryPhoto { get; set; } = string.Empty;
    }
}
