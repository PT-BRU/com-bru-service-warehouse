using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.TransferStocksInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.DanLiris.Service.Warehouse.Lib.ViewModels.TransferViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades.Stores
{
    public class TransferStockFacade : ITransferStock
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<TransferInDoc> dbSetTransferIn;
        private readonly DbSet<SPKDocs> dbSetSpk;
        private readonly IServiceProvider serviceProvider;
        private readonly DbSet<Inventory> dbSetInventory;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;
        private readonly DbSet<Expedition> dbSetExpedition;
        private readonly DbSet<TransferOutDoc> dbSet;

        public TransferStockFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSetTransferIn = dbContext.Set<TransferInDoc>();
            this.dbSetInventory = dbContext.Set<Inventory>();
            this.dbSetSpk = dbContext.Set<SPKDocs>();
            this.dbSetInventoryMovement = dbContext.Set<InventoryMovement>();
            this.dbSetExpedition = dbContext.Set<Expedition>();
            this.dbSet = dbContext.Set<TransferOutDoc>();
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

        public IQueryable<TransferStockReportViewModel> GetReportQuery(DateTime? datBTQom, DateTime? dateTo, string status, string code, int offset)
        {
            DateTime DatBTQom = datBTQom == null ? new DateTime(1970, 1, 1) : (DateTime)datBTQom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.TransferOutDocs
                         join b in dbContext.SPKDocs on a.Code equals b.Reference
                         join c in dbContext.TransferOutDocItems on a.Id equals c.TransferOutDocsId
                         where a.IsDeleted == false
                             && b.IsDeleted == false
                             && c.IsDeleted == false
                             && a.Date.AddHours(offset).Date >= DatBTQom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                             && a.Code.Contains(string.IsNullOrWhiteSpace(code) ? a.Code : code)
                             && b.Reference.Contains("BTQ-KB/RTT")
                             && b.DestinationName != "GUDANG TRANSFER STOCK"
                             && b.IsReceived == (status.Equals("Semua") ? b.IsReceived : (status.Equals("Belum Diterima") ? false : true)) 

                         select new TransferStockReportViewModel
                         {
                             code = a.Code,
                             date = a.Date,
                             sourceId = a.SourceId,
                             sourceCode = a.SourceCode,
                             sourceName = a.SourceName,
                             destinationId = a.DestinationId,
                             destinationCode = a.DestinationCode,
                             destinationName = a.DestinationName,
                             isReceived = b.IsReceived,
                             packingList = b.PackingList,
                             itemCode = c.ItemCode,
                             itemName = c.ItemName,
                             itemSize = c.Size,
                             itemUom = c.Uom,
                             itemArticleRealizationOrder = c.ArticleRealizationOrder,
                             Quantity = c.Quantity,
                             itemDomesticSale = c.DomesticSale,
                             LastModifiedUtc = a.LastModifiedUtc
                         });
            return Query.AsQueryable();
        }

        public Tuple<List<TransferStockReportViewModel>, int> GetReport(DateTime? datBTQom, DateTime? dateTo, string status, string code, int page, int size, string Order, int offset)
        {
            var Query = GetReportQuery(datBTQom, dateTo, status, code, offset);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            Pageable<TransferStockReportViewModel> pageable = new Pageable<TransferStockReportViewModel>(Query, page - 1, size);
            List<TransferStockReportViewModel> Data = pageable.Data.ToList<TransferStockReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public async Task<int> Create(TransferOutDocViewModel model, TransferOutDoc model2, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    string codeOut = GenerateCode("BTQ-KB/RTT");
                    string packingList1 = GenerateCode("BTQ-KB/PLR");
                    string CodeIn = GenerateCode("BTQ-TB/BRT");
                    string packingList2 = GenerateCode("BTQ-KB/PLB");
                    string expCode = GenerateCode("BTQ-KB/EXP");
                    string expCode2 = GenerateCode("BTQ-KB/EXP");
                    string codetransferin = GenerateCode("BTQ-TB/BRT");
                    model2.Code = codeOut;
                    model2.Date = DateTimeOffset.Now;
                    var storages = GetStorage("GDG.05");
                    var expeditionService = GetExpedition("Dikirim Sendiri");
                    List<ExpeditionItem> expeditionItems = new List<ExpeditionItem>();
                    List<ExpeditionDetail> expeditionDetails = new List<ExpeditionDetail>();
                    List<ExpeditionItem> expeditionItems2 = new List<ExpeditionItem>();
                    List<ExpeditionDetail> expeditionDetails2 = new List<ExpeditionDetail>();
                    List<SPKDocsItem> sPKDocsItem1 = new List<SPKDocsItem>();
                    List<SPKDocsItem> sPKDocsItem2 = new List<SPKDocsItem>();
                    List<TransferInDocItem> transferInDocs = new List<TransferInDocItem>();
                    List<InventoryMovement> inventoryMovements = new List<InventoryMovement>();
                    List<TransferOutDocItem> transferOutDocItems = new List<TransferOutDocItem>();
                    EntityExtension.FlagForCreate(model2, username, USER_AGENT);

                    foreach (var i in model2.Items)
                    {
                        var invenInTransfer = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == storages.Id).FirstOrDefault();
                        if (invenInTransfer == null)
                        {
                            Inventory inventory = new Inventory
                            {
                                ItemArticleRealizationOrder = i.ArticleRealizationOrder,
                                ItemCode = i.ItemCode,
                                ItemDomesticCOGS = i.DomesticCOGS,
                                ItemDomesticRetail = i.DomesticRetail,
                                ItemDomesticSale = i.DomesticSale,
                                ItemDomesticWholeSale = i.DomesticWholeSale,
                                ItemId = i.ItemId,
                                ItemInternationalCOGS = 0,
                                ItemInternationalRetail = 0,
                                ItemInternationalSale = 0,
                                ItemInternationalWholeSale = 0,
                                ItemName = i.ItemName,
                                ItemSize = i.Size,
                                ItemUom = i.Uom,
                                Quantity = 0,
                                StorageCode = storages.Code,
                                StorageId = storages.Id,
                                StorageName = storages.Name,
                                StorageIsCentral = storages.Name.Contains("GUDANG") ? true : false,
                            };
                            EntityExtension.FlagForCreate(inventory, username, USER_AGENT);
                            dbSetInventory.Add(inventory);
                        }

                        transferOutDocItems.Add(new TransferOutDocItem
                        {
                            ArticleRealizationOrder = i.ArticleRealizationOrder,
                            DomesticCOGS = i.DomesticCOGS,
                            DomesticRetail = i.DomesticRetail,
                            DomesticSale = i.DomesticSale,
                            DomesticWholeSale = i.DomesticWholeSale,
                            ItemCode = i.ItemCode,
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            Quantity = i.Quantity,
                            Remark = i.Remark,
                            Size = i.Size,
                            Uom = i.Uom
                        });

                        sPKDocsItem1.Add(new SPKDocsItem
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

                        sPKDocsItem2.Add(new SPKDocsItem
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

                        transferInDocs.Add(new TransferInDocItem
                        {
                            ArticleRealizationOrder = i.ArticleRealizationOrder,
                            ItemCode = i.ItemCode,
                            DomesticCOGS = i.DomesticCOGS,
                            DomesticRetail = i.DomesticRetail,
                            DomesticSale = i.DomesticSale,
                            DomesticWholeSale = i.DomesticWholeSale,
                            ItemId = i.ItemId,
                            ItemName = i.ItemName,
                            Size = i.Size,
                            Uom = i.Uom,
                            Quantity = i.Quantity,
                            Remark = i.Remark
                        });

                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    EntityExtension.FlagForCreate(model2, username, USER_AGENT);

                    SPKDocs sPKDocs1 = new SPKDocs
                    {
                        Code = GenerateCode("BTQ-PK/PBJ"),
                        Date = DateTimeOffset.Now,
                        SourceId = model2.SourceId,
                        SourceCode = model2.SourceCode,
                        SourceName = model2.SourceName,
                        DestinationId = storages.Id,
                        DestinationCode = storages.Code,
                        DestinationName = storages.Name,
                        IsDistributed = true,
                        IsReceived = true,
                        IsDraft = false,
                        PackingList = packingList1,
                        Reference = codeOut,
                        Password = String.Join("", GenerateCode(DateTime.Now.ToString("dd")).Split("/")),
                        Weight = 0,
                        Items = sPKDocsItem1
                    };
                    EntityExtension.FlagForCreate(sPKDocs1, username, USER_AGENT);
                    foreach(var i in sPKDocs1.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    dbSetSpk.Add(sPKDocs1);

                    TransferInDoc transferInDoc = new TransferInDoc
                    {
                        Code = codetransferin,
                        Date = DateTimeOffset.Now,
                        DestinationId = storages.Id,
                        DestinationCode = storages.Code,
                        DestinationName = storages.Name,
                        SourceCode = model2.SourceCode,
                        SourceId = model2.SourceId,
                        SourceName = model2.SourceName,
                        Reference = packingList1,
                        Remark = "",
                        Items = transferInDocs
                    };
                    EntityExtension.FlagForCreate(transferInDoc, username, USER_AGENT);

                    SPKDocs sPKDocs2 = new SPKDocs
                    {
                        Code = GenerateCode("BTQ-PK/PBJ"),
                        Date = DateTimeOffset.Now,
                        DestinationId = model2.DestinationId,
                        DestinationCode = model2.DestinationCode,
                        DestinationName = model2.DestinationName,
                        SourceId = storages.Id,
                        SourceCode = storages.Code,
                        SourceName = storages.Name,
                        IsDistributed = true,
                        IsReceived = false,
                        IsDraft = false,
                        PackingList = packingList2,
                        Reference = codeOut,
                        Password = String.Join("", GenerateCode(DateTime.Now.ToString("dd")).Split("/")),
                        Weight = 0,
                        Items = sPKDocsItem2
                    };
                    EntityExtension.FlagForCreate(sPKDocs2, username, USER_AGENT);
                    foreach (var i in sPKDocs2.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    dbSetSpk.Add(sPKDocs2);

                    await dbContext.SaveChangesAsync();

                    foreach(var i in sPKDocs1.Items)
                    {
                        var QtySource = 0.0;
                        var invenOutSource = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == model2.SourceId).FirstOrDefault();
                        
                        if (invenOutSource != null)
                        {
                            QtySource = invenOutSource.Quantity;
                            invenOutSource.Quantity = invenOutSource.Quantity - i.Quantity;
                        }

                        inventoryMovements.Add(new InventoryMovement
                        {
                            Before = QtySource,
                            After = invenOutSource.Quantity,
                            Date = DateTimeOffset.Now,
                            ItemCode = i.ItemCode,
                            ItemDomesticCOGS = i.ItemDomesticCOGS,
                            ItemDomesticRetail = i.ItemDomesticRetail,
                            ItemDomesticWholeSale = i.ItemDomesticRetail,
                            ItemDomesticSale = i.ItemDomesticSale,
                            ItemId = i.ItemId,
                            ItemInternationalCOGS = 0,
                            ItemInternationalRetail = 0,
                            ItemInternationalSale = 0,
                            ItemInternationalWholeSale = 0,
                            ItemName = i.ItemName,
                            ItemSize = i.ItemSize,
                            ItemUom = i.ItemUom,
                            Quantity = i.Quantity,
                            StorageCode = model2.SourceCode,
                            StorageId = model2.SourceId,
                            StorageName = model2.SourceName,
                            Type = "OUT",
                            Reference = codeOut,
                            Remark = model2.Remark,
                            StorageIsCentral = model2.SourceName.Contains("GUDANG") ? true : false,
                        });

                        inventoryMovements.Add(new InventoryMovement
                        {
                            Before = 0,
                            After = i.Quantity,
                            Date = DateTimeOffset.Now,
                            ItemCode = i.ItemCode,
                            ItemDomesticCOGS = i.ItemDomesticCOGS,
                            ItemDomesticRetail = i.ItemDomesticRetail,
                            ItemDomesticWholeSale = i.ItemDomesticRetail,
                            ItemDomesticSale = i.ItemDomesticSale,
                            ItemId = i.ItemId,
                            ItemInternationalCOGS = 0,
                            ItemInternationalRetail = 0,
                            ItemInternationalSale = 0,
                            ItemInternationalWholeSale = 0,
                            ItemName = i.ItemName,
                            ItemSize = i.ItemSize,
                            ItemUom = i.ItemUom,
                            Quantity = i.Quantity,
                            StorageCode = storages.Code,
                            StorageId = storages.Id,
                            StorageName = storages.Name,
                            Type = "IN",
                            Reference = codetransferin,
                            Remark = model2.Remark,
                            StorageIsCentral = storages.Name.Contains("GUDANG") ? true : false,
                        });

                        inventoryMovements.Add(new InventoryMovement
                        {
                            Before = i.Quantity,
                            After = 0,
                            Date = DateTimeOffset.Now,
                            ItemCode = i.ItemCode,
                            ItemDomesticCOGS = i.ItemDomesticCOGS,
                            ItemDomesticRetail = i.ItemDomesticRetail,
                            ItemDomesticWholeSale = i.ItemDomesticRetail,
                            ItemDomesticSale = i.ItemDomesticSale,
                            ItemId = i.ItemId,
                            ItemInternationalCOGS = 0,
                            ItemInternationalRetail = 0,
                            ItemInternationalSale = 0,
                            ItemInternationalWholeSale = 0,
                            ItemName = i.ItemName,
                            ItemSize = i.ItemSize,
                            ItemUom = i.ItemUom,
                            Quantity = i.Quantity,
                            StorageCode = storages.Code,
                            StorageId = storages.Id,
                            StorageName = storages.Name,
                            Type = "OUT",
                            Reference = expCode,
                            Remark = model2.Remark,
                            StorageIsCentral = model2.DestinationName.Contains("GUDANG") ? true : false,
                        });

                        expeditionDetails2.Add(new ExpeditionDetail
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
                            //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x=>x.Id).FirstOrDefault().Id + 1
                            SPKDocsId = (int)sPKDocs1.Id
                        });
                    }
                    
                    foreach(var i in sPKDocs2.Items)
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
                            //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x=>x.Id).FirstOrDefault().Id + 1
                            SPKDocsId = (int)sPKDocs2.Id
                        });
                    }

                    expeditionItems.Add(new ExpeditionItem
                    {
                        Code = sPKDocs2.Code,
                        Date = sPKDocs2.Date,
                        DestinationCode = sPKDocs2.DestinationCode,
                        DestinationId = (int)sPKDocs2.DestinationId,
                        DestinationName = sPKDocs2.DestinationName,
                        IsDistributed = sPKDocs2.IsDistributed,
                        IsDraft = sPKDocs2.IsDraft,
                        IsReceived = sPKDocs2.IsReceived,
                        PackingList = sPKDocs2.PackingList,
                        Password = sPKDocs2.Password,
                        Reference = codeOut,
                        SourceCode = sPKDocs2.SourceCode,
                        SourceId = (int)sPKDocs2.SourceId,
                        SourceName = sPKDocs2.SourceName,
                        //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        SPKDocsId = (int)sPKDocs2.Id,
                        Weight = sPKDocs2.Weight,
                        Details = expeditionDetails
                    });

                    expeditionItems2.Add(new ExpeditionItem
                    {
                        Code = sPKDocs1.Code,
                        Date = sPKDocs1.Date,
                        DestinationCode = sPKDocs1.DestinationCode,
                        DestinationId = (int)sPKDocs1.DestinationId,
                        DestinationName = sPKDocs1.DestinationName,
                        IsDistributed = sPKDocs1.IsDistributed,
                        IsDraft = sPKDocs1.IsDraft,
                        IsReceived = sPKDocs1.IsReceived,
                        PackingList = sPKDocs1.PackingList,
                        Password = sPKDocs1.Password,
                        Reference = codeOut,
                        SourceCode = sPKDocs1.SourceCode,
                        SourceId = (int)sPKDocs1.SourceId,
                        SourceName = sPKDocs1.SourceName,
                        //SPKDocsId = (int)dbContext.SPKDocs.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1,
                        SPKDocsId = (int)sPKDocs1.Id,
                        Weight = sPKDocs1.Weight,
                        Details = expeditionDetails2
                    });

                    Expedition expedition = new Expedition
                    {
                        Code = expCode,
                        Date = DateTimeOffset.Now,
                        ExpeditionServiceCode = expeditionService.code,
                        ExpeditionServiceId = (int)expeditionService._id,
                        ExpeditionServiceName = expeditionService.name,
                        Remark = "",
                        Weight = 0,
                        Items = expeditionItems,

                    };
                    EntityExtension.FlagForCreate(expedition, username, USER_AGENT);

                    Expedition expedition2 = new Expedition
                    {
                        Code = expCode2,
                        Date = DateTimeOffset.Now,
                        ExpeditionServiceCode = expeditionService.code,
                        ExpeditionServiceId = (int)expeditionService._id,
                        ExpeditionServiceName = expeditionService.name,
                        Remark = "",
                        Weight = 0,
                        Items = expeditionItems2,

                    };
                    EntityExtension.FlagForCreate(expedition2, username, USER_AGENT);

                    TransferOutDoc transferOutDoc2 = new TransferOutDoc
                    {
                        Code = expCode,
                        Date = DateTimeOffset.Now,
                        DestinationCode = model2.DestinationCode,
                        DestinationId = model2.DestinationId,
                        DestinationName = model2.DestinationName,
                        Reference = packingList2,
                        Remark = "",
                        SourceId = storages.Id,
                        SourceCode = storages.Code,
                        SourceName = storages.Name,
                        Items = transferOutDocItems
                    };
                    EntityExtension.FlagForCreate(transferOutDoc2, username, USER_AGENT);

                    #region Saving
                    foreach(var i in transferOutDoc2.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }

                    foreach (var i in expedition.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        foreach(var d in i.Details)
                        {
                            EntityExtension.FlagForCreate(d, username, USER_AGENT);
                        }
                    }
                    foreach (var i in expedition2.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        foreach (var d in i.Details)
                        {
                            EntityExtension.FlagForCreate(d, username, USER_AGENT);
                        }
                    }
                    foreach (var i in transferInDoc.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    foreach(var i in inventoryMovements)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        dbSetInventoryMovement.Add(i);
                    }

                    dbSetExpedition.Add(expedition);
                    dbSetExpedition.Add(expedition2);
                    dbSet.Add(model2);
                    dbSet.Add(transferOutDoc2);
                    dbSetTransferIn.Add(transferInDoc);

                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();

                    #endregion

                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }

                return Created;
            }
        }

        private StorageViewModel2 GetStorage(string code)
        {
            string itemUri = "master/storages/code";
            string queryUri = "?code=" + code;
            string uri = itemUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{uri}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                StorageViewModel2 viewModel = JsonConvert.DeserializeObject<StorageViewModel2>(result.GetValueOrDefault("data").ToString());
                return viewModel;//.Where(x => x.dataDestination[0].name == name && x.dataDestination[0].code == code).FirstOrDefault();
                //throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
            else
            {
                return null;
            }
        }
        private ExpeditionServiceViewModel GetExpedition(string code)
        {
            string itemUri = "expedition-service-routers/all/code";
            string queryUri = "?code=" + code;
            string uri = itemUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{uri}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                ExpeditionServiceViewModel viewModel = JsonConvert.DeserializeObject<ExpeditionServiceViewModel>(result.GetValueOrDefault("data").ToString());
                return viewModel;//.Where(x => x.dataDestination[0].name == name && x.dataDestination[0].code == code).FirstOrDefault();
                //throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
            else
            {
                return null;
            }
        }
        public Tuple<List<TransferOutDoc>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<TransferOutDoc> Query = this.dbSet.Include(m => m.Items).OrderByDescending(x => x.Date);

            List<string> searchAttributes = new List<string>()
            {
                "Code","DestinationName","SourceName","Referensi","TransferName"
            };

            Query = QueryHelper<TransferOutDoc>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<TransferOutDoc>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<TransferOutDoc>.ConfigureOrder(Query, OrderDictionary);

            Pageable<TransferOutDoc> pageable = new Pageable<TransferOutDoc>(Query, Page - 1, Size);
            List<TransferOutDoc> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<TransferStockViewModel>, int, Dictionary<string, string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            var Query =  from a in dbContext.TransferOutDocs
                         join b in dbContext.SPKDocs on a.Code equals b.Reference
                         where a.Code.Contains("BTQ-KB/RTT") && b.DestinationName != "GUDANG TRANSFER STOCK"
                         orderby a.Date descending
                         select new TransferStockViewModel
                         {
                             id = (int)a.Id,
                             code = a.Code,
                             createdBy = a.CreatedBy,
                             createdDate = a.CreatedUtc,
                             destinationname = a.DestinationName,
                             destinationcode = a.DestinationCode,
                             sourcename = a.SourceName,
                             sourcecode = a.SourceCode,
                             password = b.Password,
                             referensi = a.Reference,
                             transfername = b.SourceName,
                             transfercode = b.SourceCode
                         };
            List<string> searchAttributes = new List<string>()
            {
                "Code","DestinationName","SourceName"
            };

            Query = QueryHelper<TransferStockViewModel>.ConfigureSearch(Query, searchAttributes, Keyword);


            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            //Query = QueryHelper<TransferOutDoc>.ConfigureOrder(Query, OrderDictionary);

            Pageable<TransferStockViewModel> pageable = new Pageable<TransferStockViewModel>(Query, Page - 1, Size);
            List<TransferStockViewModel> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }
        public TransferStockViewModel ReadById(int id)
        {
            var Query = from a in dbContext.TransferOutDocs
                        join b in dbContext.SPKDocs on a.Code equals b.Reference
                        join c in dbContext.TransferOutDocItems on a.Id equals c.TransferOutDocsId
                        where a.Code.Contains("BTQ-KB/RTT") && b.DestinationName != "GUDANG TRANSFER STOCK"
                        && a.Id == id
                        select new 
                        {
                            id = (int)a.Id,
                            code = a.Code,
                            createdBy = a.CreatedBy,
                            createdDate = a.CreatedUtc,
                            destinationname = a.DestinationName,
                            sourcecode = a.SourceCode,
                            sourcename = a.SourceName,
                            password = b.Password,
                            transfercode = b.SourceCode,
                            destinationcode = a.DestinationCode,
                            referensi = a.Reference,
                            transfername = b.SourceName,
                            itemCode = c.ItemCode,
                            itemName = c.ItemName,
                            quantity = c.Quantity,
                            price = c.DomesticSale,
                            remark = c.Remark
                        };
            List<TransferOutDocItemViewModel> transferOutDocsitems = new List<TransferOutDocItemViewModel>();
            foreach(var i in Query)
            {
                transferOutDocsitems.Add(new TransferOutDocItemViewModel
                {
                    item = new ItemViewModel
                    {
                        code = i.itemCode,
                        name = i.itemName,
                        domesticSale = i.price,

                    },
                    quantity = i.quantity,
                    remark = i.remark
                });
            }

            TransferStockViewModel model = new TransferStockViewModel
            {
                code = Query.FirstOrDefault().code,
                createdBy = Query.FirstOrDefault().createdBy,
                createdDate = Query.FirstOrDefault().createdDate,
                destinationcode = Query.FirstOrDefault().destinationcode,
                destinationname = Query.FirstOrDefault().destinationname,
                id = Query.FirstOrDefault().id,
                password = Query.FirstOrDefault().password,
                referensi = Query.FirstOrDefault().referensi,
                sourcecode = Query.FirstOrDefault().sourcecode,
                sourcename = Query.FirstOrDefault().sourcename,
                transfercode = Query.FirstOrDefault().transfercode,
                transfername = Query.FirstOrDefault().transfername,
                items = transferOutDocsitems
            };
            //var model = dbSet.Where(m => m.Id == id)
            //     .Include(m => m.Items)
            //     .FirstOrDefault();
            return model;
        }
    }
}
