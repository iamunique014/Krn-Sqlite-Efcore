using Microsoft.AspNetCore.Identity;

namespace KasiRoomNetwork.Data.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public ICollection<AdminVerificationLog> AdminVerificationLogs { get; set; } = new List<AdminVerificationLog>();
    public ICollection<Conversation> TenantConversations { get; set; } = new List<Conversation>();
    public ICollection<Conversation> LandlordConversations { get; set; } = new List<Conversation>();
    public ICollection<Listing> LandlordListings { get; set; } = new List<Listing>();
    public ICollection<Listing> VerifiedListings { get; set; } = new List<Listing>();
    public ICollection<Property> LandlordProperties { get; set; } = new List<Property>();
    public ICollection<Property> VerifiedProperties { get; set; } = new List<Property>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<ListingContactLog> ListingContactLogs { get; set; } = new List<ListingContactLog>();
    public ICollection<LandlordProfile> VerifiedLandlordProfiles { get; set; } = new List<LandlordProfile>();

    public LandlordProfile? LandlordProfile { get; set; }
    public UserProfile? UserProfile { get; set; }
}
