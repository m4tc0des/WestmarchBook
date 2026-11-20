using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WestmarchBook.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHasImageToAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                table: "Authors",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasImage",
                table: "Authors");
        }
    }
}
