using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Moonlay.Models;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class SPKDocsControllerFacade : ISPKDoc
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<Inventory> dbSet;
        private readonly IServiceProvider serviceProvider;

        public SPKDocsControllerFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<Inventory>();
        }

        public async Task<int> Create(SPKDocsFromFinihsingOutsViewModel viewModel, string username, string token)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<SPKDocsItem> sPKDocsItems = new List<SPKDocsItem>();
                    
                    foreach (var item in viewModel.Items)
                    {
                        var isDifferentSize = item.IsDifferentSize;
                        if (isDifferentSize == true)
                        {
                            
                            foreach (var detail in item.Details)
                            {
                                var sizeId = detail.Size.Id.ToString("00");
                                var counterId = viewModel.counters._id.ToString("00");
                                var subCounterId = viewModel.subCounters._id.ToString("00");
                                var asal = viewModel.SourceId.ToString("0");
                                var roCreatedUtc = viewModel.RoCreatedUtc;
                                var materialId = viewModel.materials._id.ToString("00");

                                var barcode = asal + counterId + subCounterId + materialId + sizeId + roCreatedUtc;
                                Console.WriteLine("barcodefad " + barcode);
                                var itemx = GetItem(barcode);

                                if (itemx == null || itemx.Count() == 0) //barcode belum terdaftar, insert ke tabel items (BMS) terlebih dahulu
                                {
                                    ItemCoreViewModelUsername itemCore = new ItemCoreViewModelUsername
                                    {
                                        dataDestination = new List<ItemViewModelRead>
                                        {
                                           new ItemViewModelRead
                                           {
                                               ArticleRealizationOrder = viewModel.RONo,
                                               code = barcode,
                                               name = viewModel.Comodity.name,
                                               Size = item.Size.Size,
                                               Uom = item.Uom.Unit,
                                               ImagePath = viewModel.ImagePath,
                                               ImgFile = "",
                                               Tags = "",
                                               Remark = "",
                                               Description = "",
                                               _id = 0
                                           }
                                        },
                                        color = viewModel.color,
                                        process = viewModel.process,
                                        materials = viewModel.materials,
                                        materialCompositions = viewModel.materialCompositions,
                                        collections = viewModel.collections,
                                        seasons = viewModel.seasons,
                                        counters = viewModel.counters,
                                        subCounters = viewModel.subCounters,
                                        categories = viewModel.categories,
                                        DomesticCOGS = item.BasicPrice,
                                        DomesticRetail = 0,
                                        DomesticSale = item.BasicPrice + item.ComodityPrice,
                                        DomesticWholesale = 0,
                                        InternationalCOGS = 0,
                                        InternationalWholesale = 0,
                                        InternationalRetail = 0,
                                        InternationalSale = 0,
                                        ImageFile = "",
                                        _id = 0,
                                        Username = username,
                                        Token = token,
                                        TotalQty = detail.Quantity
                                    };

                                    string itemsUri = "items/finished-goods/item";
                                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                                    var response = await httpClient.PostAsync($"{APIEndpoint.Core}{itemsUri}", new StringContent(JsonConvert.SerializeObject(itemCore).ToString(), Encoding.UTF8, General.JsonMediaType));

                                    response.EnsureSuccessStatusCode();

                                    var item2 = GetItem2(barcode);

                                    sPKDocsItems.Add(new SPKDocsItem
                                    {
                                        ItemArticleRealizationOrder = viewModel.RONo,
                                        ItemCode = barcode,
                                        ItemDomesticCOGS = item.BasicPrice,
                                        ItemDomesticSale = item.BasicPrice + item.ComodityPrice,
                                        ItemId = item2.FirstOrDefault()._id,
                                        ItemName = viewModel.Comodity.name,
                                        ItemSize = detail.Size.Size,
                                        ItemUom = item.Uom.Unit,
                                        Quantity = detail.Quantity,
                                        Remark = "",
                                        SendQuantity = detail.Quantity,
                                    });
                                }
                                else // barcode sudah terdaftar
                                {
                                    var existItemId = itemx.FirstOrDefault()._id;
                                    ItemCoreViewModelUsername itemCore = new ItemCoreViewModelUsername
                                    {
                                        _id = existItemId,
                                        Username = username,
                                        Token = token,
                                        TotalQty = detail.Quantity
                                    };

                                    sPKDocsItems.Add(new SPKDocsItem
                                    {
                                        ItemArticleRealizationOrder = viewModel.RONo,
                                        ItemCode = barcode,
                                        ItemDomesticCOGS = item.BasicPrice,
                                        ItemDomesticSale = item.BasicPrice + item.ComodityPrice,
                                        ItemId = existItemId,
                                        ItemName = viewModel.Comodity.name,
                                        ItemSize = detail.Size.Size,
                                        ItemUom = item.Uom.Unit,
                                        Quantity = detail.Quantity,
                                        Remark = "",
                                        SendQuantity = detail.Quantity,
                                    });

                                    //update TotalQty di tabel Items
                                    string itemPutUri = $"items/finished-goods/update-qty-by-id/{existItemId}";
                                    IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                                    var response = await httpClient.PutAsync($"{APIEndpoint.Core}{itemPutUri}", new StringContent(JsonConvert.SerializeObject(itemCore).ToString(), Encoding.UTF8, General.JsonMediaType));
                                    if (response != null)
                                    {
                                        response.EnsureSuccessStatusCode();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var sizeId = item.Size.Id.ToString("00");
                            var counterId = viewModel.counters._id.ToString("00");
                            var subCounterId = viewModel.subCounters._id.ToString("00");
                            var asal = viewModel.SourceId.ToString("0");
                            var roCreatedUtc = viewModel.RoCreatedUtc;
                            var materialId = viewModel.materials._id.ToString("00");

                            var barcode = asal + counterId + subCounterId + materialId + sizeId + roCreatedUtc;
                            Console.WriteLine("barcodefad " + barcode);
                            var itemx = GetItem(barcode);

                            if (itemx == null || itemx.Count() == 0) //barcode belum terdaftar, insert ke tabel items (BMS) terlebih dahulu
                            {
                                ItemCoreViewModelUsername itemCore = new ItemCoreViewModelUsername
                                {
                                    dataDestination = new List<ItemViewModelRead>
                                        {
                                           new ItemViewModelRead
                                           {
                                               ArticleRealizationOrder = viewModel.RONo,
                                               code = barcode,
                                               name = viewModel.Comodity.name,
                                               Size = item.Size.Size,
                                               Uom = item.Uom.Unit,
                                               ImagePath = viewModel.ImagePath,
                                               ImgFile = "",
                                               Tags = "",
                                               Remark = "",
                                               Description = "",
                                               _id = 0
                                           }
                                        },
                                    color = viewModel.color,
                                    process = viewModel.process,
                                    materials = viewModel.materials,
                                    materialCompositions = viewModel.materialCompositions,
                                    collections = viewModel.collections,
                                    seasons = viewModel.seasons,
                                    counters = viewModel.counters,
                                    subCounters = viewModel.subCounters,
                                    categories = viewModel.categories,
                                    DomesticCOGS = item.BasicPrice,
                                    DomesticRetail = 0,
                                    DomesticSale = item.BasicPrice + item.ComodityPrice,
                                    DomesticWholesale = 0,
                                    InternationalCOGS = 0,
                                    InternationalWholesale = 0,
                                    InternationalRetail = 0,
                                    InternationalSale = 0,
                                    ImageFile = "",
                                    _id = 0,
                                    Username = username,
                                    Token = token,
                                    TotalQty = item.Quantity
                                };

                                string itemsUri = "items/finished-goods/item";
                                var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                                var response = await httpClient.PostAsync($"{APIEndpoint.Core}{itemsUri}", new StringContent(JsonConvert.SerializeObject(itemCore).ToString(), Encoding.UTF8, General.JsonMediaType));

                                response.EnsureSuccessStatusCode();

                                var item2 = GetItem2(barcode);

                                sPKDocsItems.Add(new SPKDocsItem
                                {
                                    ItemArticleRealizationOrder = viewModel.RONo,
                                    ItemCode = barcode,
                                    ItemDomesticCOGS = item.BasicPrice,
                                    ItemDomesticSale = item.BasicPrice + item.ComodityPrice,
                                    ItemId = item2.FirstOrDefault()._id,
                                    ItemName = viewModel.Comodity.name,
                                    ItemSize = item.Size.Size,
                                    ItemUom = item.Uom.Unit,
                                    Quantity = item.Quantity,
                                    Remark = "",
                                    SendQuantity = item.Quantity,
                                });
                            }
                            else // barcode sudah terdaftar
                            {
                                var existItemId = itemx.FirstOrDefault()._id;
                                ItemCoreViewModelUsername itemCore = new ItemCoreViewModelUsername
                                {
                                    _id = existItemId,
                                    Username = username,
                                    Token = token,
                                    TotalQty = item.Quantity
                                };

                                sPKDocsItems.Add(new SPKDocsItem
                                {
                                    ItemArticleRealizationOrder = viewModel.RONo,
                                    ItemCode = barcode,
                                    ItemDomesticCOGS = item.BasicPrice,
                                    ItemDomesticSale = item.BasicPrice + item.ComodityPrice,
                                    ItemId = existItemId,
                                    ItemName = viewModel.Comodity.name,
                                    ItemSize = item.Size.Size,
                                    ItemUom = item.Uom.Unit,
                                    Quantity = item.Quantity,
                                    Remark = "",
                                    SendQuantity = item.Quantity,
                                });

                                //update TotalQty di tabel Items
                                string itemPutUri = $"items/finished-goods/update-qty-by-id/{existItemId}";
                                IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                                var response = await httpClient.PutAsync($"{APIEndpoint.Core}{itemPutUri}", new StringContent(JsonConvert.SerializeObject(itemCore).ToString(), Encoding.UTF8, General.JsonMediaType));
                                if (response != null)
                                {
                                    response.EnsureSuccessStatusCode();
                                }
                            }
                        }
                    }

                    var packingListCode = GeneratePackingList();

                    SPKDocs data = new SPKDocs()
                    {
                        Code = GenerateCode("BTQ-PK/PBJ"),
                        Date = viewModel.FinishingOutDate,
                        DestinationId = (long)viewModel.DestinationStorageId,
                        DestinationCode = viewModel.DestinationStorageCode,
                        DestinationName = viewModel.DestinationStorageName,
                        IsDistributed = false,
                        IsReceived = false,
                        PackingList = packingListCode,
                        Password = "1",
                        Reference = packingListCode,
                        SourceId = (long)viewModel.SourceStorageId,
                        SourceCode = viewModel.SourceStorageCode,
                        SourceName = viewModel.SourceStorageName,
                        Weight = 0,
                        FinishingOutIdentity = viewModel.FinishingOutIdentity,
                        Items = sPKDocsItems
                    };

                    foreach (var i in data.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                    }
                    EntityExtension.FlagForCreate(data, username, USER_AGENT);
                    dbContext.Add(data);

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

        private List<ItemCoreViewModel> GetItem(string itemCode)
        {
            if (itemCode.Length < 14)
            {
                return null;
            }
            else
            {
                string itemUri = "items/finished-goods/Code";
                IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

                var response = httpClient.GetAsync($"{APIEndpoint.Core}{itemUri}/{itemCode}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    List<ItemCoreViewModel> viewModel = JsonConvert.DeserializeObject<List<ItemCoreViewModel>>(result.GetValueOrDefault("data").ToString());
                    //return viewModel.OrderByDescending(s => s.Date).FirstOrDefault(s => s.Date < doDate.AddDays(1)); ;
                    return viewModel;
                }
                else
                {
                    return null;
                }
            }
        }

        private List<ItemCoreViewModel> GetItem2(string itemCode)
        {
            string itemUri = "items/finished-goods/Code";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Core}{itemUri}/{itemCode}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<ItemCoreViewModel> viewModel = JsonConvert.DeserializeObject<List<ItemCoreViewModel>>(result.GetValueOrDefault("data").ToString());
                //return viewModel.OrderByDescending(s => s.Date).FirstOrDefault(s => s.Date < doDate.AddDays(1)); ;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        //public string GenerateBarcode(int asal, int sizeId, int productId, int counterId, int subCounterId, int motif)
        //{
        //    string code = "" + idx + sizeId;
        //    return code;
        //}

        //public async Task<int> InsertToInventory(SPKDocsFromFinihsingOutsViewModel viewModel, SPKDocItemsFromFinihsingOutsViewModel item, string barcode, long itemId, string username)
        //{
        //    var Inserted = 0;

        //    Inventory inventory = new Inventory()
        //    {
        //        ItemArticleRealizationOrder = viewModel.RONo,
        //        ItemCode = barcode,
        //        ItemDomesticCOGS = item.BasicPrice,
        //        ItemDomesticSale = item.BasicPrice + item.ComodityPrice,
        //        ItemId = itemId,
        //        ItemName = viewModel.Comodity.name,
        //        ItemSize = item.Size.Size,
        //        ItemUom = item.Uom.Unit,
        //        Quantity = item.Quantity,
        //        ItemDomesticRetail = 0,
        //        ItemDomesticWholeSale = 0,
        //        ItemInternationalCOGS = 0,
        //        ItemInternationalRetail = 0,
        //        ItemInternationalSale = 0,
        //        ItemInternationalWholeSale = 0,
        //        StorageId = viewModel.UnitTo.Id,
        //        StorageCode = viewModel.UnitTo.code,
        //        StorageName = viewModel.UnitTo.name,
        //        StorageIsCentral = false,
        //    };

        //    EntityExtension.FlagForCreate(inventory, username, USER_AGENT);
        //    dbContext.Add(inventory);
        //    return Inserted;
        //}

        public string GeneratePackingList() // nomor urut/BTQ-FN/bulan/tahun
        {
            var generatedNo = "";
            var date = DateTime.Now;
            var lastSPKDoc = dbContext.SPKDocs.OrderByDescending(entity => entity.Id).FirstOrDefault(entity => entity.PackingList.Contains("BTQ-FN"));
            string lastPackingListCode = "";

            if (lastSPKDoc != null)
            {
                lastPackingListCode = lastSPKDoc.PackingList;
                var code = lastPackingListCode.Split("/");
                int nomorUrut = int.Parse(code.ElementAt(0));
                nomorUrut++;

                generatedNo = $"{nomorUrut.ToString("0000")}/BTQ-FN/{date.ToString("MM")}/{date.ToString("yy")}";
            }
            else
            {
                generatedNo = $"0001/BTQ-FN/{date.ToString("MM")}/{date.ToString("yy")}";
            }

            return generatedNo;
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

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            throw new NotImplementedException();
        }

        public SPKDocs ReadById(int id)
        {
            throw new NotImplementedException();
        }

        public SPKDocs ReadByReference(string reference)
        {
            throw new NotImplementedException();
        }

        public List<SPKDocs> ReadByFinishingOutIdentity(string FinishingOutIdentity)
        {
            var result = new List<SPKDocs>();
            if(FinishingOutIdentity != null)
            {
                var spkDocs = dbContext.SPKDocs.Where(entity => entity.FinishingOutIdentity == FinishingOutIdentity).FirstOrDefault();
                if(spkDocs != null)
                    result.Add(spkDocs);
            }

            return result;
        }
    }
}
