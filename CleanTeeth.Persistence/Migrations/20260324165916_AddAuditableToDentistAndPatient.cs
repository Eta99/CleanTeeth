using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanTeeth.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableToDentistAndPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Patients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Patients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Dentists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Dentists",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Dentists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Dentists",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Dentists");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Dentists");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Dentists");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Dentists");
        }
    }
}
