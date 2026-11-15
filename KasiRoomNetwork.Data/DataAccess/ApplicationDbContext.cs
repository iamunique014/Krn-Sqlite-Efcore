using KasiRoomNetwork.Data.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KasiRoomNetwork.Data.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<AdminVerificationLog> AdminVerificationLogs => Set<AdminVerificationLog>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<LandlordProfile> LandlordProfiles => Set<LandlordProfile>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingContactLog> ListingContactLogs => Set<ListingContactLog>();
    public DbSet<ListingPhoto> ListingPhotos => Set<ListingPhoto>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
    public DbSet<PropertyPhoto> PropertyPhotos => Set<PropertyPhoto>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    private const string UtcNowSql = "strftime('%Y-%m-%dT%H:%M:%fZ','now')";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("AspNetUser");
        builder.Entity<IdentityRole>().ToTable("AspNetRole");
        builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaim");
        builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogin");
        builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRole");
        builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserToken");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaim");

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.CreatedAt)
                .HasDefaultValueSql(UtcNowSql);
            entity.Property(user => user.EmailConfirmed).HasDefaultValue(false);
            entity.Property(user => user.PhoneNumberConfirmed).HasDefaultValue(false);
            entity.Property(user => user.TwoFactorEnabled).HasDefaultValue(false);
            entity.Property(user => user.LockoutEnabled).HasDefaultValue(false);
            entity.Property(user => user.AccessFailedCount).HasDefaultValue(0);

            entity.HasOne(user => user.LandlordProfile)
                .WithOne(profile => profile.User)
                .HasForeignKey<LandlordProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(user => user.UserProfile)
                .WithOne(profile => profile.User)
                .HasForeignKey<UserProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(user => user.AdminVerificationLogs)
                .WithOne(log => log.AdminUser)
                .HasForeignKey(log => log.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.TenantConversations)
                .WithOne(conversation => conversation.Tenant)
                .HasForeignKey(conversation => conversation.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.LandlordConversations)
                .WithOne(conversation => conversation.Landlord)
                .HasForeignKey(conversation => conversation.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.LandlordListings)
                .WithOne(listing => listing.Landlord)
                .HasForeignKey(listing => listing.LandlordUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.VerifiedListings)
                .WithOne(listing => listing.Verifier)
                .HasForeignKey(listing => listing.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.LandlordProperties)
                .WithOne(property => property.Landlord)
                .HasForeignKey(property => property.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.VerifiedProperties)
                .WithOne(property => property.Verifier)
                .HasForeignKey(property => property.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.SentMessages)
                .WithOne(message => message.Sender)
                .HasForeignKey(message => message.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(user => user.ListingContactLogs)
                .WithOne(log => log.User)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Address>(entity =>
        {
            entity.ToTable("Address", table => table.HasCheckConstraint(
                "CK_Address_IsActive", "IsActive IS NULL OR IsActive IN (0, 1)"));
            entity.HasKey(address => address.AddressId);
            entity.Property(address => address.Street).HasMaxLength(150);
            entity.Property(address => address.City).HasMaxLength(100);
            entity.Property(address => address.Suburb).HasMaxLength(100);
            entity.Property(address => address.Province).IsRequired().HasMaxLength(100);
            entity.Property(address => address.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.Property(address => address.IsActive).HasDefaultValue(true);
            entity.HasIndex(address => new { address.Province, address.City, address.Suburb })
                .HasDatabaseName("IX_Address_Search");
        });

        builder.Entity<AdminVerificationLog>(entity =>
        {
            entity.ToTable("AdminVerificationLog");
            entity.HasKey(log => log.LogId);
            entity.Property(log => log.EntityType).HasMaxLength(50);
            entity.Property(log => log.Action).HasMaxLength(50);
            entity.Property(log => log.Notes).HasMaxLength(255);
            entity.Property(log => log.CreatedOn).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(log => log.AdminUserId);
        });

        builder.Entity<Amenity>(entity =>
        {
            entity.ToTable("Amenity", table =>
            {
                table.HasCheckConstraint("CK_Amenity_IsActive", "IsActive IN (0, 1)");
            });
            entity.HasKey(amenity => amenity.AmenityId);
            entity.Property(amenity => amenity.Name).IsRequired().HasMaxLength(100);
            entity.Property(amenity => amenity.Category).HasMaxLength(50);
            entity.Property(amenity => amenity.Icon).HasMaxLength(50);
            entity.Property(amenity => amenity.IsActive).HasDefaultValue(true);
            entity.Property(amenity => amenity.CreatedAt).HasDefaultValueSql(UtcNowSql);
        });

        builder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversation");
            entity.HasKey(conversation => conversation.ConversationId);
            entity.Property(conversation => conversation.TenantId).HasMaxLength(450);
            entity.Property(conversation => conversation.LandlordId).HasMaxLength(450);
            entity.Property(conversation => conversation.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(conversation => new { conversation.ListingId, conversation.TenantId })
                .IsUnique().HasDatabaseName("UX_Conversation");
            entity.HasIndex(conversation => conversation.TenantId);
            entity.HasIndex(conversation => conversation.LandlordId);
            entity.HasOne(conversation => conversation.Listing)
                .WithMany(listing => listing.Conversations)
                .HasForeignKey(conversation => conversation.ListingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<LandlordProfile>(entity =>
        {
            entity.ToTable("LandlordProfile", table =>
                table.HasCheckConstraint("CK_LandlordProfile_WhatsAppNumber",
                    "WhatsAppNumber IS NULL OR length(WhatsAppNumber) BETWEEN 10 AND 20"));
            entity.HasKey(profile => profile.LandlordProfileId);
            entity.Property(profile => profile.UserId).IsRequired().HasMaxLength(450);
            entity.Property(profile => profile.Bio).HasMaxLength(500);
            entity.Property(profile => profile.IsVerified).HasDefaultValue(false);
            entity.Property(profile => profile.VerifiedBy).HasMaxLength(450);
            entity.Property(profile => profile.VerificationNotes).HasMaxLength(255);
            entity.Property(profile => profile.WhatsAppNumber).HasMaxLength(20);
            entity.HasIndex(profile => profile.UserId).IsUnique().HasDatabaseName("UQ_Landlord_User");
            entity.HasOne(profile => profile.Verifier)
                .WithMany(user => user.VerifiedLandlordProfiles)
                .HasForeignKey(profile => profile.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Listing>(entity =>
        {
            entity.ToTable("Listing", table =>
            {
                table.HasCheckConstraint("CK_Listing_Price", "Price >= 0");
                table.HasCheckConstraint("CK_Listing_AvailableUnits", "AvailableUnits >= 0");
                table.HasCheckConstraint("CK_Listing_IsAvailable", "IsAvailable IN (0, 1)");
                table.HasCheckConstraint("CK_Listing_IsVerified", "IsVerified IN (0, 1)");
                table.HasCheckConstraint("CK_Listing_IsActive", "IsActive IN (0, 1)");
            });
            entity.HasKey(listing => listing.ListingId);
            entity.Property(listing => listing.LandlordUserId).HasMaxLength(450);
            entity.Property(listing => listing.Title).IsRequired().HasMaxLength(150);
            entity.Property(listing => listing.Price).HasColumnType("NUMERIC");
            entity.Property(listing => listing.IsAvailable).HasDefaultValue(true);
            entity.Property(listing => listing.IsVerified).HasDefaultValue(false);
            entity.Property(listing => listing.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.Property(listing => listing.IsActive).HasDefaultValue(true);
            entity.Property(listing => listing.VerifiedBy).HasMaxLength(450);
            entity.Property(listing => listing.VerificationNotes).HasMaxLength(255);
            entity.Property(listing => listing.AvailableUnits).HasDefaultValue(1);
            entity.Property(listing => listing.Status).IsRequired().HasDefaultValue("Active");
            entity.HasIndex(listing => listing.PropertyId);
            entity.HasIndex(listing => listing.LandlordUserId);
            entity.HasIndex(listing => listing.Price);
            entity.HasIndex(listing => new { listing.IsAvailable, listing.IsVerified, listing.Price })
                .HasDatabaseName("IX_Listing_Search");
            entity.HasOne(listing => listing.Property)
                .WithMany(property => property.Listings)
                .HasForeignKey(listing => listing.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ListingContactLog>(entity =>
        {
            entity.ToTable("ListingContactLog");
            entity.HasKey(log => log.ContactLogId);
            entity.Property(log => log.UserId).HasMaxLength(450);
            entity.Property(log => log.ContactMethod).IsRequired().HasMaxLength(50);
            entity.Property(log => log.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasOne(log => log.Listing)
                .WithMany(listing => listing.ContactLogs)
                .HasForeignKey(log => log.ListingId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(log => log.Conversation)
                .WithMany(conversation => conversation.ContactLogs)
                .HasForeignKey(log => log.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ListingPhoto>(entity =>
        {
            entity.ToTable("ListingPhoto", table =>
            {
                table.HasCheckConstraint("CK_ListingPhoto_IsPrimary", "IsPrimary IN (0, 1)");
                table.HasCheckConstraint("CK_ListingPhoto_IsActive", "IsActive IN (0, 1)");
            });
            entity.HasKey(photo => photo.PhotoId);
            entity.Property(photo => photo.PhotoPath).IsRequired().HasMaxLength(255);
            entity.Property(photo => photo.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.Property(photo => photo.IsPrimary).HasDefaultValue(false);
            entity.Property(photo => photo.IsActive).HasDefaultValue(true);
            entity.HasIndex(photo => photo.ListingId);
            entity.HasOne(photo => photo.Listing)
                .WithMany(listing => listing.Photos)
                .HasForeignKey(photo => photo.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Message>(entity =>
        {
            entity.ToTable("Message", table =>
            {
                table.HasCheckConstraint("CK_Message_IsRead", "IsRead IN (0, 1)");
                table.HasCheckConstraint("CK_Message_IsDeleted", "IsDeleted IN (0, 1)");
            });
            entity.HasKey(message => message.MessageId);
            entity.Property(message => message.SenderId).IsRequired().HasMaxLength(450);
            entity.Property(message => message.MessageText).IsRequired().HasMaxLength(1000);
            entity.Property(message => message.IsRead).HasDefaultValue(false);
            entity.Property(message => message.IsDeleted).HasDefaultValue(false);
            entity.Property(message => message.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(message => new { message.ConversationId, message.CreatedAt })
                .HasDatabaseName("IX_Message_ConversationId_CreatedAt");
            entity.HasOne(message => message.Conversation)
                .WithMany(conversation => conversation.Messages)
                .HasForeignKey(message => message.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Property>(entity =>
        {
            entity.ToTable("Property", table =>
            {
                table.HasCheckConstraint("CK_Property_IsVerified", "IsVerified IN (0, 1)");
                table.HasCheckConstraint("CK_Property_IsActive", "IsActive IN (0, 1)");
            });
            entity.HasKey(property => property.PropertyId);
            entity.Property(property => property.LandlordId).HasMaxLength(450);
            entity.Property(property => property.PropertyType).HasMaxLength(50);
            entity.Property(property => property.PropertyName).HasMaxLength(150);
            entity.Property(property => property.IsVerified).HasDefaultValue(false);
            entity.Property(property => property.VerifiedBy).HasMaxLength(450);
            entity.Property(property => property.VerificationNotes).HasMaxLength(255);
            entity.Property(property => property.IsActive).HasDefaultValue(true);
            entity.Property(property => property.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(property => property.AddressId);
            entity.HasIndex(property => property.LandlordId);
            entity.HasOne(property => property.Address)
                .WithMany(address => address.Properties)
                .HasForeignKey(property => property.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PropertyAmenity>(entity =>
        {
            entity.ToTable("PropertyAmenity");
            entity.HasKey(link => link.PropertyAmenityId);
            entity.HasIndex(link => new { link.PropertyId, link.AmenityId })
                .IsUnique().HasDatabaseName("UQ_PropertyAmenity");
            entity.HasIndex(link => link.AmenityId);
            entity.HasOne(link => link.Property)
                .WithMany(property => property.PropertyAmenities)
                .HasForeignKey(link => link.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(link => link.Amenity)
                .WithMany(amenity => amenity.PropertyAmenities)
                .HasForeignKey(link => link.AmenityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PropertyPhoto>(entity =>
        {
            entity.ToTable("PropertyPhoto", table =>
            {
                table.HasCheckConstraint("CK_PropertyPhoto_IsPrimary", "IsPrimary IN (0, 1)");
                table.HasCheckConstraint("CK_PropertyPhoto_IsActive", "IsActive IN (0, 1)");
            });
            entity.HasKey(photo => photo.PhotoId);
            entity.Property(photo => photo.PhotoPath).IsRequired().HasMaxLength(255);
            entity.Property(photo => photo.IsPrimary).HasDefaultValue(false);
            entity.Property(photo => photo.IsActive).HasDefaultValue(false);
            entity.Property(photo => photo.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(photo => photo.PropertyId);
            entity.HasOne(photo => photo.Property)
                .WithMany(property => property.Photos)
                .HasForeignKey(photo => photo.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfile");
            entity.HasKey(profile => profile.UserProfileId);
            entity.Property(profile => profile.UserId).IsRequired().HasMaxLength(450);
            entity.Property(profile => profile.FullName).IsRequired().HasMaxLength(100);
            entity.Property(profile => profile.PhoneNumber).HasMaxLength(20);
            entity.Property(profile => profile.City).HasMaxLength(100);
            entity.Property(profile => profile.Province).HasMaxLength(100);
            entity.Property(profile => profile.CreatedAt).HasDefaultValueSql(UtcNowSql);
            entity.HasIndex(profile => profile.UserId).IsUnique().HasDatabaseName("UQ_UserProfile_User");
        });
    }
}
