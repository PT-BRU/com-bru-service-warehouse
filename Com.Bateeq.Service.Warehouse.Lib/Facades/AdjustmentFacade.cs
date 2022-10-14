using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.AdjustmentInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades.AdjustmentFacade
{
    public class AdjustmentFacade : IAdjustmentDoc
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<TransferInDoc> dbSetTransferIn;
        private readonly DbSet<AdjustmentDocs> dbSetAdjustment;
        private readonly IServiceProvider serviceProvider;
        private readonly DbSet<Inventory> dbSetInventory;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;
        private readonly DbSet<TransferOutDoc> dbSetTransferOut;

        public AdjustmentFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSetTransferIn = dbContext.Set<TransferInDoc>();
            this.dbSetInventory = dbContext.Set<Inventory>();
            this.dbSetAdjustment = dbContext.Set<AdjustmentDocs>();
            this.dbSetInventoryMovement = dbContext.Set<InventoryMovement>();
            this.dbSetTransferOut = dbContext.Set<TransferOutDoc>();
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

        public async Task<int> Create(AdjustmentDocs model, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    string code = GenerateCode("BTQ-ADJ/INT");
                    var storages = GetStorage(model.StorageCode);

                    string inventoryMovementIn = GenerateCode("BTQ-TB/ADJ");
                    string inventoryMovementOut = GenerateCode("BTQ-KB/ADJ");

                    List<TransferInDocItem> transferInDocsItems = new List<TransferInDocItem>();
                    List<InventoryMovement> inventoryMovements = new List<InventoryMovement>();
                    List<TransferOutDocItem> transferOutDocsItems = new List<TransferOutDocItem>();

                    model.Code = code;

                    EntityExtension.FlagForCreate(model, username, USER_AGENT);
                    foreach (var i in model.Items)
                    {
                        if (i.Type == "IN")
                        {
                            //var SourceQuantity = 0.0;

                            var inventoriesAvailable = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == storages.Id).FirstOrDefault();

                            if (inventoriesAvailable != null)
                            {
                                //SourceQuantity = inventoriesAvailable.Quantity;
                                inventoriesAvailable.Quantity = inventoriesAvailable.Quantity + i.QtyAdjustment;
                                EntityExtension.FlagForUpdate(inventoriesAvailable, username, USER_AGENT);
                            }
                            //else {
                            //    //throw new Exception("data tidak ada di inventory");
                            //}
                            else
                            {
                                Inventory inventory = new Inventory
                                {
                                    ItemArticleRealizationOrder = i.ItemArticleRealizationOrder,
                                    ItemCode = i.ItemCode,
                                    ItemDomesticCOGS = i.ItemDomesticCOGS,
                                    ItemDomesticRetail = i.ItemDomesticRetail,
                                    ItemDomesticSale = i.ItemDomesticSale,
                                    ItemDomesticWholeSale = i.ItemDomesticWholeSale,
                                    ItemId = i.ItemId,
                                    ItemInternationalCOGS = 0,
                                    ItemInternationalRetail = 0,
                                    ItemInternationalSale = 0,
                                    ItemInternationalWholeSale = 0,
                                    ItemName = i.ItemName,
                                    ItemSize = i.ItemSize,
                                    ItemUom = i.ItemUom,
                                    Quantity = i.QtyAdjustment,
                                    StorageCode = model.StorageCode,
                                    StorageId = model.StorageId,
                                    StorageName = model.StorageName,
                                    StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false
                                };

                                EntityExtension.FlagForCreate(inventory, username, USER_AGENT);
                                dbSetInventory.Add(inventory);
                            }

                            //i.QtyBeforeAdjustment = SourceQuantity;

                            transferInDocsItems.Add(new TransferInDocItem
                            {
                                ArticleRealizationOrder = i.ItemArticleRealizationOrder,
                                DomesticCOGS = i.ItemDomesticCOGS,
                                DomesticRetail = i.ItemDomesticRetail,
                                DomesticSale = i.ItemDomesticSale,
                                DomesticWholeSale = i.ItemDomesticWholeSale,
                                ItemCode = i.ItemCode,
                                ItemId = i.ItemId,
                                ItemName = i.ItemName,
                                Quantity = i.QtyAdjustment,
                                Remark = i.Remark,
                                Size = i.ItemSize,
                                Uom = i.ItemUom
                            });
                            EntityExtension.FlagForCreate(i, username, USER_AGENT);

                            var im = new InventoryMovement
                            {
                                Before = i.QtyBeforeAdjustment,
                                After = i.QtyBeforeAdjustment + i.QtyAdjustment,
                                Date = DateTimeOffset.Now,
                                ItemArticleRealizationOrder = i.ItemArticleRealizationOrder,
                                ItemCode = i.ItemCode,
                                ItemDomesticCOGS = i.ItemDomesticCOGS,
                                ItemDomesticRetail = i.ItemDomesticRetail,
                                ItemDomesticWholeSale = i.ItemDomesticRetail,
                                ItemDomesticSale = i.ItemDomesticSale,
                                ItemId = i.ItemId,
                                ItemInternationalCOGS = i.ItemInternationalCOGS,
                                ItemInternationalRetail = i.ItemInternationalRetail,
                                ItemInternationalSale = i.ItemInternationalSale,
                                ItemInternationalWholeSale = i.ItemInternationalWholeSale,
                                ItemName = i.ItemName,
                                ItemSize = i.ItemSize,
                                ItemUom = i.ItemUom,
                                Quantity = i.QtyAdjustment,
                                StorageCode = model.StorageCode,
                                StorageId = model.StorageId,
                                StorageName = model.StorageName,
                                Type = "IN",
                                Reference = inventoryMovementIn,
                                Remark = i.Remark,
                                StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false
                            };

                            inventoryMovements.Add(im);
                        }
                        else
                        {
                            //var SourceQuantity = 0.0;

                            var inventoriesAvailable = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == storages.Id).FirstOrDefault();
                            if (inventoriesAvailable != null)
                            {
                                //SourceQuantity = inventoriesAvailable.Quantity;
                                inventoriesAvailable.Quantity = inventoriesAvailable.Quantity - i.QtyAdjustment;
                                EntityExtension.FlagForUpdate(inventoriesAvailable, username, USER_AGENT);
                            }
                            else
                            {
                                throw new Exception("data tidak ada di inventory");
                            }
                            //else
                            //{
                            //    Inventory inventory = new Inventory
                            //    {
                            //        ItemArticleRealizationOrder = i.ItemArticleRealizationOrder,
                            //        ItemCode = i.ItemCode,
                            //        ItemDomesticCOGS = i.ItemDomesticCOGS,
                            //        ItemDomesticRetail = i.ItemDomesticRetail,
                            //        ItemDomesticSale = i.ItemDomesticSale,
                            //        ItemDomesticWholeSale = i.ItemDomesticWholeSale,
                            //        ItemId = i.ItemId,
                            //        ItemInternationalCOGS = 0,
                            //        ItemInternationalRetail = 0,
                            //        ItemInternationalSale = 0,
                            //        ItemInternationalWholeSale = 0,
                            //        ItemName = i.ItemName,
                            //        ItemSize = i.ItemSize,
                            //        ItemUom = i.ItemUom,
                            //        Quantity = i.QtyAdjustment,
                            //        StorageCode = model.StorageCode,
                            //        StorageId = model.StorageId,
                            //        StorageName = model.StorageName,
                            //        StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false
                            //    };
                            //    EntityExtension.FlagForCreate(inventory, username, USER_AGENT);
                            //    dbSetInventory.Add(inventory);
                            //}

                            //i.QtyBeforeAdjustment = SourceQuantity;

                            transferOutDocsItems.Add(new TransferOutDocItem
                            {
                                ArticleRealizationOrder = i.ItemArticleRealizationOrder,
                                DomesticCOGS = i.ItemDomesticCOGS,
                                DomesticRetail = i.ItemDomesticRetail,
                                DomesticSale = i.ItemDomesticSale,
                                DomesticWholeSale = i.ItemDomesticWholeSale,
                                ItemCode = i.ItemCode,
                                ItemId = i.ItemId,
                                ItemName = i.ItemName,
                                Quantity = i.QtyAdjustment,
                                Remark = i.Remark,
                                Size = i.ItemSize,
                                Uom = i.ItemUom
                            });
                            EntityExtension.FlagForCreate(i, username, USER_AGENT);

                            var im = new InventoryMovement()
                            {
                                Before = i.QtyBeforeAdjustment,
                                After = i.QtyBeforeAdjustment - i.QtyAdjustment,
                                Date = DateTimeOffset.Now,
                                ItemArticleRealizationOrder = i.ItemArticleRealizationOrder,
                                ItemCode = i.ItemCode,
                                ItemDomesticCOGS = i.ItemDomesticCOGS,
                                ItemDomesticRetail = i.ItemDomesticRetail,
                                ItemDomesticWholeSale = i.ItemDomesticRetail,
                                ItemDomesticSale = i.ItemDomesticSale,
                                ItemId = i.ItemId,
                                ItemInternationalCOGS = i.ItemInternationalCOGS,
                                ItemInternationalRetail = i.ItemInternationalRetail,
                                ItemInternationalSale = i.ItemInternationalSale,
                                ItemInternationalWholeSale = i.ItemInternationalWholeSale,
                                ItemName = i.ItemName,
                                ItemSize = i.ItemSize,
                                ItemUom = i.ItemUom,
                                Quantity = i.QtyAdjustment,
                                StorageCode = model.StorageCode,
                                StorageId = model.StorageId,
                                StorageName = model.StorageName,
                                Type = "OUT",
                                Reference = inventoryMovementOut,
                                Remark = i.Remark,
                                StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false,
                            };

                            inventoryMovements.Add(im);

                        }
                    }

                    if(transferInDocsItems.Count > 0)
                    {
                        TransferInDoc transferInDoc = new TransferInDoc
                        {
                            Code = inventoryMovementIn,
                            Date = DateTimeOffset.Now,
                            DestinationId = model.StorageId,
                            DestinationCode = model.StorageCode,
                            DestinationName = model.StorageName,
                            SourceId = model.StorageId,
                            SourceCode = model.StorageCode,
                            SourceName = model.StorageName,
                            Reference = code,
                            Remark = "",
                            Items = transferInDocsItems
                        };
                        EntityExtension.FlagForCreate(transferInDoc, username, USER_AGENT);

                        foreach (var tii in transferInDoc.Items)
                        {
                            EntityExtension.FlagForCreate(tii, username, USER_AGENT);
                        }

                        dbSetTransferIn.Add(transferInDoc);
                    }

                    if(transferOutDocsItems.Count > 0)
                    {
                        TransferOutDoc transferOutDoc = new TransferOutDoc
                        {
                            Code = inventoryMovementOut,
                            Date = DateTimeOffset.Now,
                            DestinationId = model.StorageId,
                            DestinationCode = model.StorageCode,
                            DestinationName = model.StorageName,
                            SourceId = model.StorageId,
                            SourceCode = model.StorageCode,
                            SourceName = model.StorageName,
                            Reference = code,
                            Remark = "",
                            Items = transferOutDocsItems
                        };
                        EntityExtension.FlagForCreate(transferOutDoc, username, USER_AGENT);

                        foreach (var toi in transferOutDoc.Items)
                        {
                            EntityExtension.FlagForCreate(toi, username, USER_AGENT);
                        }

                        dbSetTransferOut.Add(transferOutDoc);
                    }

                    foreach (var im in inventoryMovements)
                    {
                        EntityExtension.FlagForCreate(im, username, USER_AGENT);
                        dbSetInventoryMovement.Add(im);
                    }

                    dbSetAdjustment.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
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

        public Tuple<List<AdjustmentDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<AdjustmentDocs> Query = this.dbSetAdjustment.Include(m => m.Items).Where(m => m.IsDeleted == false);

            List<string> searchAttributes = new List<string>()
            {
                "Code"
            };

            Query = QueryHelper<AdjustmentDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<AdjustmentDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<AdjustmentDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<AdjustmentDocs> pageable = new Pageable<AdjustmentDocs>(Query, Page - 1, Size);
            List<AdjustmentDocs> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        //public Tuple<List<TransferStockViewModel>, int, Dictionary<string, string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        //{
        //    var Query = from a in dbContext.TransferOutDocs
        //               join b in dbContext.SPKDocs on a.Code equals b.Reference
        //               where a.Code.Contains("BTQ-KB/RTT") && b.DestinationName != "GUDANG TRANSFER STOCK"
        //               select new TransferStockViewModel
        //               {
        //                   id = (int)a.Id,
        //                   code = a.Code,
        //                   createdBy = a.CreatedBy,
        //                   createdDate = a.CreatedUtc,
        //                   destinationname = a.DestinationName,
        //                   destinationcode = a.DestinationCode,
        //                   sourcename = a.SourceName,
        //                   sourcecode = a.SourceCode,
        //                   password = b.Password,
        //                   referensi = a.Reference,
        //                   transfername = b.SourceName,
        //                   transfercode = b.SourceCode
        //               };

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    //Query = QueryHelper<TransferOutDoc>.ConfigureOrder(Query, OrderDictionary);
        //
        //    Pageable<TransferStockViewModel> pageable = new Pageable<TransferStockViewModel>(Query, Page - 1, Size);
        //    List<TransferStockViewModel> Data = pageable.Data.ToList();
        //    int TotalData = pageable.TotalCount;
        //
        //    return Tuple.Create(Data, TotalData, OrderDictionary);
        //}

        public AdjustmentDocs ReadById(int id)
        {
            var model = dbSetAdjustment.Where(m => m.Id == id)
                .Include(m => m.Items)
                .FirstOrDefault();
            return model;
        }

        #region readByStorage

        public List<AdjustmentDocs> ReadByStorage(int id)
        {
            var modelList = dbSetAdjustment.Where(m => m.StorageId == id)
                .Include(m => m.Items)
                .ToList();

            return modelList;
        }

        #endregion
    }
}
