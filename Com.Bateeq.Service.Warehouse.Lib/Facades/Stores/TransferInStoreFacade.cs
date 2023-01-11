using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Utilities;
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

namespace Com.Bateeq.Service.Warehouse.Lib.Facades.Stores
{
    public class TransferInStoreFacade
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<TransferInDoc> dbSet;
        private readonly DbSet<SPKDocs> dbSetSpk;
        private readonly IServiceProvider serviceProvider;
        private readonly DbSet<Inventory> dbSetInventory;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;

        public TransferInStoreFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TransferInDoc>();
            this.dbSetInventory = dbContext.Set<Inventory>();
            this.dbSetSpk = dbContext.Set<SPKDocs>();
            this.dbSetInventoryMovement = dbContext.Set<InventoryMovement>();
        }

        public Tuple<List<TransferInDoc>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<TransferInDoc> Query = this.dbSet.Include(m => m.Items).Where(x=>x.SourceCode.Contains("GDG.")).OrderByDescending(x=>x.Date);

            List<string> searchAttributes = new List<string>()
            {
                "Code","DestinationName","SourceName","Reference"
            };

            Query = QueryHelper<TransferInDoc>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<TransferInDoc>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<TransferInDoc>.ConfigureOrder(Query, OrderDictionary);

            Pageable<TransferInDoc> pageable = new Pageable<TransferInDoc>(Query, Page - 1, Size);
            List<TransferInDoc> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        //public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPendingStore(string destinationName,int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        //      {
        //	String[] strlist = destinationName.Split(";");

        //	List<string> destName = new List<string>();
        //	foreach (String s in strlist)
        //	{
        //		destName.Add(s);
        //	}
        //	IQueryable<SPKDocs> Query = this.dbSetSpk.Include(m => m.Items).Where(i=>i.IsDistributed == true && i.IsReceived == false && destName.Contains(i.DestinationCode) && i.DestinationCode != "GDG.01" );


        //	List<string> searchAttributes = new List<string>()
        //          {
        //              "Code","DestinationName","SourceName","Reference","PackingList"
        //          };

        //          foreach(var i in Query)
        //          {
        //              if (/*i.Reference != null || i.Reference != ""*/ !String.IsNullOrWhiteSpace(i.Reference) && i.Reference.Contains("RTT"))
        //              {
        //                  var transferout = dbContext.TransferOutDocs.Where(x => x.Code == i.Reference).FirstOrDefault();
        //                  if (transferout != null)
        //                  {
        //                      i.SourceId = transferout.SourceId;
        //                      i.SourceCode = transferout.SourceCode;
        //                      i.SourceName = transferout.SourceName;
        //                      i.DestinationId = transferout.DestinationId;
        //                      i.DestinationName = transferout.DestinationName;
        //                      i.DestinationCode = transferout.DestinationCode;
        //                  }
        //                  else
        //                  {
        //                      i.SourceId = 0;
        //                      i.SourceCode = "-";
        //                      i.SourceName = "-";
        //                      i.DestinationId = 0;
        //                      i.DestinationName = "-";
        //                      i.DestinationCode = "-";
        //                  }
        //              }
        //          }

        //          Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

        //          Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
        //          Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

        //          Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //          Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

        //          Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
        //          List<SPKDocs> Data = pageable.Data.ToList();
        //          int TotalData = pageable.TotalCount;

        //          return Tuple.Create(Data, TotalData, OrderDictionary);
        //      }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPendingStore(string token, int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            var stores = TokenDecrypter.GetStore(token);

            IQueryable<SPKDocs> Query = this.dbSetSpk.Include(m => m.Items).Where(i => i.IsDistributed == true && i.IsReceived == false && stores.Contains(i.DestinationCode) && i.DestinationCode != "GDG.01");

            List<string> searchAttributes = new List<string>()
            {
                "Code","DestinationName","SourceName","Reference","PackingList"
            };

            foreach (var i in Query)
            {
                if (/*i.Reference != null || i.Reference != ""*/ !String.IsNullOrWhiteSpace(i.Reference) && i.Reference.Contains("RTT"))
                {
                    var transferout = dbContext.TransferOutDocs.Where(x => x.Code == i.Reference).FirstOrDefault();
                    if (transferout != null)
                    {
                        i.SourceId = transferout.SourceId;
                        i.SourceCode = transferout.SourceCode;
                        i.SourceName = transferout.SourceName;
                        i.DestinationId = transferout.DestinationId;
                        i.DestinationName = transferout.DestinationName;
                        i.DestinationCode = transferout.DestinationCode;
                    }
                    else
                    {
                        i.SourceId = 0;
                        i.SourceCode = "-";
                        i.SourceName = "-";
                        i.DestinationId = 0;
                        i.DestinationName = "-";
                        i.DestinationCode = "-";
                    }
                }
            }

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPending(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
		{
			IQueryable<SPKDocs> Query = this.dbSetSpk.Include(m => m.Items).Where(i => i.IsDistributed == true && i.IsReceived == false);

			List<string> searchAttributes = new List<string>()
			{
				"Code","DestinationName","SourceName","Reference","PackingList"
			};

			foreach (var i in Query)
			{
				if (/*i.Reference != null || i.Reference != ""*/ !String.IsNullOrWhiteSpace(i.Reference) && i.Reference.Contains("RTT"))
				{
					var transferout = dbContext.TransferOutDocs.Where(x => x.Code == i.Reference).FirstOrDefault();
					if (transferout != null)
					{
						i.SourceId = transferout.SourceId;
						i.SourceCode = transferout.SourceCode;
						i.SourceName = transferout.SourceName;
						i.DestinationId = transferout.DestinationId;
						i.DestinationName = transferout.DestinationName;
						i.DestinationCode = transferout.DestinationCode;
					}
					else
					{
						i.SourceId = 0;
						i.SourceCode = "-";
						i.SourceName = "-";
						i.DestinationId = 0;
						i.DestinationName = "-";
						i.DestinationCode = "-";
					}
				}
			}

			Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

			Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
			Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

			Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
			List<SPKDocs> Data = pageable.Data.ToList();
			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData, OrderDictionary);
		}

		public TransferInDoc ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                 .Include(m => m.Items)
                 .FirstOrDefault();
            return model;
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

        public async Task<int> Create(TransferInDoc model, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    string code = GenerateCode("BTQ-TB/BBT");
                    model.Code = code;
                    var SPK = dbContext.SPKDocs.Where(x => x.PackingList == model.Reference).Single();
                    var ExpItem = dbContext.ExpeditionItems.Where(x => x.SPKDocsId == SPK.Id).Single();

                    var Id = SPK.Id;
                    //var ExpeditionCode = (from a in dbContext.Expeditions
                    //                      join b in dbContext.ExpeditionItems on a.Id equals b.ExpeditionId
                    //                      where b.SPKDocsId == SPK.Id
                    //                      select a.Code).Single();
                    
                    ExpItem.IsReceived = true;
                    SPK.IsReceived = true;

                    List<InventoryMovement> inventoryMovement = new List<InventoryMovement>();
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

                    foreach (var i in model.Items)
                    {
                        i.Id = 0;
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);

                        var SPKItems = dbContext.SPKDocsItems.Where(x => x.ItemArticleRealizationOrder == i.ArticleRealizationOrder && x.ItemCode == i.ItemCode && i.ItemName == i.ItemName && x.SPKDocsId == Id).Single();
                        //SPKItems.SendQuantity = i.Quantity;

                        //var invenSource = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == SPK.SourceId && x.IsDeleted == false).FirstOrDefault();
                        var invenDestination = dbContext.Inventories.Where(x => x.ItemId == i.ItemId && x.StorageId == SPK.DestinationId && x.IsDeleted == false).FirstOrDefault();
                        
                        //if (invenSource != null) {
                        //    if (invenSource.Quantity >= SPKItems.SendQuantity) {
                        //        var movement = new InventoryMovement
                        //        {
                        //            After = invenSource.Quantity - SPKItems.SendQuantity,
                        //            Before = invenSource.Quantity,
                        //            Date = DateTimeOffset.Now,
                        //            ItemArticleRealizationOrder = SPKItems.ItemArticleRealizationOrder,
                        //            ItemCode = SPKItems.ItemCode,
                        //            ItemDomesticCOGS = SPKItems.ItemDomesticCOGS,
                        //            ItemDomesticRetail = SPKItems.ItemDomesticRetail,
                        //            ItemDomesticSale = SPKItems.ItemDomesticSale,
                        //            ItemDomesticWholeSale = SPKItems.ItemDomesticWholesale,
                        //            ItemInternationalCOGS = 0,
                        //            ItemInternationalRetail = 0,
                        //            ItemInternationalSale = 0,
                        //            ItemInternationalWholeSale = 0,
                        //            ItemId = SPKItems.ItemId,
                        //            ItemName = SPKItems.ItemName,
                        //            ItemSize = SPKItems.ItemSize,
                        //            ItemUom = SPKItems.ItemUom,
                        //            Quantity = SPKItems.SendQuantity,
                        //            Reference = ExpeditionCode,
                        //            Remark = SPKItems.Remark,
                        //            StorageIsCentral = SPK.SourceName.Contains("GUDANG") ? true : false,
                        //            StorageId = SPK.SourceId,
                        //            StorageCode = SPK.SourceCode,
                        //            StorageName = SPK.SourceName,
                        //            Type = "OUT"
                        //        };

                        //        inventoryMovement.Add(movement);
                        //        invenSource.Quantity = invenSource.Quantity - SPKItems.SendQuantity;
                        //    }
                        //}

                        if(invenDestination != null)
                        {
                            var movementInStock = new InventoryMovement
                            {
                                After = invenDestination.Quantity + SPKItems.SendQuantity,
                                Before = invenDestination.Quantity,
                                Date = DateTimeOffset.Now,
                                ItemArticleRealizationOrder = SPKItems.ItemArticleRealizationOrder,
                                ItemCode = SPKItems.ItemCode,
                                ItemDomesticCOGS = SPKItems.ItemDomesticCOGS,
                                ItemDomesticRetail = SPKItems.ItemDomesticRetail,
                                ItemDomesticSale = SPKItems.ItemDomesticSale,
                                ItemDomesticWholeSale = SPKItems.ItemDomesticWholesale,
                                ItemInternationalCOGS = 0,
                                ItemInternationalRetail = 0,
                                ItemInternationalSale = 0,
                                ItemInternationalWholeSale = 0,
                                ItemId = SPKItems.ItemId,
                                ItemName = SPKItems.ItemName,
                                ItemSize = SPKItems.ItemSize,
                                ItemUom = SPKItems.ItemUom,
                                Quantity = SPKItems.SendQuantity,
                                Reference = code,
                                Remark = SPKItems.Remark,
                                StorageIsCentral = SPK.DestinationName.Contains("GUDANG") ? true : false,
                                StorageId = SPK.DestinationId,
                                StorageCode = SPK.DestinationCode,
                                StorageName = SPK.DestinationName,
                                Type = "IN"
                            };

                            inventoryMovement.Add(movementInStock);
                            invenDestination.Quantity = invenDestination.Quantity + SPKItems.SendQuantity;

                        }
                        else
                        {
                            var movementInNew = new InventoryMovement
                            {
                                After = SPKItems.SendQuantity,
                                Before = 0,
                                Date = DateTimeOffset.Now,
                                ItemArticleRealizationOrder = SPKItems.ItemArticleRealizationOrder,
                                ItemCode = SPKItems.ItemCode,
                                ItemDomesticCOGS = SPKItems.ItemDomesticCOGS,
                                ItemDomesticRetail = SPKItems.ItemDomesticRetail,
                                ItemDomesticSale = SPKItems.ItemDomesticSale,
                                ItemDomesticWholeSale = SPKItems.ItemDomesticWholesale,
                                ItemInternationalCOGS = 0,
                                ItemInternationalRetail = 0,
                                ItemInternationalSale = 0,
                                ItemInternationalWholeSale = 0,
                                ItemId = SPKItems.ItemId,
                                ItemName = SPKItems.ItemName,
                                ItemSize = SPKItems.ItemSize,
                                ItemUom = SPKItems.ItemUom,
                                Quantity = SPKItems.SendQuantity,
                                Reference = code,
                                Remark = SPKItems.Remark,
                                StorageIsCentral = SPK.DestinationName.Contains("GUDANG") ? true : false,
                                StorageId = SPK.DestinationId,
                                StorageCode = SPK.DestinationCode,
                                StorageName = SPK.DestinationName,
                                Type = "IN"
                            };

                            inventoryMovement.Add(movementInNew);

                            var InvenIn = new Inventory
                            {
                                ItemArticleRealizationOrder = SPKItems.ItemArticleRealizationOrder,
                                ItemCode = SPKItems.ItemCode,
                                ItemDomesticCOGS = SPKItems.ItemDomesticCOGS,
                                ItemDomesticRetail = SPKItems.ItemDomesticRetail,
                                ItemDomesticSale = SPKItems.ItemDomesticSale,
                                ItemDomesticWholeSale = SPKItems.ItemDomesticWholesale,
                                ItemInternationalCOGS = 0,
                                ItemInternationalRetail = 0,
                                ItemInternationalSale = 0,
                                ItemInternationalWholeSale = 0,
                                ItemId = SPKItems.ItemId,
                                ItemName = SPKItems.ItemName,
                                ItemSize = SPKItems.ItemSize,
                                ItemUom = SPKItems.ItemUom,
                                Quantity = SPKItems.SendQuantity,
                                StorageIsCentral = SPK.DestinationName.Contains("GUDANG") ? true : false,
                                StorageId = SPK.DestinationId,
                                StorageCode = SPK.DestinationCode,
                                StorageName = SPK.DestinationName,
                            };

                            EntityExtension.FlagForCreate(InvenIn, username, USER_AGENT);
                            dbSetInventory.Add(InvenIn);   
                        }
                    }

                    #region save
                    foreach(var i in inventoryMovement)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        dbSetInventoryMovement.Add(i);
                    }
                    #endregion

                    dbSet.Add(model);
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
    }
}
