using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_03.Migrations
{
    /// <inheritdoc />
    public partial class add_libraryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LibraryId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "AspNetUsers");
        }
    }
}
