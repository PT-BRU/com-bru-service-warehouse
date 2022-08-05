using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using CsvHelper;
using CsvHelper.TypeConversion;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class SOFacade : ISODoc
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<TransferInDoc> dbSetTransferIn;
        private readonly DbSet<SODocs> dbSetSO;
        private readonly IServiceProvider serviceProvider;
        private readonly DbSet<Inventory> dbSetInventory;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;
        private readonly DbSet<TransferOutDoc> dbSetTransferOut;

        public SOFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSetSO = dbContext.Set<SODocs>();
            this.dbSetTransferIn = dbContext.Set<TransferInDoc>();
            this.dbSetInventory = dbContext.Set<Inventory>();
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

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "Barcode", "Nama Barang", "Kuantitas Stock"
        };

        public sealed class SOMap : CsvHelper.Configuration.ClassMap<SODocsCsvViewModel>
        {
            public SOMap()
            {
                Map(p => p.code).Index(0);
                Map(p => p.name).Index(1);
                Map(p => p.quantity).Index(2).TypeConverter<StringConverter>();
            }
        }

        public MemoryStream DownloadTemplate()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    var configuration = new CsvHelper.Configuration.Configuration();
                    configuration.Delimiter = ";";

                    using (var csvWriter = new CsvWriter(streamWriter, configuration))
                    {
                        foreach (var item in CsvHeader)
                        {
                            csvWriter.WriteField(item);
                        }
                        csvWriter.NextRecord();
                    }
                }
                return stream;
            }
        }

        public Tuple<bool, List<object>> UploadValidate(ref List<SODocsCsvViewModel> Data, List<KeyValuePair<string, StringValues>> Body, string source)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            var storages = GetStorage(source);

            foreach (SODocsCsvViewModel productVM in Data)
            {
                //var item = dbContext.Inventories.Where(x => x.ItemCode == productVM.code && x.StorageId == storages.Id).FirstOrDefault();
                ErrorMessage = "";

                //if (item == null)
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Barang tidak ditemukan, ");
                //}

                if (string.IsNullOrWhiteSpace(productVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Barcode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.code.Equals(productVM.code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Barcode tidak boleh duplikat, ");
                }
                if (string.IsNullOrWhiteSpace(productVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                //else if (Data.Any(d => d != productVM && d.name.Equals(productVM.name)))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                //}
                decimal quantity = 0;
                if (string.IsNullOrWhiteSpace(productVM.quantity))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.quantity, out quantity))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity harus numerik, ");
                }
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    productVM.quantity = quantity;
                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;
                    Error.Add("Barcode", productVM.code);
                    Error.Add("Nama Barang", productVM.name);
                    Error.Add("Kuantitas Stock", productVM.quantity);
                    Error.Add("Error", ErrorMessage);
                    ErrorList.Add(Error);
                }
            }

            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }

        public async Task UploadData(SODocs data, string username)
        {            
            foreach (var i in data.Items)
            {
                EntityExtension.FlagForCreate(i, username, USER_AGENT);
            }
            EntityExtension.FlagForCreate(data, username, USER_AGENT);
            dbSetSO.Add(data);
            var result = await dbContext.SaveChangesAsync();
        }

        public async Task<SODocsViewModel> MapToViewModel(List<SODocsCsvViewModel> csv, string source)
        {
            List<SODocsItemViewModel> soDocsItems = new List<SODocsItemViewModel>();
            var storages = GetStorage(source);
            foreach (var i in csv)
            {
                var inventoryAvailable = dbContext.Inventories.Where(x => x.ItemCode == i.code && x.StorageId == storages.Id).FirstOrDefault();
                var item = GetItems(i.code);

                if(item != null)
                {
                    soDocsItems.Add(new SODocsItemViewModel
                    {
                        item = new ItemViewModel
                        {
                            _id = item._id,
                            articleRealizationOrder = item.ArticleRealizationOrder,
                            code = item.code,
                            name = item.name,
                            domesticCOGS = Convert.ToDouble(item.DomesticCOGS),
                            domesticRetail = Convert.ToDouble(item.DomesticRetail),
                            domesticWholesale = Convert.ToDouble(item.DomesticWholesale),
                            domesticSale = Convert.ToDouble(item.DomesticSale),
                            size = item.Size,
                            uom = item.Uom
                        },
                        qtySO = Convert.ToDouble(i.quantity),
                        qtyBeforeSO = inventoryAvailable == null ? 0 : Convert.ToDouble(inventoryAvailable.Quantity),
                        remark = ""
                    });
                } 
                else
                {
                    throw new Exception("data "+i.code+" tidak ada di master barang");
                }
            }

            SODocsViewModel soDocs = new SODocsViewModel
            {
                code = GenerateCode("BTQ-SO/INT"),
                storage = new StorageViewModel
                {
                    _id = storages.Id,
                    code = storages.Code, 
                    name = storages.Name,
                    isCentral = storages.IsCentral 
                },
                items = soDocsItems,
                isProcessed = false
            };

            return soDocs;
        }

        public async Task<int> Process(SODocs viewModel, string username, int clientTimeZoneOffset = 7)
        {
            
            int Process = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    var model = dbContext.StockOpnameDocs.Where(x => x.Id == viewModel.Id).FirstOrDefault();
                    model.IsProcessed = true;
                    
                    string inventoryMovementIn = GenerateCode("BTQ-TB/SO");
                    string inventoryMovementOut = GenerateCode("BTQ-KB/SO");

                    List<TransferInDocItem> transferInDocsItems = new List<TransferInDocItem>();
                    List<InventoryMovement> inventoryMovements = new List<InventoryMovement>();
                    List<TransferOutDocItem> transferOutDocsItems = new List<TransferOutDocItem>();

                    EntityExtension.FlagForUpdate(model, username, USER_AGENT);
                    foreach (var i in viewModel.Items)
                    {
                        var inventoriesAvailable = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == viewModel.StorageId).FirstOrDefault();
                        //var sourceQty = inventoriesAvailable == null ? 0 : inventoriesAvailable.Quantity;

                        if (i.IsAdjusted == true)
                        {
                            var items = dbContext.StockOpnameDocsItems.Where(x => x.Id == i.Id && x.SODocsId == viewModel.Id).FirstOrDefault();
                            items.IsAdjusted = true;
                            EntityExtension.FlagForUpdate(items, username, USER_AGENT);

                            if (i.QtyBeforeSO < i.QtySO)
                            {
                                if(inventoriesAvailable == null)
                                {
                                    var inven = new Inventory
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
                                        Quantity = i.QtySO,
                                        StorageCode = model.StorageCode,
                                        StorageId = model.StorageId,
                                        StorageName = model.StorageName,
                                        StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false
                                    };

                                    EntityExtension.FlagForCreate(inven, username, USER_AGENT);
                                    dbSetInventory.Add(inven);
                                } 
                                else
                                {
                                    inventoriesAvailable.Quantity = i.QtySO;
                                    EntityExtension.FlagForUpdate(inventoriesAvailable, username, USER_AGENT);
                                }

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
                                    Quantity = i.QtySO - i.QtyBeforeSO,
                                    Remark = i.Remark,
                                    Size = i.ItemSize,
                                    Uom = i.ItemUom
                                });

                                inventoryMovements.Add(new InventoryMovement
                                {
                                    Before = i.QtyBeforeSO,
                                    After = i.QtySO,
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
                                    Quantity = i.QtySO - i.QtyBeforeSO,
                                    StorageCode = viewModel.StorageCode,
                                    StorageId = viewModel.StorageId,
                                    StorageName = viewModel.StorageName,
                                    Type = "IN",
                                    Reference = inventoryMovementIn,
                                    Remark = i.Remark,
                                    StorageIsCentral = viewModel.StorageName.Contains("GUDANG") ? true : false,
                                });
                                EntityExtension.FlagForCreate(i, username, USER_AGENT);
                            }

                            else
                            {
                                if (inventoriesAvailable == null)
                                {
                                    var inven = new Inventory
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
                                        Quantity = i.QtySO,
                                        StorageCode = model.StorageCode,
                                        StorageId = model.StorageId,
                                        StorageName = model.StorageName,
                                        StorageIsCentral = model.StorageName.Contains("GUDANG") ? true : false
                                    };

                                    EntityExtension.FlagForCreate(inven, username, USER_AGENT);
                                    dbSetInventory.Add(inven);
                                }
                                else
                                {
                                    inventoriesAvailable.Quantity = i.QtySO;
                                    EntityExtension.FlagForUpdate(inventoriesAvailable, username, USER_AGENT);
                                }

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
                                    Quantity = i.QtyBeforeSO - i.QtySO,
                                    Remark = i.Remark,
                                    Size = i.ItemSize,
                                    Uom = i.ItemUom
                                });                                

                                inventoryMovements.Add(new InventoryMovement
                                {
                                    Before = i.QtyBeforeSO,
                                    After = i.QtySO,
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
                                    Quantity = i.QtyBeforeSO - i.QtySO,
                                    StorageCode = viewModel.StorageCode,
                                    StorageId = viewModel.StorageId,
                                    StorageName = viewModel.StorageName,
                                    Type = "OUT",
                                    Reference = inventoryMovementOut,
                                    Remark = i.Remark,
                                    StorageIsCentral = viewModel.StorageName.Contains("GUDANG") ? true : false,
                                });

                                EntityExtension.FlagForCreate(i, username, USER_AGENT);
                            }
                        }
                    }

                    if (transferInDocsItems.Count > 0)
                    {
                        TransferInDoc transferInDoc = new TransferInDoc
                        {
                            Code = inventoryMovementIn,
                            Date = DateTimeOffset.Now,
                            DestinationId = viewModel.StorageId,
                            DestinationCode = viewModel.StorageCode,
                            DestinationName = viewModel.StorageName,
                            SourceId = viewModel.StorageId,
                            SourceCode = viewModel.StorageCode,
                            SourceName = viewModel.StorageName,
                            Reference = viewModel.Code,
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

                    if (transferOutDocsItems.Count > 0)
                    {
                        TransferOutDoc transferOutDoc = new TransferOutDoc
                        {
                            Code = inventoryMovementOut,
                            Date = DateTimeOffset.Now,
                            DestinationId = viewModel.StorageId,
                            DestinationCode = viewModel.StorageCode,
                            DestinationName = viewModel.StorageName,
                            SourceId = viewModel.StorageId,
                            SourceCode = viewModel.StorageCode,
                            SourceName = viewModel.StorageName,
                            Reference = viewModel.Code,
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

                    Process = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }

                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }

                return Process;
            }
        }

        public async Task<int> Delete(int id, string username)
        {
            SODocs model = ReadById(id);
            EntityExtension.FlagForDelete(model, username, USER_AGENT, true);

            foreach (var i in model.Items)
            {
                EntityExtension.FlagForDelete(model, username, USER_AGENT, true);
            }

            dbSetSO.Update(model);

            return await dbContext.SaveChangesAsync();
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

        public Tuple<List<SODocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<SODocs> Query = this.dbSetSO.Include(m => m.Items).Where(m => m.IsDeleted == false);

            List<string> searchAttributes = new List<string>()
            {
                "Code"
            };

            Query = QueryHelper<SODocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SODocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SODocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SODocs> pageable = new Pageable<SODocs>(Query, Page - 1, Size);
            List<SODocs> Data = pageable.Data.ToList();
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

        public SODocs ReadById(int id)
        {
            var model = dbSetSO.Where(m => m.Id == id)
                .Include(m => m.Items)
                .FirstOrDefault();
            return model;
        }

        private ItemCustomViewModel GetItems(string code)
        {
            string itemUri = "items/finished-goods/code-discount/";
            string queryUri = code;
            string uri = itemUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{uri}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<ItemCustomViewModel> viewModel = JsonConvert.DeserializeObject<List<ItemCustomViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel.FirstOrDefault();
                //.Where(x => x.dataDestination[0].name == name && x.dataDestination[0].code == code).FirstOrDefault();
                //throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
            else
            {
                return null;
            }
        }
    }
}
