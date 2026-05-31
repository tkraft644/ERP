using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAuthAndTrimDemoSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Attachments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Attachments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AuditLogs",
                keyColumn: "Id",
                keyValue: 3);

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
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CurrencyRates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CurrencyRates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DocumentHistoryEntries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DriverProfiles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmployeeDocuments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmployeeDocuments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmployeeDocuments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmploymentContracts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmploymentContracts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmploymentContracts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Trailers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trailers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkSchedules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Carriers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Carriers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Contractors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "DictionaryItems",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "DictionaryName", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "Name", "SortOrder", "Value" },
                values: new object[,]
                {
                    { 7, "PZ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Przyjęcie zewnętrzne", 10, "PZ" },
                    { 8, "WZ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Wydanie zewnętrzne", 20, "WZ" },
                    { 9, "MM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Przesunięcie międzymagazynowe", 30, "MM" },
                    { 10, "RW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Rozchód wewnętrzny", 40, "RW" },
                    { 11, "PW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Przyjęcie wewnętrzne", 50, "PW" },
                    { 12, "INW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "WarehouseDocumentTypes", true, false, null, null, "Inwentaryzacja", 60, "INW" },
                    { 13, "Domestic", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "TransportOrderTypes", true, false, null, null, "Krajowe", 10, "Domestic" },
                    { 14, "International", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "TransportOrderTypes", true, false, null, null, "Międzynarodowe", 20, "International" }
                });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "DictionaryItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Attachments",
                columns: new[] { "Id", "ContentType", "CreatedAt", "CreatedByUserId", "FileName", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "OwnerEntityId", "OwnerEntityName", "SizeBytes", "StoragePath", "UploadedAtUtc", "UploadedByUserId" },
                values: new object[,]
                {
                    { 1, "application/pdf", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "cmr-scan.pdf", false, null, null, 9001, "TransportOrder", 245000L, "/attachments/cmr-scan.pdf", new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 2, "image/jpeg", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "goods-photo.jpg", false, null, null, 9002, "WarehouseGoods", 180250L, "/attachments/goods-photo.jpg", new DateTime(2026, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.InsertData(
                table: "AuditLogs",
                columns: new[] { "Id", "ActionName", "ChangedAtUtc", "ChangedByUserId", "CreatedAt", "CreatedByUserId", "EntityId", "EntityName", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "NewValues", "OldValues", "Summary" },
                values: new object[,]
                {
                    { 1, "Seeded", new DateTime(2026, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "User", false, null, null, "{\"userName\":\"admin\"}", "{}", "Seeded administrator account." },
                    { 2, "Seeded", new DateTime(2026, 1, 1, 2, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, "Role", false, null, null, "{\"role\":\"WAREHOUSE_MANAGER\"}", "{}", "Seeded warehouse role." },
                    { 3, "Seeded", new DateTime(2026, 1, 1, 3, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, "DocumentNumberSequence", false, null, null, "{\"key\":\"TR_ORD\"}", "{}", "Seeded shared transport numbering." }
                });

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

            migrationBuilder.InsertData(
                table: "CurrencyRates",
                columns: new[] { "Id", "BaseCurrencyCode", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "QuoteCurrencyCode", "Rate", "RateDate", "Source" },
                values: new object[,]
                {
                    { 1, "EUR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, "PLN", 4.28m, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBP" },
                    { 2, "USD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, "PLN", 3.91m, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBP" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "Description", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, "LOG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Dział transportu i operacji.", true, false, null, null, "Logistyka" },
                    { 2, "HR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Kadry i administracja pracownicza.", true, false, null, null, "HR" },
                    { 3, "ADM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Obsługa wewnętrzna firmy.", true, false, null, null, "Administracja" }
                });

            migrationBuilder.InsertData(
                table: "DocumentHistoryEntries",
                columns: new[] { "Id", "ActorUserId", "CreatedAt", "CreatedByUserId", "Description", "DocumentId", "DocumentType", "EntryType", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "OccurredAtUtc", "StatusCode" },
                values: new object[,]
                {
                    { 1, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Transport order created.", 9001, "TransportOrder", "Created", false, null, null, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc), "New" },
                    { 2, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Driver assigned to the transport order.", 9001, "TransportOrder", "Assigned", false, null, null, new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Planned" },
                    { 3, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Goods stock was corrected.", 9002, "WarehouseGoods", "Updated", false, null, null, new DateTime(2026, 1, 1, 11, 0, 0, 0, DateTimeKind.Utc), "Active" }
                });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "Email", "EmployeeNumber", "FirstName", "IsActive", "IsDeleted", "LastName", "ModifiedAt", "ModifiedByUserId", "PersonalId", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "anna.krawczyk@erpsystem.local", "EMP-001", "Anna", true, false, "Krawczyk", null, null, "85010112345", "+48 600 101 101" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "jan.nowak@erpsystem.local", "EMP-002", "Jan", true, false, "Nowak", null, null, "87020212345", "+48 600 100 200" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "piotr.kaczmarek@erpsystem.local", "EMP-003", "Piotr", true, false, "Kaczmarek", null, null, "89030312345", "+48 600 300 400" }
                });

            migrationBuilder.InsertData(
                table: "LeaveTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "IsPaid", "ModifiedAt", "ModifiedByUserId", "Name", "RequiresApproval", "SortOrder" },
                values: new object[,]
                {
                    { 1, "ANNUAL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, true, null, null, "Urlop wypoczynkowy", true, 10 },
                    { 2, "SICK", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, true, null, null, "Nieobecność chorobowa", false, 20 },
                    { 3, "UNPAID", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, false, null, null, "Urlop bezpłatny", true, 30 }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "Description", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, "DRIVER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Stanowisko kierowcy zawodowego.", true, false, null, null, "Kierowca" },
                    { 2, "DISPATCHER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Planowanie i nadzór nad transportem.", true, false, null, null, "Dyspozytor" },
                    { 3, "HRSPEC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Obsługa procesów kadrowych.", true, false, null, null, "Specjalista HR" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "MinimumStockLevel", "ModifiedAt", "ModifiedByUserId", "Name", "ProductCategoryId", "Sku", "UnitOfMeasureId" },
                values: new object[,]
                {
                    { 1, "PRD-001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, 20m, null, null, "Płyn do spryskiwaczy", 1, "590000000001", 1 },
                    { 2, "PRD-002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, 10m, null, null, "Pojemnik transportowy", 2, "590000000002", 1 },
                    { 3, "PRD-003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, 12m, null, null, "Folia stretch", 2, "590000000003", 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "DisplayName", "Email", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "System Administrator", "admin@erpsystem.local", true, false, null, null, "admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Warehouse Manager", "warehouse@erpsystem.local", true, false, null, null, "warehouse" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Transport Dispatcher", "transport@erpsystem.local", true, false, null, null, "transport" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "HR Manager", "hr@erpsystem.local", true, false, null, null, "hr" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Finance Manager", "finance@erpsystem.local", true, false, null, null, "finance" }
                });

            migrationBuilder.InsertData(
                table: "Carriers",
                columns: new[] { "Id", "Code", "ContractorId", "CreatedAt", "CreatedByUserId", "IsActive", "IsDeleted", "IsPreferred", "ModifiedAt", "ModifiedByUserId" },
                values: new object[,]
                {
                    { 1, "CAR-TRANS-POL", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, true, null, null },
                    { 2, "CAR-INTER-EU", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, false, null, null }
                });

            migrationBuilder.InsertData(
                table: "DriverProfiles",
                columns: new[] { "Id", "AdrValidUntil", "CreatedAt", "CreatedByUserId", "DriverCardNumber", "DriverCardValidUntil", "EmployeeId", "IsDeleted", "LicenseCategories", "LicenseNumber", "LicenseValidUntil", "ModifiedAt", "ModifiedByUserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "CARD-002", new DateTime(2028, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, false, "C+E", "PL1234567", new DateTime(2030, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "CARD-003", new DateTime(2028, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, false, "C+E", "PL7654321", new DateTime(2031, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDocuments",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "DocumentNumber", "DocumentType", "EmployeeId", "IsActive", "IsDeleted", "IssuedAt", "ModifiedAt", "ModifiedByUserId", "Notes", "Title", "ValidUntil" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "MED-001", "MedicalExam", 2, true, false, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Badania okresowe", new DateTime(2027, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "DRV-QUAL-002", "DriverQualification", 2, true, false, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Świadectwo kwalifikacji", new DateTime(2030, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "DRV-QUAL-003", "DriverQualification", 3, true, false, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "Świadectwo kwalifikacji", new DateTime(2030, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "EmploymentContracts",
                columns: new[] { "Id", "ContractNumber", "ContractType", "CreatedAt", "CreatedByUserId", "DepartmentId", "EmployeeId", "EmploymentRate", "EndDate", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "MonthlySalary", "Notes", "PositionId", "StartDate" },
                values: new object[,]
                {
                    { 1, "CON-001", "Employment", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, 1, 1.0m, null, false, null, null, 9200m, "Prowadzenie procesów HR.", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "CON-002", "Employment", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 2, 1.0m, null, false, null, null, 7800m, "Kierowca krajowy i międzynarodowy.", 1, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "CON-003", "Employment", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, 3, 1.0m, null, false, null, null, 7600m, "Kierowca floty chłodniczej.", 1, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "LeaveRequests",
                columns: new[] { "Id", "ApprovedByUserId", "CreatedAt", "CreatedByUserId", "DateFrom", "DateTo", "DayCount", "DecidedAtUtc", "DecisionNote", "EmployeeId", "IsDeleted", "LeaveTypeId", "ModifiedAt", "ModifiedByUserId", "Reason", "Status" },
                values: new object[,]
                {
                    { 1, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5m, new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Zatwierdzono.", 1, false, 1, null, null, "Planowany urlop letni.", 2 },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 5m, null, null, 2, false, 1, null, null, "Urlop rodzinny.", 1 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 1, 1 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 2, 2 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 3, 3 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 4, 4 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 5, 5 }
                });

            migrationBuilder.InsertData(
                table: "WorkSchedules",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "EffectiveFrom", "EffectiveTo", "EmployeeId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "Name", "Notes", "WeeklyHours" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, false, null, null, "Biurowy 8-16", null, 40m },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, false, null, null, "Kierowcy 4/1", "Cykl 4 tygodnie pracy / 1 tydzień wolnego.", 40m },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, false, null, null, "Kierowcy 4/1", "Cykl 4 tygodnie pracy / 1 tydzień wolnego.", 40m }
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
        }
    }
}
