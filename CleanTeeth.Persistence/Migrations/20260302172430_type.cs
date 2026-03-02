using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanTeeth.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            // Single batch: ensure Types has Id=1 so FK from Actions can reference it
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT [dbo].[Types] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Types] WHERE [Id] = 1)
    INSERT INTO [dbo].[Types] ([Id], [Name]) VALUES (1, N'Default');
SET IDENTITY_INSERT [dbo].[Types] OFF;
            ".Trim());

            migrationBuilder.AddColumn<long>(
                name: "TypeId",
                table: "Actions",
                type: "bigint",
                nullable: true);

            // All rows must reference existing Types.Id before we add FK
            migrationBuilder.Sql(@"
UPDATE [dbo].[Actions] SET [TypeId] = 1 WHERE [TypeId] IS NULL;
UPDATE [dbo].[Actions] SET [TypeId] = 1 WHERE [TypeId] NOT IN (SELECT [Id] FROM [dbo].[Types]);
            ".Trim());

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
