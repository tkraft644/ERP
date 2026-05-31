using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contractors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Types = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Contractors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractorAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApartmentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorAddresses_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractorBankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Swift = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorBankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorBankAccounts_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractorContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorContacts_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractorNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorNotes_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 30, 58, 816, DateTimeKind.Utc).AddTicks(4850));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 30, 58, 816, DateTimeKind.Utc).AddTicks(4930));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 30, 58, 816, DateTimeKind.Utc).AddTicks(4930));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 30, 58, 816, DateTimeKind.Utc).AddTicks(4930));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read contractors.", "contractors", "Contractors.Contractor.Read", "Contractor" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Create contractors.", "contractors", "Contractors.Contractor.Create", "Contractor" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Edit contractors.", "contractors", "Contractors.Contractor.Edit", "Contractor" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "ChangeStatus", "Change contractor activity status.", "contractors", "Contractors.Contractor.ChangeStatus", "Contractor" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "ViewHistory", "Read contractor change history.", "contractors", "Contractors.Contractor.ViewHistory", "Contractor" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read", "Read warehouse goods.", "warehouse", "Warehouse.Goods.Read", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Create", "Create warehouse goods.", "warehouse", "Warehouse.Goods.Create", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Edit", "Edit warehouse goods.", "warehouse", "Warehouse.Goods.Edit", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Delete", "Delete warehouse goods.", "warehouse", "Warehouse.Goods.Delete", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read", "Read transport orders.", "transport", "Transport.Order.Read", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Create", "Create transport orders.", "transport", "Transport.Order.Create", "Order" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "CreatedByUserId", "Description", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "ModuleKey", "Name", "Resource" },
                values: new object[,]
                {
                    { 30, "ChangeStatus", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Change transport order status.", false, null, null, "transport", "Transport.Order.ChangeStatus", "Order" },
                    { 31, "AssignDriver", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Assign driver to transport order.", false, null, null, "transport", "Transport.Order.AssignDriver", "Order" },
                    { 32, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read employee cards.", false, null, null, "hr", "HR.Employee.Read", "Employee" },
                    { 33, "Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Edit employee cards.", false, null, null, "hr", "HR.Employee.Edit", "Employee" },
                    { 34, "Approve", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Approve leave requests.", false, null, null, "hr", "HR.Leave.Approve", "Leave" }
                });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 30, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 31, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 32, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 33, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 34, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "PermissionId",
                value: 17);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 18, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 24, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 25, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 26, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 27, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "PermissionId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "RoleId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "RoleId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "RoleId",
                value: 3);

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 55, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 9, 4 },
                    { 56, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 16, 4 },
                    { 53, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 30, 3 },
                    { 54, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 31, 3 },
                    { 57, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 32, 4 },
                    { 58, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 33, 4 },
                    { 59, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 34, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractorAddresses_ContractorId",
                table: "ContractorAddresses",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorBankAccounts_ContractorId",
                table: "ContractorBankAccounts",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorContacts_ContractorId",
                table: "ContractorContacts",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorNotes_ContractorId",
                table: "ContractorNotes",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Code",
                table: "Contractors",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractorAddresses");

            migrationBuilder.DropTable(
                name: "ContractorBankAccounts");

            migrationBuilder.DropTable(
                name: "ContractorContacts");

            migrationBuilder.DropTable(
                name: "ContractorNotes");

            migrationBuilder.DropTable(
                name: "Contractors");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 18, 55, 988, DateTimeKind.Utc).AddTicks(1250));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 18, 55, 988, DateTimeKind.Utc).AddTicks(1330));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 3,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 18, 55, 988, DateTimeKind.Utc).AddTicks(1330));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 13, 18, 55, 988, DateTimeKind.Utc).AddTicks(1330));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read warehouse goods.", "warehouse", "Warehouse.Goods.Read", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Create warehouse goods.", "warehouse", "Warehouse.Goods.Create", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Edit warehouse goods.", "warehouse", "Warehouse.Goods.Edit", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Delete", "Delete warehouse goods.", "warehouse", "Warehouse.Goods.Delete", "Goods" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read", "Read transport orders.", "transport", "Transport.Order.Read", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Create", "Create transport orders.", "transport", "Transport.Order.Create", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "ChangeStatus", "Change transport order status.", "transport", "Transport.Order.ChangeStatus", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "AssignDriver", "Assign driver to transport order.", "transport", "Transport.Order.AssignDriver", "Order" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Read", "Read employee cards.", "hr", "HR.Employee.Read", "Employee" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Edit", "Edit employee cards.", "hr", "HR.Employee.Edit", "Employee" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Action", "Description", "ModuleKey", "Name", "Resource" },
                values: new object[] { "Approve", "Approve leave requests.", "hr", "HR.Leave.Approve", "Leave" });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 13, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 17, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "PermissionId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "PermissionId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "PermissionId",
                value: 20);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "PermissionId",
                value: 21);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "PermissionId",
                value: 22);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 13, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 23, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 24, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "PermissionId",
                value: 25);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "PermissionId",
                value: 26);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "RoleId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 27, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "RoleId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "RoleId",
                value: 4);
        }
    }
}
