using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanTeeth.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Create Types first and insert row Id=1 so Actions can reference it
            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.Id);
                });

            migrationBuilder.Sql(@"
SET IDENTITY_INSERT [dbo].[Types] ON;
INSERT INTO [dbo].[Types] ([Id], [Name]) VALUES (1, N'Default');
SET IDENTITY_INSERT [dbo].[Types] OFF;
            ".Trim());

            // 2) Add column as nullable, then set all to 1, then make NOT NULL
            migrationBuilder.AddColumn<long>(
                name: "TypeId",
                table: "Actions",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Actions] SET [TypeId] = 1 WHERE [TypeId] IS NULL;");

            migrationBuilder.AlterColumn<long>(
                name: "TypeId",
                table: "Actions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actions_TypeId",
                table: "Actions",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Types_TypeId",
                table: "Actions",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Types_TypeId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_TypeId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Actions");

            migrationBuilder.DropTable(
                name: "Types");
        }
    }
}
