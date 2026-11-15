namespace KasiRoomNetwork.Data.Domain.Models;

public class Address
{
    public int AddressId { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Suburb { get; set; }
    public required string Province { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}

public class AdminVerificationLog
{
    public int LogId { get; set; }
    public string? AdminUserId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Action { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedOn { get; set; }
    public ApplicationUser? AdminUser { get; set; }
}

public class Amenity
{
    public int AmenityId { get; set; }
    public required string Name { get; set; }
    public string? Category { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}

public class Conversation
{
    public int ConversationId { get; set; }
    public int ListingId { get; set; }
    public required string TenantId { get; set; }
    public required string LandlordId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public Listing Listing { get; set; } = null!;
    public ApplicationUser Tenant { get; set; } = null!;
    public ApplicationUser Landlord { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ListingContactLog> ContactLogs { get; set; } = new List<ListingContactLog>();
}

public class LandlordProfile
{
    public int LandlordProfileId { get; set; }
    public required string UserId { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public string? VerificationNotes { get; set; }
    public string? WhatsAppNumber { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public ApplicationUser? Verifier { get; set; }
}

public class Listing
{
    public int ListingId { get; set; }
    public string? LandlordUserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UpdatedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? VerificationNotes { get; set; }
    public int PropertyId { get; set; }
    public int AvailableUnits { get; set; } = 1;
    public required string Status { get; set; } = "Active";
    public ApplicationUser? Landlord { get; set; }
    public ApplicationUser? Verifier { get; set; }
    public Property Property { get; set; } = null!;
    public ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public ICollection<ListingContactLog> ContactLogs { get; set; } = new List<ListingContactLog>();
}

public class ListingContactLog
{
    public int ContactLogId { get; set; }
    public int ListingId { get; set; }
    public string? UserId { get; set; }
    public int? ConversationId { get; set; }
    public required string ContactMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public Listing Listing { get; set; } = null!;
    public ApplicationUser? User { get; set; }
    public Conversation? Conversation { get; set; }
}

public class ListingPhoto
{
    public int PhotoId { get; set; }
    public int ListingId { get; set; }
    public required string PhotoPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public Listing Listing { get; set; } = null!;
}

public class Message
{
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public required string SenderId { get; set; }
    public required string MessageText { get; set; }
    public bool IsRead { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public Conversation Conversation { get; set; } = null!;
    public ApplicationUser Sender { get; set; } = null!;
}

public class Property
{
    public int PropertyId { get; set; }
    public int AddressId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? LandlordId { get; set; }
    public string? PropertyType { get; set; }
    public int? TotalRooms { get; set; }
    public string? PropertyName { get; set; }
    public bool IsVerified { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? VerificationNotes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
    public Address Address { get; set; } = null!;
    public ApplicationUser? Landlord { get; set; }
    public ApplicationUser? Verifier { get; set; }
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
}

public class PropertyAmenity
{
    public int PropertyAmenityId { get; set; }
    public int PropertyId { get; set; }
    public int AmenityId { get; set; }
    public Property Property { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}

public class PropertyPhoto
{
    public int PhotoId { get; set; }
    public int PropertyId { get; set; }
    public required string PhotoPath { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Property Property { get; set; } = null!;
}

public class UserProfile
{
    public int UserProfileId { get; set; }
    public required string UserId { get; set; }
    public required string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public DateTime? CreatedAt { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
