﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Posts.Api.Migrations
{
    /// <inheritdoc />
    public partial class statusaddedtofriends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friends");
        }
    }
}
