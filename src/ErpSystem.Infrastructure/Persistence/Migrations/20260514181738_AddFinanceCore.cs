using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractorId = table.Column<int>(type: "int", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExternalNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDocuments_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaseCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    QuoteCurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractorId = table.Column<int>(type: "int", nullable: false),
                    TransportOrderId = table.Column<int>(type: "int", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    ExternalNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    ContractorId = table.Column<int>(type: "int", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
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
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostDocumentId = table.Column<int>(type: "int", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    CostCategory = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    TransportOrderId = table.Column<int>(type: "int", nullable: true),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: true),
                    ContractorId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_CostPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostPositions_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostPositions_CostDocuments_CostDocumentId",
                        column: x => x.CostDocumentId,
                        principalTable: "CostDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostPositions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostPositions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostPositions_TransportOrders_TransportOrderId",
                        column: x => x.TransportOrderId,
                        principalTable: "TransportOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostPositions_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostPositions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
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
                    table.PrimaryKey("PK_InvoicePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePositions_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CostDocumentId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_Settlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settlements_CostDocuments_CostDocumentId",
                        column: x => x.CostDocumentId,
                        principalTable: "CostDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CurrencyRates",
                columns: new[] { "Id", "BaseCurrencyCode", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "QuoteCurrencyCode", "Rate", "RateDate", "Source" },
                values: new object[,]
                {
                    { 1, "EUR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, "PLN", 4.28m, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBP" },
                    { 2, "USD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, "PLN", 3.91m, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "NBP" }
                });

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4770));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920));

            migrationBuilder.InsertData(
                table: "DocumentNumberSequences",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "CurrentNumber", "IsDeleted", "Key", "LastGeneratedAtUtc", "ModifiedAt", "ModifiedByUserId", "Padding", "Prefix", "ResetPolicy" },
                values: new object[,]
                {
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, false, "FIN_COST", new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920), null, null, 5, "KD", 2 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, false, "FIN_INV", new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920), null, null, 5, "FV", 2 }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "CreatedByUserId", "Description", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "ModuleKey", "Name", "Resource" },
                values: new object[,]
                {
                    { 53, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read cost documents.", false, null, null, "finance", "Finance.Cost.Read", "Cost" },
                    { 54, "Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Create cost documents.", false, null, null, "finance", "Finance.Cost.Create", "Cost" },
                    { 55, "Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Edit cost documents.", false, null, null, "finance", "Finance.Cost.Edit", "Cost" },
                    { 56, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read invoices.", false, null, null, "finance", "Finance.Invoice.Read", "Invoice" },
                    { 57, "Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Create invoices.", false, null, null, "finance", "Finance.Invoice.Create", "Invoice" },
                    { 58, "Edit", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Edit invoices.", false, null, null, "finance", "Finance.Invoice.Edit", "Invoice" },
                    { 59, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read payments.", false, null, null, "finance", "Finance.Payment.Read", "Payment" },
                    { 60, "Create", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Create payments.", false, null, null, "finance", "Finance.Payment.Create", "Payment" },
                    { 61, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read currency rates.", false, null, null, "finance", "Finance.CurrencyRate.Read", "CurrencyRate" },
                    { 62, "Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Manage currency rates.", false, null, null, "finance", "Finance.CurrencyRate.Manage", "CurrencyRate" },
                    { 63, "Read", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Read settlements.", false, null, null, "finance", "Finance.Settlement.Read", "Settlement" },
                    { 64, "Manage", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Manage settlements.", false, null, null, "finance", "Finance.Settlement.Manage", "Settlement" }
                });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 53, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 54, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 55, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 56, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 57, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 58, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 59, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 60, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 61, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 62, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 63, 1 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 64, 1 });

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
                value: 13);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "PermissionId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 17, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 18, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 24, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 25, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 26, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 27, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 28, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 29, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 30, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 31, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 13, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 32, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 33, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 34, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 35, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 36, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "PermissionId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "PermissionId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "PermissionId",
                value: 37);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "PermissionId",
                value: 38);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "PermissionId",
                value: 39);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "PermissionId",
                value: 40);

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 96, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 41, 4 },
                    { 97, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 42, 4 },
                    { 98, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 43, 4 },
                    { 99, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 44, 4 },
                    { 100, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 45, 4 },
                    { 101, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 46, 4 },
                    { 102, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 47, 4 },
                    { 103, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 48, 4 },
                    { 104, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 49, 4 },
                    { 105, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 50, 4 },
                    { 106, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 51, 4 },
                    { 107, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 52, 4 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "Description", "IsDeleted", "IsSystem", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[] { 5, "FINANCE_MANAGER", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Costs, invoices, payments and settlements.", false, false, null, null, "Finance manager" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "DisplayName", "Email", "IsActive", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "UserName" },
                values: new object[] { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Finance Manager", "finance@erpsystem.local", true, false, null, null, "finance" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 108, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 9, 5 },
                    { 109, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 16, 5 },
                    { 110, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 17, 5 },
                    { 111, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 18, 5 },
                    { 112, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 19, 5 },
                    { 113, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 32, 5 },
                    { 114, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 37, 5 },
                    { 115, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 40, 5 },
                    { 116, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 31, 5 },
                    { 117, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 53, 5 },
                    { 118, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 54, 5 },
                    { 119, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 55, 5 },
                    { 120, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 56, 5 },
                    { 121, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 57, 5 },
                    { 122, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 58, 5 },
                    { 123, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 59, 5 },
                    { 124, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 60, 5 },
                    { 125, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 61, 5 },
                    { 126, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 62, 5 },
                    { 127, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 63, 5 },
                    { 128, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 64, 5 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "IsDeleted", "ModifiedAt", "ModifiedByUserId", "RoleId", "UserId" },
                values: new object[] { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, null, null, 5, 5 });

            migrationBuilder.CreateIndex(
                name: "IX_CostDocuments_ContractorId",
                table: "CostDocuments",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDocuments_Number",
                table: "CostDocuments",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_ContractorId",
                table: "CostPositions",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_CostDocumentId_LineNumber",
                table: "CostPositions",
                columns: new[] { "CostDocumentId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_DepartmentId",
                table: "CostPositions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_EmployeeId",
                table: "CostPositions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_TransportOrderId",
                table: "CostPositions",
                column: "TransportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_VehicleId",
                table: "CostPositions",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_CostPositions_WarehouseId",
                table: "CostPositions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_RateDate_BaseCurrencyCode_QuoteCurrencyCode",
                table: "CurrencyRates",
                columns: new[] { "RateDate", "BaseCurrencyCode", "QuoteCurrencyCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePositions_InvoiceId_LineNumber",
                table: "InvoicePositions",
                columns: new[] { "InvoiceId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ContractorId",
                table: "Invoices",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Number",
                table: "Invoices",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TransportOrderId",
                table: "Invoices",
                column: "TransportOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ContractorId",
                table: "Payments",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_CostDocumentId",
                table: "Settlements",
                column: "CostDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_InvoiceId",
                table: "Settlements",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_PaymentId",
                table: "Settlements",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostPositions");

            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "InvoicePositions");

            migrationBuilder.DropTable(
                name: "Settlements");

            migrationBuilder.DropTable(
                name: "CostDocuments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DeleteData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8340));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8430));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8430));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8430));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 14, 55, 37, 694, DateTimeKind.Utc).AddTicks(8440));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 13, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 17, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 18, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 24, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 25, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 26, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 27, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 28, 2 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "PermissionId",
                value: 29);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "PermissionId",
                value: 30);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "PermissionId",
                value: 31);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 13, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 14, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 19, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 32, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 33, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 34, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 35, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 36, 3 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 9, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 16, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 37, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 38, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 39, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 40, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 41, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 42, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 43, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 44, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 45, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 46, 4 });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "PermissionId",
                value: 47);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "PermissionId",
                value: 48);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "PermissionId",
                value: 49);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "PermissionId",
                value: 50);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "PermissionId",
                value: 51);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "PermissionId",
                value: 52);
        }
    }
}
