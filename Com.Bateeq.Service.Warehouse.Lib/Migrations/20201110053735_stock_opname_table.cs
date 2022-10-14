using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.Migrations
{
    public partial class stock_opname_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockOpnameDocs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsProcess = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOpnameDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockOpnameDocsItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    IsAdjusted = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemArticleRealizationOrder = table.Column<string>(maxLength: 255, nullable: true),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemDomesticCOGS = table.Column<double>(nullable: false),
                    ItemDomesticRetail = table.Column<double>(nullable: false),
                    ItemDomesticSale = table.Column<double>(nullable: false),
                    ItemDomesticWholeSale = table.Column<double>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    ItemInternationalCOGS = table.Column<double>(nullable: false),
                    ItemInternationalRetail = table.Column<double>(nullable: false),
                    ItemInternationalSale = table.Column<double>(nullable: false),
                    ItemInternationalWholeSale = table.Column<double>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 255, nullable: true),
                    ItemSize = table.Column<string>(nullable: true),
                    ItemUom = table.Column<string>(nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    QtyBeforeSO = table.Column<double>(maxLength: 1000, nullable: false),
                    QtySO = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SODocsId = table.Column<long>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOpnameDocsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockOpnameDocsItems_StockOpnameDocs_SODocsId",
                        column: x => x.SODocsId,
                        principalTable: "StockOpnameDocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockOpnameDocsItems_SODocsId",
                table: "StockOpnameDocsItems",
                column: "SODocsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockOpnameDocsItems");

            migrationBuilder.DropTable(
                name: "StockOpnameDocs");
        }
    }
}
