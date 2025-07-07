using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Api.Migrations
{
    /// <inheritdoc />
    public partial class removeisvalid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Friends");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
