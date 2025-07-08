﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ong_project.Migrations
{
    /// <inheritdoc />
    public partial class fif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "Courses");
        }
    }
}
