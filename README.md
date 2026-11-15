# Krn-Sqlite-Efcore

## SQLite schema and EF Core migrations

The active `ApplicationDbContext` uses the SQLite provider and maps the ASP.NET Identity tables plus the rental-domain entities to the SQLite schema. The connection string is loaded from the environment-specific appsettings file and points to `Data Source=krn.db`.

From the repository root, install the matching EF Core CLI tool if needed, then create or update the database:

```bash
dotnet tool install --global dotnet-ef --version 8.0.22
dotnet ef database update \
  --project KasiRoomNetwork.Data/KasiRoomNetwork.Data.csproj \
  --startup-project "Kasi Room Network - KRN/KasiRoomNetwork.UI.csproj" \
  --context KasiRoomNetwork.Data.DataAccess.ApplicationDbContext
```

To add a later schema change, add/update the entity and fluent configuration, then scaffold a migration before applying it:

```bash
dotnet ef migrations add <MigrationName> \
  --project KasiRoomNetwork.Data/KasiRoomNetwork.Data.csproj \
  --startup-project "Kasi Room Network - KRN/KasiRoomNetwork.UI.csproj" \
  --context KasiRoomNetwork.Data.DataAccess.ApplicationDbContext \
  --output-dir Migrations
```

The repository already contains `Kasi Room Network - KRN/krn.db`, a legacy SQLite schema snapshot whose tables predate the EF migration history. Do **not** run the initial migration against that file as-is: EF will attempt to create tables that already exist. Back up any database with data and use a new/empty SQLite file for the initial migration, or baseline an existing database only after verifying its schema against the migration. The migration test performed during development uses a temporary database and leaves the checked-in file untouched.

The SQL Server scripts under `Database/` are schema references, not the active SQLite schema. The SQLite mapping retains its physical table names (`Properties`, `Listings`, and `ListingPhotos`) and SQLite-specific schema details such as `Listings.Status`; differences in constraints and deletion rules should be reviewed before attempting to share the model with SQL Server.
