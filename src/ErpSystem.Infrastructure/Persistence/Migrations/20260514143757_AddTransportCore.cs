using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carriers_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CarrierId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrailerType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PayloadTons = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CarrierId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailers_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PayloadTons = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CarrierId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarrierId = table.Column<int>(type: "int", nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    TrailerId = table.Column<int>(type: "int", nullable: true),
                    DriverId = table.Column<int>(type: "int", nullable: true),
                    LoadingCountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    UnloadingCountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Incoterms = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    RequiresCustomsClearance = table.Column<bool>(type: "bit", nullable: false),
                    RequiresCmrDocuments = table.Column<bool>(type: "bit", nullable: false),
                    DomesticRegion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DomesticTransportKind = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportOrders_Carriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "Carriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportOrders_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportOrders_Trailers_TrailerId",
                        column: x => x.TrailerId,
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportOrders_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportOrderId = table.Column<int>(type: "int", nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    IsLocalCost = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportCosts_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportOrderId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IssuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportDocuments_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportOrderRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportOrderId = table.Column<int>(type: "int", nullable: false),
                    PlannedDistanceKm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlannedRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlannedLoadingAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedUnloadingAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RouteSummary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportOrderRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportOrderRoutes_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportStatusHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportOrderId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportStatusHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportStatusHistoryEntries_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransportStatusHistoryEntries_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransportStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportOrderId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StopType = table.Column<int>(type: "int", nullable: false),
                    ContractorId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PlannedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportStops_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransportStops_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NewValues", "Summary" },
                values: new object[] { "{\"key\":\"TR_ORD\"}", "Seeded shared transport numbering." });

            migrationBuilder.InsertData(
                table: "Contractors",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "Name", "ShortName", "TaxId", "Types" },
                values: new object[,]
                {
                    { 1, "CTR-001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Baltic Retail Sp. z o.o.", "Baltic Retail", "5253001001", 9 },
                    { 2, "CTR-002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Nordic Components GmbH", "Nordic Components", "DE123456789", 18 },
                    { 3, "CTR-003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Trans-Pol Logistics Sp. z o.o.", "Trans-Pol", "5272003003", 20 },
                    { 4, "CTR-004", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Euro Customs Agency Sp. z o.o.", "Euro Customs", "5264004004", 96 },
                    { 5, "CTR-005", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Inter Cargo EU s.r.o.", "Inter Cargo EU", "CZ12345678", 12 }
                });

            migrationBuilder.UpdateData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 1,
                column: "StatusCode",
                value: "New");

            migrationBuilder.UpdateData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 2,
                column: "StatusCode",
                value: "Planned");

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4060));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Key", "LastGeneratedAtUtc", "Prefix" },
                values: new object[] { "TR_ORD", new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150), "TR" });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 37, 56, 757, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "New", "Nowe" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "Accepted", "Przyjęte" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "IsTerminal", "Name" },
                values: new object[] { "Planned", false, "Zaplanowane" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "InProgress", "transport", "W realizacji", 40 });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "IsTerminal", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "Loaded", false, "transport", "Załadowane", 50 });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "IsTerminal", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "Delivered", false, "transport", "Dostarczone", 60 });

            migrationBuilder.InsertData(
                table: "DocumentStatuses",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "IsDeleted", "IsTerminal", "ModifiedAt", "ModifiedByUserId", "ModuleKey", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 9, "Closed", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, null, null, "transport", "Zamknięte", 70 },
                    { 10, "Cancelled", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, null, null, "transport", "Anulowane", 80 },
                    { 11, "Requested", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, false, null, null, "hr", "Requested", 10 },
                    { 12, "Approved", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, null, null, "hr", "Approved", 20 },
                    { 13, "Rejected", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, true, null, null, "hr", "Rejected", 30 }
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "Edit", "Edit transport orders.", "Transport.Order.Edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "ChangeStatus", "Change transport order status.", "Transport.Order.ChangeStatus" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "AssignDriver", "Assign driver to transport order.", "transport", "Transport.Order.AssignDriver", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "Read", "Read employee cards.", "HR.Employee.Read" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Action", "Description", "Name", "Resource" },
                values: new object[] { "Edit", "Edit employee cards.", "HR.Employee.Edit", "Employee" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "CreatedByUserId", "Description", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "ModuleKey", "Name", "Resource" },
                values: new object[] { 39, "Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Approve leave requests.", false, null, null, "hr", "HR.Leave.Approve", "Leave" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 39, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "PermissionId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "PermissionId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "PermissionId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "PermissionId",
                value: 24);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "PermissionId",
                value: 25);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "PermissionId",
                value: 26);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "PermissionId",
                value: 27);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "PermissionId",
                value: 28);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "PermissionId",
                value: 29);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "PermissionId",
                value: 30);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 31, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "PermissionId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "PermissionId",
                value: 32);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "PermissionId",
                value: 33);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "PermissionId",
                value: 34);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 35, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 36, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "PermissionId",
                value: 37);

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PermissionId", "RoleId" },
                values: new object[] { 68, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 38, 4 });

            migrationBuilder.InsertData(
                table: "Carriers",
                columns: new[] { "Id", "Code", "ContractorId", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "IsPreferred", "ModifiedAt", "ModifiedByUserId" },
                values: new object[,]
                {
                    { 1, "CAR-TRANS-POL", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, true, null, null },
                    { 2, "CAR-INTER-EU", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, false, null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PermissionId", "RoleId" },
                values: new object[] { 69, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 39, 4 });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "CarrierId", "CreatedAt", "CreatedByUserId", "FirstName", "IsActive", "IsDeleted", "LastName", "LicenseNumber", "ModifiedAt", "ModifiedByUserId", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Jan", true, false, "Nowak", "PL1234567", null, null, "+48 600 100 200" },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Piotr", true, false, "Kaczmarek", "PL7654321", null, null, "+48 600 300 400" }
                });

            migrationBuilder.InsertData(
                table: "Trailers",
                columns: new[] { "Id", "CarrierId", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PayloadTons", "RegistrationNumber", "TrailerType" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, 24.0m, "WPR 3003C", "Firanka" },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, 22.0m, "WPR 4004D", "Chłodnia" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Brand", "CarrierId", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "Model", "ModifiedAt", "ModifiedByUserId", "PayloadTons", "RegistrationNumber" },
                values: new object[,]
                {
                    { 1, "DAF", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, "XF 480", null, null, 18.0m, "WPR 1001A" },
                    { 2, "Volvo", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, "FH 460", null, null, 18.0m, "WPR 2002B" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_Code",
                table: "Carriers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_ContractorId",
                table: "Carriers",
                column: "ContractorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CarrierId",
                table: "Drivers",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_CarrierId",
                table: "Trailers",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_RegistrationNumber",
                table: "Trailers",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportCosts_TransportOrderId",
                table: "TransportCosts",
                column: "TransportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportDocuments_TransportOrderId",
                table: "TransportDocuments",
                column: "TransportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrderRoutes_TransportOrderId",
                table: "TransportOrderRoutes",
                column: "TransportOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrders_CarrierId",
                table: "TransportOrders",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrders_DriverId",
                table: "TransportOrders",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrders_Number",
                table: "TransportOrders",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrders_TrailerId",
                table: "TransportOrders",
                column: "TrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportOrders_VehicleId",
                table: "TransportOrders",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportStatusHistoryEntries_ChangedByUserId",
                table: "TransportStatusHistoryEntries",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportStatusHistoryEntries_TransportOrderId",
                table: "TransportStatusHistoryEntries",
                column: "TransportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportStops_ContractorId",
                table: "TransportStops",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportStops_TransportOrderId_Sequence",
                table: "TransportStops",
                columns: new[] { "TransportOrderId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CarrierId",
                table: "Vehicles",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RegistrationNumber",
                table: "Vehicles",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportCosts");

            migrationBuilder.DropTable(
                name: "TransportDocuments");

            migrationBuilder.DropTable(
                name: "TransportOrderRoutes");

            migrationBuilder.DropTable(
                name: "TransportStatusHistoryEntries");

            migrationBuilder.DropTable(
                name: "TransportStops");

            migrationBuilder.DropTable(
                name: "TransportOrders");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.UpdateData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NewValues", "Summary" },
                values: new object[] { "{\"key\":\"TRANSPORT_DOMESTIC\"}", "Seeded domestic transport numbering." });

            migrationBuilder.UpdateData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 1,
                column: "StatusCode",
                value: "Draft");

            migrationBuilder.UpdateData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 2,
                column: "StatusCode",
                value: "Assigned");

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7390));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Key", "LastGeneratedAtUtc", "Prefix" },
                values: new object[] { "TRANSPORT_DOMESTIC", new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7470), "TD" });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7470));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7480));

            migrationBuilder.InsertData(
                table: "DocumentNumberSequences",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "CurrentNumber", "IsDeleted", "Key", "LastGeneratedAtUtc", "ModifiedAt", "ModifiedByUserId", "Padding", "Prefix", "ResetPolicy" },
                values: new object[] { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 9, false, "TRANSPORT_INTERNATIONAL", new DateTime(2026, 5, 14, 13, 48, 37, 277, DateTimeKind.Utc).AddTicks(7470), null, null, 5, "TI", 2 });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Name" },
                values: new object[] { "Draft", "Draft" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Name" },
                values: new object[] { "Assigned", "Assigned" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "IsTerminal", "Name" },
                values: new object[] { "Completed", true, "Completed" });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "Requested", "hr", "Requested", 10 });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "IsTerminal", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "Approved", true, "hr", "Approved", 20 });

            migrationBuilder.UpdateData(
                table: "DocumentStatuses",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "IsTerminal", "ModuleKey", "Name", "SortOrder" },
                values: new object[] { "Rejected", true, "hr", "Rejected", 30 });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "ChangeStatus", "Change transport order status.", "Transport.Order.ChangeStatus" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "AssignDriver", "Assign driver to transport order.", "Transport.Order.AssignDriver" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read", "Read employee cards.", "hr", "HR.Employee.Read", "Employee" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Action", "Description", "Name" },
                values: new object[] { "Edit", "Edit employee cards.", "HR.Employee.Edit" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Action", "Description", "Name", "Resource" },
                values: new object[] { "Approve", "Approve leave requests.", "HR.Leave.Approve", "Leave" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "PermissionId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "PermissionId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "PermissionId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "PermissionId",
                value: 24);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "PermissionId",
                value: 25);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "PermissionId",
                value: 26);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "PermissionId",
                value: 27);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "PermissionId",
                value: 28);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "PermissionId",
                value: 29);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "PermissionId",
                value: 30);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "PermissionId",
                value: 31);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "PermissionId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "PermissionId",
                value: 32);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "PermissionId",
                value: 33);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "PermissionId",
                value: 34);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "PermissionId",
                value: 35);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "PermissionId",
                value: 36);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "PermissionId",
                value: 37);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "PermissionId",
                value: 38);
        }
    }
}
