using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
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
                    ItemSize = table.Column<string>(maxLength: 255, nullable: true),
                    ItemUom = table.Column<string>(maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageIsCentral = table.Column<bool>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryMovements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    After = table.Column<double>(nullable: false),
                    Before = table.Column<double>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
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
                    ItemSize = table.Column<string>(maxLength: 255, nullable: true),
                    ItemUom = table.Column<string>(maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Reference = table.Column<string>(maxLength: 255, nullable: true),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageIsCentral = table.Column<bool>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 255, nullable: true),
                    Type = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMovements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPKDocs",
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
                    DestinationCode = table.Column<string>(maxLength: 255, nullable: true),
                    DestinationId = table.Column<long>(nullable: false),
                    DestinationName = table.Column<string>(maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsDistributed = table.Column<bool>(nullable: false),
                    IsDraft = table.Column<bool>(nullable: false),
                    IsReceived = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    PackingList = table.Column<string>(maxLength: 255, nullable: true),
                    Password = table.Column<string>(maxLength: 255, nullable: true),
                    Reference = table.Column<string>(maxLength: 255, nullable: true),
                    SourceCode = table.Column<string>(maxLength: 255, nullable: true),
                    SourceId = table.Column<long>(nullable: false),
                    SourceName = table.Column<string>(maxLength: 1000, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPKDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferInDocs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DestinationCode = table.Column<string>(maxLength: 255, nullable: true),
                    DestinationId = table.Column<long>(nullable: false),
                    DestinationName = table.Column<string>(maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 255, nullable: true),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    SourceCode = table.Column<string>(maxLength: 255, nullable: true),
                    SourceId = table.Column<long>(nullable: false),
                    SourceName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferInDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferOutDocs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DestinationCode = table.Column<string>(maxLength: 255, nullable: true),
                    DestinationId = table.Column<long>(nullable: false),
                    DestinationName = table.Column<string>(maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 255, nullable: true),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    SourceCode = table.Column<string>(maxLength: 255, nullable: true),
                    SourceId = table.Column<long>(nullable: false),
                    SourceName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferOutDocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPKDocsItems",
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
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemArticleRealizationOrder = table.Column<string>(maxLength: 255, nullable: true),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemDomesticCOGS = table.Column<double>(nullable: false),
                    ItemDomesticRetail = table.Column<double>(nullable: false),
                    ItemDomesticSale = table.Column<double>(nullable: false),
                    ItemDomesticWholesale = table.Column<double>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 1000, nullable: true),
                    ItemSize = table.Column<string>(maxLength: 255, nullable: true),
                    ItemUom = table.Column<string>(maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SPKDocsId = table.Column<long>(nullable: false),
                    SendQuantity = table.Column<double>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPKDocsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPKDocsItems_SPKDocs_SPKDocsId",
                        column: x => x.SPKDocsId,
                        principalTable: "SPKDocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferInDocItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    ArticleRealizationOrder = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DomesticCOGS = table.Column<double>(nullable: false),
                    DomesticRetail = table.Column<double>(nullable: false),
                    DomesticSale = table.Column<double>(nullable: false),
                    DomesticWholeSale = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    Size = table.Column<string>(nullable: true),
                    TransferDocsId = table.Column<long>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferInDocItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferInDocItems_TransferInDocs_TransferDocsId",
                        column: x => x.TransferDocsId,
                        principalTable: "TransferInDocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferOutDocItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    ArticleRealizationOrder = table.Column<string>(maxLength: 255, nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DomesticCOGS = table.Column<double>(nullable: false),
                    DomesticRetail = table.Column<double>(nullable: false),
                    DomesticSale = table.Column<double>(nullable: false),
                    DomesticWholeSale = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(maxLength: 1000, nullable: true),
                    Size = table.Column<string>(nullable: true),
                    TransferOutDocsId = table.Column<long>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Uom = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferOutDocItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferOutDocItems_TransferOutDocs_TransferOutDocsId",
                        column: x => x.TransferOutDocsId,
                        principalTable: "TransferOutDocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPKDocs_PackingList",
                table: "SPKDocs",
                column: "PackingList",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_SPKDocsItems_SPKDocsId",
                table: "SPKDocsItems",
                column: "SPKDocsId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferInDocItems_TransferDocsId",
                table: "TransferInDocItems",
                column: "TransferDocsId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferOutDocItems_TransferOutDocsId",
                table: "TransferOutDocItems",
                column: "TransferOutDocsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "InventoryMovements");

            migrationBuilder.DropTable(
                name: "SPKDocsItems");

            migrationBuilder.DropTable(
                name: "TransferInDocItems");

            migrationBuilder.DropTable(
                name: "TransferOutDocItems");

            migrationBuilder.DropTable(
                name: "SPKDocs");

            migrationBuilder.DropTable(
                name: "TransferInDocs");

            migrationBuilder.DropTable(
                name: "TransferOutDocs");
        }
    }
}
