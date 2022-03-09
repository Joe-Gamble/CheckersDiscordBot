// <copyright file="20220220182112_UpdatedPlayer.cs" company="GambleDev">
// Copyright (c) GambleDev. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checkers.Data.Migrations
{
    public partial class UpdatedPlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Registered",
                table: "Players",
                newName: "IsQueued");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Players",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlaying",
                table: "Players",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsPlaying",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "IsQueued",
                table: "Players",
                newName: "Registered");
        }
    }
}
