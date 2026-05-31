using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseProductMinimumAndBootstrap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumStockLevel",
                table: "Products",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3880));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 7, 57, 9, 303, DateTimeKind.Utc).AddTicks(3970));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "MinimumStockLevel",
                value: 20m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "MinimumStockLevel",
                value: 10m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "MinimumStockLevel",
                value: 12m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumStockLevel",
                table: "Products");

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

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 14, 18, 17, 38, 337, DateTimeKind.Utc).AddTicks(4920));
        }
    }
}
