using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.Migrations
{
    public partial class create_adjustment_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdjustmentDocs",
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
                    table.PrimaryKey("PK_AdjustmentDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdjustmentDocsItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    AdjustmentDocsId = table.Column<long>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
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
                    QtyAdjustment = table.Column<double>(nullable: false),
                    QtyBeforeAdjustment = table.Column<double>(maxLength: 1000, nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Type = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustmentDocsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdjustmentDocsItems_AdjustmentDocs_AdjustmentDocsId",
                        column: x => x.AdjustmentDocsId,
                        principalTable: "AdjustmentDocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentDocsItems_AdjustmentDocsId",
                table: "AdjustmentDocsItems",
                column: "AdjustmentDocsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjustmentDocsItems");

            migrationBuilder.DropTable(
                name: "AdjustmentDocs");
        }
    }
}
