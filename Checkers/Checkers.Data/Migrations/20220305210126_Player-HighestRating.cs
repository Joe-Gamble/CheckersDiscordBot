// <copyright file="20220305210126_Player-HighestRating.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checkers.Data.Migrations
{
    public partial class PlayerHighestRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HighestRating",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighestRating",
                table: "Players");
        }
    }
}
