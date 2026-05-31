using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErpSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeSeedTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8790));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 4,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 5,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 6,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 7,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 8,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8880));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 9,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8880));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 10,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8880));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 11,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8880));

            migrationBuilder.UpdateData(
                table: "DocumentNumberSequences",
                keyColumn: "Id",
                keyValue: 12,
                column: "LastGeneratedAtUtc",
                value: new DateTime(2026, 5, 15, 12, 0, 34, 191, DateTimeKind.Utc).AddTicks(8880));
        }
    }
}
