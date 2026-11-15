# Krn-Sqlite-Efcore

## SQLite schema and EF Core migrations

`ApplicationDbContext` uses EF Core's SQLite provider. Domain and ASP.NET Identity tables use singular physical names (for example, `Property`, `Listing`, `ListingPhoto`, and `AspNetUser`). EF's migration ledger is named `__EFMigrationHistory`. The connection string is `Data Source=krn.db` in the web project's `appsettings.json`.

At application startup, EF Core applies pending migrations before the Identity seeder runs. If `krn.db` is missing, this creates it from the migrations. The generated database and its SQLite WAL sidecars are ignored by Git; the schema source of truth is the migration code, not a checked-in `.db` file.

To create/update the database manually from the repository root:

```bash
dotnet tool install --global dotnet-ef --version 8.0.22
dotnet ef database update \
  --project KasiRoomNetwork.Data/KasiRoomNetwork.Data.csproj \
  --startup-project "Kasi Room Network - KRN/KasiRoomNetwork.UI.csproj" \
  --context KasiRoomNetwork.Data.DataAccess.ApplicationDbContext
```

To add a later schema change, update the entity and fluent configuration, then scaffold and apply a migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project KasiRoomNetwork.Data/KasiRoomNetwork.Data.csproj \
  --startup-project "Kasi Room Network - KRN/KasiRoomNetwork.UI.csproj" \
  --context KasiRoomNetwork.Data.DataAccess.ApplicationDbContext \
  --output-dir Migrations

dotnet ef database update \
  --project KasiRoomNetwork.Data/KasiRoomNetwork.Data.csproj \
  --startup-project "Kasi Room Network - KRN/KasiRoomNetwork.UI.csproj" \
  --context KasiRoomNetwork.Data.DataAccess.ApplicationDbContext
```

The old checked-in `krn.db` schema snapshot is removed; a new file is created from the initial EF migration. If you have a separate local copy of the former database, back it up before using it: its pre-existing tables have no EF migration-history record, so automatic migration cannot safely treat it as a fresh database. Use a new file or baseline an existing database only after carefully verifying its schema and data.

The SQL Server scripts under `Database/` are references, not the active SQLite schema. The repository's legacy Dapper/SqlClient stored-procedure services are not converted by EF migrations and still need a separate SQLite port.
