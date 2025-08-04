using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Api.Migrations
{
    /// <inheritdoc />
    public partial class isvalidaddedtofollower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Followers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Followers");
        }
    }
}
