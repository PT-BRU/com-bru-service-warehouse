using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.Migrations
{
    public partial class delete_date_stockopname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "StockOpnameDocs");

            migrationBuilder.RenameColumn(
                name: "IsProcess",
                table: "StockOpnameDocs",
                newName: "IsProcessed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProcessed",
                table: "StockOpnameDocs",
                newName: "IsProcess");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                table: "StockOpnameDocs",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
