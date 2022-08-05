using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.ReturnToCenterInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades.Stores
{
    public class ReturnToCenterFacade : IReturnToCenter
	{
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<TransferOutDoc> dbSet;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;
        private readonly DbSet<SPKDocs> dbSetSPKDocs;
        private readonly DbSet<Expedition> dbSetExpedition;
        private readonly IServiceProvider serviceProvider;

        public ReturnToCenterFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TransferOutDoc>();
            this.dbSetSPKDocs = dbContext.Set<SPKDocs>();
            this.dbSetInventoryMovement = dbContext.Set<InventoryMovement>();
            this.dbSetExpedition = dbContext.Set<Expedition>();
        }

        public Tuple<List<TransferOutReadViewModel>, int, Dictionary<string, string>> ReadForRetur(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<TransferOutReadViewModel> Query = (from a in dbContext.TransferOutDocs
                                                         //join b in dbContext.TransferOutDocItems on a.Id equals b.TransferOutDocsId
                                                         //join c in dbContext.SPKDocs on a.Code equals c.Reference
                                                         //join d in dbContext.SPKDocsItems on c.Id equals d.SPKDocsId
                                                         join f in dbContext.ExpeditionItems on a.Code equals f.Reference
                                                         join g in dbContext.Expeditions on f.ExpeditionId equals g.Id
                                                         where a.Code.Contains("BTQ-KB/RTP")
                                                         select new TransferOutReadViewModel
                                                         {
                                                             _id = (int)a.Id,
                                                             code = a.Code,
                                                             date = a.Date ,
                                                             destination = new ViewModels.NewIntegrationViewModel.DestinationViewModel
                                                             {
                                                                 code = a.DestinationCode,
                                                                 name = a.DestinationName,
                                                                 _id = a.DestinationId
                                                             },
                                                             source = new ViewModels.NewIntegrationViewModel.SourceViewModel
                                                             {
                                                                 code = a.SourceCode,
                                                                 name = a.SourceName,
                                                                 _id = a.SourceId
                                                             },
                                                             expeditionService = new ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                                                             {
                                                                 code = g.ExpeditionServiceCode,
                                                                 name = g.ExpeditionServiceName,
                                                                 _id = g.ExpeditionServiceId
                                                             },
                                                             isReceived = f.IsReceived,
                                                             packingList = f.PackingList,
                                                             password = f.Password,
                                                             reference = a.Reference,
                                                             createdby = a.CreatedBy

                                                         }).OrderByDescending(x => x.date);
			IQueryable<TransferOutDoc> QueryDoc = this.dbSet.Include(m => m.Items).OrderByDescending(m => m.Date);

			List<string> searchAttributes = new List<string>()
			{
				"Code","SourceName","DestinationName"
			};

			QueryDoc = QueryHelper<TransferOutDoc>.ConfigureSearch(QueryDoc, searchAttributes, Keyword);
			List<long> listID = new List<long>();
			foreach(var item in QueryDoc)
			{
				listID.Add(item.Id);
			}

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //Query = QueryHelper<TransferOutDoc>.ConfigureOrder(Query, OrderDictionary);

			Query = Query.Where(x => listID.Any(y => y == x._id));
            Pageable<TransferOutReadViewModel> pageable = new Pageable<TransferOutReadViewModel>(Query, Page - 1, Size);
            List<TransferOutReadViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);

        }
        public string GenerateCode(string ModuleId)
        {
            var uid = ObjectId.GenerateNewId().ToString();
            var hashids = new Hashids(uid, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            var now = DateTime.Now;
            var begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var diff = (now - begin).Milliseconds;
            string code = String.Format("{0}/{1}/{2}", hashids.Encode(diff), ModuleId, DateTime.Now.ToString("MM/yyyy"));
            return code;
        }
        public async Task<int> Create(TransferOutDocViewModel model, TransferOutDoc model2, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    string codeOut = GenerateCode("BTQ-KB/RTP");
                    model2.Code = codeOut;
                    model2.Date = DateTimeOffset.Now;
                    List<ExpeditionItem> expeditionItems = new List<ExpeditionItem>();
                    List<ExpeditionDetail> expeditionDetails = new List<ExpeditionDetail>();
                    List<SPKDocsItem> sPKDocsItem = new List<SPKDocsItem>();
                    EntityExtension.FlagForCreate(model2, username, USER_AGENT);
                    foreach (var i in model2.Items)
                    {
                        sPKDocsItem.Add(new SPKDocsItem
                        {
                            ItemArticleRealizationOrder = i.ArticleRealizationOrder,
                            ItemCode = i.ItemCode,
                            ItemDomesticCOGS = i.DomesticCOGS,
                            ItemDomesticRetail = i.DomesticRetail,
                            ItemDomesticSale = i.DomesticSale,
                            ItemDomesticWholesale = i.DomesticWholeSale,
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            ItemSize = i.Size,
                            ItemUom = i.Uom,
                            Quantity = i.Quantity,
                            Remark = i.Remark,
                            SendQuantity = i.Quantity
                        });
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }

                    dbSet.Add(model2);
                    //Created = await dbContext.SaveChangesAsync();

                    SPKDocs sPKDocs = new SPKDocs
                    {
                        Code = GenerateCode("BTQ-PK/PBJ"),
                        Date = DateTimeOffset.Now,
                        IsDistributed = true,
                        IsDraft = false,
                        IsReceived = false,
                        DestinationCode = model2.DestinationCode,
                        DestinationId = model2.DestinationId,
                        DestinationName = model2.DestinationName,
                        PackingList = GenerateCode("BTQ-KB/PLR"),
                        Password = String.Join("", GenerateCode(DateTime.Now.ToString("dd")).Split("/")),
                        Reference = codeOut,
                        SourceCode = model2.SourceCode,
                        SourceName = model2.SourceName,
                        SourceId = model2.SourceId,
                        Weight = 0,
                        Items = sPKDocsItem
                    };
                    EntityExtension.FlagForCreate(sPKDocs, username, USER_AGENT);
                    foreach (var i in sPKDocs.Items)
                    {
                        var inventorymovement = new InventoryMovement();
                        var inven = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == model2.SourceId).FirstOrDefault();
                        if (inven != null)
                        {
                            inventorymovement.Before = inven.Quantity;
                            inven.Quantity = inven.Quantity - i.Quantity;
                        }
                        inventorymovement.After = inventorymovement.Before - i.Quantity;
                        inventorymovement.Date = DateTimeOffset.UtcNow;
                        inventorymovement.ItemCode = i.ItemCode;
                        inventorymovement.ItemDomesticCOGS = i.ItemDomesticCOGS;
                        inventorymovement.ItemDomesticRetail = i.ItemDomesticRetail;
                        inventorymovement.ItemDomesticWholeSale = i.ItemDomesticRetail;
                        inventorymovement.ItemDomesticSale = i.ItemDomesticSale;
                        inventorymovement.ItemId = i.ItemId;
                        inventorymovement.ItemInternationalCOGS = 0;
                        inventorymovement.ItemInternationalRetail = 0;
                        inventorymovement.ItemInternationalSale = 0;
                        inventorymovement.ItemInternationalWholeSale = 0;
                        inventorymovement.ItemName = i.ItemName;
                        inventorymovement.ItemSize = i.ItemSize;
                        inventorymovement.ItemUom = i.ItemUom;
                        inventorymovement.Quantity = i.Quantity;
                        inventorymovement.StorageCode = model2.SourceCode;
                        inventorymovement.StorageId = model2.SourceId;
                        inventorymovement.StorageName = model2.SourceName;
                        inventorymovement.Type = "OUT";
                        inventorymovement.Reference = codeOut;
                        inventorymovement.Remark = model2.Remark;
                        inventorymovement.StorageIsCentral = model2.SourceName.Contains("GUDANG") ? true : false;
                        EntityExtension.FlagForCreate(inventorymovement, username, USER_AGENT);
                        dbSetInventoryMovement.Add(inventorymovement);

                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    dbSetSPKDocs.Add(sPKDocs);
                    Created = await dbContext.SaveChangesAsync();

                    foreach (var i in sPKDocs.Items)
                    {
                        expeditionDetails.Add(new ExpeditionDetail
                        {
                            ArticleRealizationOrder = i.ItemArticleRealizationOrder,
                            DomesticCOGS = i.ItemDomesticCOGS,
                            DomesticRetail = i.ItemDomesticRetail,
                            DomesticSale = i.ItemDomesticSale,
                            DomesticWholesale = i.ItemDomesticWholesale,
                            ItemCode = i.ItemCode,
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            Quantity = i.Quantity,
                            Remark = i.Remark,
                            SendQuantity = i.SendQuantity,
                            Uom = i.ItemUom,
                            Size = i.ItemSize,
                            //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1
                            SPKDocsId = (int)sPKDocs.Id
                        });
                    }

                    expeditionItems.Add(new ExpeditionItem
                    {
                        Code = sPKDocs.Code,
                        Date = sPKDocs.Date,
                        DestinationCode = sPKDocs.DestinationCode,
                        DestinationId = (int)sPKDocs.DestinationId,
                        DestinationName = sPKDocs.DestinationName,
                        IsDistributed = sPKDocs.IsDistributed,
                        IsDraft = sPKDocs.IsDraft,
                        IsReceived = sPKDocs.IsReceived,
                        PackingList = sPKDocs.PackingList,
                        Password = sPKDocs.Password,
                        Reference = sPKDocs.Reference,
                        SourceCode = sPKDocs.SourceCode,
                        SourceId = (int)sPKDocs.SourceId,
                        SourceName = sPKDocs.SourceName,
                        //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        SPKDocsId = (int)sPKDocs.Id,
                        Weight = sPKDocs.Weight,
                        Details = expeditionDetails
                    });

                    Expedition expedition = new Expedition
                    {
                        Code = GenerateCode("BTQ-KB/EXP"),
                        Date = DateTimeOffset.Now,
                        ExpeditionServiceCode = model.expeditionService.code,
                        ExpeditionServiceId = (int)model.expeditionService._id,
                        ExpeditionServiceName = model.expeditionService.name,
                        Remark = "",
                        Weight = 0,
                        Items = expeditionItems,

                    };
                    EntityExtension.FlagForCreate(expedition, username, USER_AGENT);
                    foreach (var i in expeditionItems)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        foreach (var d in expeditionDetails)
                        {
                            EntityExtension.FlagForCreate(d, username, USER_AGENT);
                        }
                    }

                    dbSetExpedition.Add(expedition);
                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                    
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }
        public TransferOutDoc ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                 .Include(m => m.Items)
                 .FirstOrDefault();
            return model;
        }
        public MemoryStream GenerateExcel(int id)
        {
            var Query = from a in dbContext.TransferOutDocs
                        join b in dbContext.TransferOutDocItems on a.Id equals b.TransferOutDocsId
                        where a.Id == id
                        select new
                        {
                            a.Code,
                            a.SourceCode,
                            a.DestinationCode,
                            b.ItemCode,
                            b.ItemName,
                            b.Quantity,
                            b.DomesticCOGS,
                            b.Remark
                        };
            DataTable result = new DataTable();

            //result.Columns.Add(new DataColumn());
            result.Columns.Add(new DataColumn() { ColumnName = "No Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dari", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ke", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas Pengiriman", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Catatan", DataType = typeof(String) });

            if (Query.Count() == 0)
                result.Rows.Add("", "", "", "", "", 0, 0, "");
            else
            {
                foreach (var item in Query)
                {

                    result.Rows.Add(item.Code, item.SourceCode, item.DestinationCode, item.ItemCode, item.ItemName, item.Quantity, item.DomesticCOGS, item.Remark);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>> { (new KeyValuePair<DataTable, string>(result, "Retur")) }, true);

        }


    }
}
