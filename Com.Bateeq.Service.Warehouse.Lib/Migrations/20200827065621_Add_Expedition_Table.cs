using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.Migrations
{
    public partial class Add_Expedition_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpeditionSPKDocItems");

            migrationBuilder.DropTable(
                name: "ExpeditionSPKDocs");

            migrationBuilder.CreateTable(
                name: "Expeditions",
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
                    ExpeditionServiceCode = table.Column<int>(nullable: false),
                    ExpeditionServiceId = table.Column<int>(nullable: false),
                    ExpeditionServiceName = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expeditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpeditionItems",
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
                    DestinationId = table.Column<int>(nullable: false),
                    DestinationName = table.Column<string>(maxLength: 255, nullable: true),
                    ExpeditionId = table.Column<long>(nullable: false),
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
                    SPKDocsId = table.Column<int>(nullable: false),
                    SourceCode = table.Column<string>(maxLength: 255, nullable: true),
                    SourceId = table.Column<int>(nullable: false),
                    SourceName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpeditionItems_Expeditions_ExpeditionId",
                        column: x => x.ExpeditionId,
                        principalTable: "Expeditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpeditionDetails",
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
                    DomesticWholesale = table.Column<double>(nullable: false),
                    ExpeditionItemId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 1000, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SPKDocsId = table.Column<int>(nullable: false),
                    SendQuantity = table.Column<double>(nullable: false),
                    Size = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Uom = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpeditionDetails_ExpeditionItems_ExpeditionItemId",
                        column: x => x.ExpeditionItemId,
                        principalTable: "ExpeditionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionDetails_ExpeditionItemId",
                table: "ExpeditionDetails",
                column: "ExpeditionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionItems_ExpeditionId",
                table: "ExpeditionItems",
                column: "ExpeditionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpeditionDetails");

            migrationBuilder.DropTable(
                name: "ExpeditionItems");

            migrationBuilder.DropTable(
                name: "Expeditions");

            migrationBuilder.CreateTable(
                name: "ExpeditionSPKDocItems",
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
                    DomesticWholesale = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 255, nullable: true),
                    ItemId = table.Column<long>(nullable: false),
                    ItemName = table.Column<string>(maxLength: 1000, nullable: true),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SPKDocsId = table.Column<int>(nullable: false),
                    SendQuantity = table.Column<double>(nullable: false),
                    Size = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Uom = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionSPKDocItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpeditionSPKDocs",
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
                    DestinationId = table.Column<int>(nullable: false),
                    DestinationName = table.Column<string>(maxLength: 255, nullable: true),
                    ExpeditionId = table.Column<int>(nullable: false),
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
                    SPKDocsId = table.Column<int>(nullable: false),
                    SourceCode = table.Column<string>(maxLength: 255, nullable: true),
                    SourceId = table.Column<int>(nullable: false),
                    SourceName = table.Column<string>(maxLength: 255, nullable: true),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionSPKDocs", x => x.Id);
                });
        }
    }
}
