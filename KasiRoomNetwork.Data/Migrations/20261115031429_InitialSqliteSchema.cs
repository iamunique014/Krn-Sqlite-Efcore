using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KasiRoomNetwork.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqliteSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Suburb = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: true, defaultValue: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.CheckConstraint("CK_Address_IsActive", "IsActive IS NULL OR IsActive IN (0, 1)");
                });

            migrationBuilder.CreateTable(
                name: "Amenity",
                columns: table => new
                {
                    AmenityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenity", x => x.AmenityId);
                    table.CheckConstraint("CK_Amenity_IsActive", "IsActive IN (0, 1)");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaim_AspNetRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminVerificationLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdminUserId = table.Column<string>(type: "TEXT", nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminVerificationLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AdminVerificationLog_AspNetUser_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaim_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogin_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRole_AspNetRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRole_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserToken_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LandlordProfile",
                columns: table => new
                {
                    LandlordProfileId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    VerificationNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    WhatsAppNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandlordProfile", x => x.LandlordProfileId);
                    table.CheckConstraint("CK_LandlordProfile_WhatsAppNumber", "WhatsAppNumber IS NULL OR length(WhatsAppNumber) BETWEEN 10 AND 20");
                    table.ForeignKey(
                        name: "FK_LandlordProfile_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LandlordProfile_AspNetUser_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    LandlordId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TotalRooms = table.Column<int>(type: "INTEGER", nullable: true),
                    PropertyName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    VerifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerificationNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyId);
                    table.CheckConstraint("CK_Property_IsActive", "IsActive IN (0, 1)");
                    table.CheckConstraint("CK_Property_IsVerified", "IsVerified IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_Property_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Property_AspNetUser_LandlordId",
                        column: x => x.LandlordId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Property_AspNetUser_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserProfileId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Listing",
                columns: table => new
                {
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LandlordUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "NUMERIC", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerifiedBy = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    VerifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerificationNotes = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    AvailableUnits = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listing", x => x.ListingId);
                    table.CheckConstraint("CK_Listing_AvailableUnits", "AvailableUnits >= 0");
                    table.CheckConstraint("CK_Listing_IsActive", "IsActive IN (0, 1)");
                    table.CheckConstraint("CK_Listing_IsAvailable", "IsAvailable IN (0, 1)");
                    table.CheckConstraint("CK_Listing_IsVerified", "IsVerified IN (0, 1)");
                    table.CheckConstraint("CK_Listing_Price", "Price >= 0");
                    table.ForeignKey(
                        name: "FK_Listing_AspNetUser_LandlordUserId",
                        column: x => x.LandlordUserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Listing_AspNetUser_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Listing_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAmenity",
                columns: table => new
                {
                    PropertyAmenityId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmenityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAmenity", x => x.PropertyAmenityId);
                    table.ForeignKey(
                        name: "FK_PropertyAmenity_Amenity_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenity",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyAmenity_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPhoto",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhotoPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPhoto", x => x.PhotoId);
                    table.CheckConstraint("CK_PropertyPhoto_IsActive", "IsActive IN (0, 1)");
                    table.CheckConstraint("CK_PropertyPhoto_IsPrimary", "IsPrimary IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_PropertyPhoto_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    LandlordId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    LastMessageAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.ConversationId);
                    table.ForeignKey(
                        name: "FK_Conversation_AspNetUser_LandlordId",
                        column: x => x.LandlordId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversation_AspNetUser_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversation_Listing_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listing",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListingPhoto",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhotoPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingPhoto", x => x.PhotoId);
                    table.CheckConstraint("CK_ListingPhoto_IsActive", "IsActive IN (0, 1)");
                    table.CheckConstraint("CK_ListingPhoto_IsPrimary", "IsPrimary IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_ListingPhoto_Listing_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listing",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingContactLog",
                columns: table => new
                {
                    ContactLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: true),
                    ContactMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingContactLog", x => x.ContactLogId);
                    table.ForeignKey(
                        name: "FK_ListingContactLog_AspNetUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListingContactLog_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListingContactLog_Listing_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listing",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConversationId = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    MessageText = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.MessageId);
                    table.CheckConstraint("CK_Message_IsDeleted", "IsDeleted IN (0, 1)");
                    table.CheckConstraint("CK_Message_IsRead", "IsRead IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_Message_AspNetUser_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "ConversationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_Search",
                table: "Address",
                columns: new[] { "Province", "City", "Suburb" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminVerificationLog_AdminUserId",
                table: "AdminVerificationLog",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaim_RoleId",
                table: "AspNetRoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaim_UserId",
                table: "AspNetUserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogin_UserId",
                table: "AspNetUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRole_RoleId",
                table: "AspNetUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_LandlordId",
                table: "Conversation",
                column: "LandlordId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_TenantId",
                table: "Conversation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "UX_Conversation",
                table: "Conversation",
                columns: new[] { "ListingId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LandlordProfile_VerifiedBy",
                table: "LandlordProfile",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "UQ_Landlord_User",
                table: "LandlordProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listing_LandlordUserId",
                table: "Listing",
                column: "LandlordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_Price",
                table: "Listing",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_PropertyId",
                table: "Listing",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_Search",
                table: "Listing",
                columns: new[] { "IsAvailable", "IsVerified", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_Listing_VerifiedBy",
                table: "Listing",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ListingContactLog_ConversationId",
                table: "ListingContactLog",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingContactLog_ListingId",
                table: "ListingContactLog",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingContactLog_UserId",
                table: "ListingContactLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingPhoto_ListingId",
                table: "ListingPhoto",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId_CreatedAt",
                table: "Message",
                columns: new[] { "ConversationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_AddressId",
                table: "Property",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_LandlordId",
                table: "Property",
                column: "LandlordId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_VerifiedBy",
                table: "Property",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAmenity_AmenityId",
                table: "PropertyAmenity",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "UQ_PropertyAmenity",
                table: "PropertyAmenity",
                columns: new[] { "PropertyId", "AmenityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPhoto_PropertyId",
                table: "PropertyPhoto",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserProfile_User",
                table: "UserProfile",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminVerificationLog");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaim");

            migrationBuilder.DropTable(
                name: "AspNetUserClaim");

            migrationBuilder.DropTable(
                name: "AspNetUserLogin");

            migrationBuilder.DropTable(
                name: "AspNetUserRole");

            migrationBuilder.DropTable(
                name: "AspNetUserToken");

            migrationBuilder.DropTable(
                name: "LandlordProfile");

            migrationBuilder.DropTable(
                name: "ListingContactLog");

            migrationBuilder.DropTable(
                name: "ListingPhoto");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "PropertyAmenity");

            migrationBuilder.DropTable(
                name: "PropertyPhoto");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "AspNetRole");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "Amenity");

            migrationBuilder.DropTable(
                name: "Listing");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "AspNetUser");
        }
    }
}
