using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elo.Api.Migrations;

/// <inheritdoc />
public partial class _20261124194020_AddUniqueMatchExternalId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Matches_Teams_BlueTeamId",
            table: "Matches");

        migrationBuilder.DropForeignKey(
            name: "FK_Matches_Teams_RedTeamId",
            table: "Matches");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Players_AttackerId",
            table: "Teams");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Players_DefenderId",
            table: "Teams");

        migrationBuilder.DropIndex(
            name: "IX_Teams_AttackerId",
            table: "Teams");

        migrationBuilder.AddColumn<Guid>(
            name: "ExternalId",
            table: "Matches",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex(
            name: "IX_Teams_AttackerId_DefenderId",
            table: "Teams",
            columns: new[] { "AttackerId", "DefenderId" },
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Matches_Teams_BlueTeamId",
            table: "Matches",
            column: "BlueTeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Matches_Teams_RedTeamId",
            table: "Matches",
            column: "RedTeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Players_AttackerId",
            table: "Teams",
            column: "AttackerId",
            principalTable: "Players",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Players_DefenderId",
            table: "Teams",
            column: "DefenderId",
            principalTable: "Players",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Matches_Teams_BlueTeamId",
            table: "Matches");

        migrationBuilder.DropForeignKey(
            name: "FK_Matches_Teams_RedTeamId",
            table: "Matches");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Players_AttackerId",
            table: "Teams");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Players_DefenderId",
            table: "Teams");

        migrationBuilder.DropIndex(
            name: "IX_Teams_AttackerId_DefenderId",
            table: "Teams");

        migrationBuilder.DropColumn(
            name: "ExternalId",
            table: "Matches");

        migrationBuilder.CreateIndex(
            name: "IX_Teams_AttackerId",
            table: "Teams",
            column: "AttackerId");

        migrationBuilder.AddForeignKey(
            name: "FK_Matches_Teams_BlueTeamId",
            table: "Matches",
            column: "BlueTeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Matches_Teams_RedTeamId",
            table: "Matches",
            column: "RedTeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Players_AttackerId",
            table: "Teams",
            column: "AttackerId",
            principalTable: "Players",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Players_DefenderId",
            table: "Teams",
            column: "DefenderId",
            principalTable: "Players",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
