//using Com.DanLiris.Service.Purchasing.Lib.Configs.Expedition;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Moonlay.Data.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Com.Bateeq.Service.Warehouse.Lib
{
    public class WarehouseDbContext : StandardDbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {
        }
        public DbSet<SPKDocs> SPKDocs { get; set; }
        public DbSet<SPKDocsItem> SPKDocsItems { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<TransferInDoc> TransferInDocs { get; set; }
        public DbSet<TransferInDocItem> TransferInDocItems { get; set; }
        public DbSet<TransferOutDoc> TransferOutDocs { get; set; }
        public DbSet<TransferOutDocItem> TransferOutDocItems { get; set; }
        public DbSet<ExpeditionItem> ExpeditionItems { get; set; }
        public DbSet<ExpeditionDetail> ExpeditionDetails { get; set; }
        public DbSet<Expedition> Expeditions { get; set; }
        public DbSet<AdjustmentDocs> AdjustmentDocs { get; set; }
        public DbSet<AdjustmentDocsItem> AdjustmentDocsItems { get; set; }
        public DbSet<SODocs> StockOpnameDocs { get; set; }
        public DbSet<SODocsItem> StockOpnameDocsItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // modelBuilder.ApplyConfiguration(new PurchasingDocumentExpeditionConfig());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            #region Purchasing

            modelBuilder.Entity<SPKDocs>()
                .HasIndex(i => i.PackingList)
                .IsUnique()
                .HasFilter("[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            #endregion
        }
    }
}
