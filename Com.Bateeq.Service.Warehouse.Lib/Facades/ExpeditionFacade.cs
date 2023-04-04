using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.DanLiris.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel;
using Com.Moonlay.Models;
using Com.Moonlay.NetCore.Lib;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class ExpeditionFacade 
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<Expedition> dbSet;
        private readonly DbSet<Inventory> dbSetInventory;
        private readonly DbSet<SPKDocs> dbSetSpk;
        private readonly DbSet<InventoryMovement> dbSetInventoryMovement;
        private readonly DbSet<TransferOutDoc> dbSetTransfer;
        private readonly IServiceProvider serviceProvider;

        private readonly IMapper mapper;

        public ExpeditionFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<Expedition>();
            this.dbSetInventory = dbContext.Set<Inventory>();
            this.dbSetInventoryMovement = dbContext.Set<InventoryMovement>();
            this.dbSetTransfer = dbContext.Set<TransferOutDoc>();
            this.dbSetSpk = dbContext.Set<SPKDocs>();

            mapper = serviceProvider == null ? null : (IMapper)serviceProvider.GetService(typeof(IMapper));
        }

        public Tuple<List<Expedition>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<Expedition> Query = dbSet
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details);

            List<string> searchAttributes = new List<string>()
            {
                "Code","CreatedBy"
            };

            Query = QueryHelper<Expedition>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<Expedition>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<Expedition>.ConfigureOrder(Query, OrderDictionary);

            Pageable<Expedition> pageable = new Pageable<Expedition>(Query, Page - 1, Size);
            List<Expedition> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<Expedition>, int, Dictionary<string, string>> ReadExpeditionByStore(string token, int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            var permissions = TokenDecrypter.GetPermission(token);
            IQueryable<SPKDocs> QuerySPK = this.dbSetSpk.Include(m => m.Items).Where(i => i.IsDistributed == true && i.IsReceived == false && permissions.Contains(i.SourceCode)  );

            List<long> idSpK = new List<long>();
            foreach(var id in QuerySPK)
            {
                idSpK.Add(id.Id);
            }
            IQueryable<Expedition> Query = dbSet
                .Include(m => m.Items)
                    .ThenInclude(i => i.Details).Where(h => h.Items.Any(i => idSpK.Contains(i.SPKDocsId)));
             
            List<string> searchAttributes = new List<string>()
            {
                "Code","CreatedBy"
            };

            Query = QueryHelper<Expedition>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<Expedition>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<Expedition>.ConfigureOrder(Query, OrderDictionary);

            Pageable<Expedition> pageable = new Pageable<Expedition>(Query, Page - 1, Size);
            List<Expedition> Data = pageable.Data.ToList();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }
        public IQueryable<ExpeditionReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int offset, string username)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.Expeditions
                         join b in dbContext.ExpeditionItems on a.Id equals b.ExpeditionId
                         join c in dbContext.ExpeditionDetails on b.Id equals c.ExpeditionItemId
                         where a.IsDeleted == false
                             && b.IsDeleted == false
                             && c.IsDeleted == false
                             && b.DestinationCode == (string.IsNullOrWhiteSpace(destinationCode) ? b.DestinationCode : destinationCode)
                             && a.CreatedBy == (string.IsNullOrWhiteSpace(username) ? a.CreatedBy : username)
                             // && a.Code == (string.IsNullOrWhiteSpace(code) ? a.Code : code)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                             && b.IsReceived == status
                             && (transaction == 0 ? (!b.Reference.Contains("BTQ-KB/RTP") && !b.Reference.Contains("BTQ-KB/RTU")) : (b.Reference.Contains("BTQ-KB/RTP") || b.Reference.Contains("BTQ-KB/RTU")))
                             && b.PackingList.Contains(string.IsNullOrWhiteSpace(packingList) ? b.PackingList : packingList)

                         select new ExpeditionReportViewModel
                         {
                             code = a.Code,
                             expeditionServiceName = a.ExpeditionServiceName,
                             date = a.Date,
                             sourceCode = b.SourceCode,
                             sourceName = b.SourceName,
                             destinationCode = b.DestinationCode,
                             destinationName = b.DestinationName,
                             isReceived = b.IsReceived,
                             //isDistributed = b.IsDistributed,
                             packingList = b.PackingList,
                             password = b.Password,
                             itemCode = c.ItemCode,
                             itemName = c.ItemName,
                             itemSize = c.Size,
                             itemUom = c.Uom,
                             itemArticleRealizationOrder = c.ArticleRealizationOrder,
                             Quantity = c.Quantity,
                             itemDomesticSale = c.DomesticSale,
                             LastModifiedUtc = b.LastModifiedUtc
                         });
            //return Query;
            return Query.AsQueryable();
        }

        public Tuple<List<ExpeditionReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int page, int size, string Order, int offset, string username)
        {
            var Query = GetReportQuery(dateFrom, dateTo, destinationCode, status, transaction, packingList, offset, username);

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

            Pageable<ExpeditionReportViewModel> pageable = new Pageable<ExpeditionReportViewModel>(Query, page - 1, size);
            List<ExpeditionReportViewModel> Data = pageable.Data.ToList<ExpeditionReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int offset, string username)
        {
            var Query = GetReportQuery(dateFrom, dateTo, destinationCode, status, transaction, packingList, offset, username);
            Query = Query.OrderByDescending(b => b.LastModifiedUtc);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal


            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Code", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sumber Penyimpanan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan Penyimpanan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Packing List", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Deskripsi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Size", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Diterima", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Ekspedisi", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Diminta", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Satuan Diminta", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang PO Eksternal", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Deal PO Eksternal", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Satuan Deal PO Eksternal", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Status PR", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Status Barang", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", "", 0, ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.date == null ? "-" : item.date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string prDate = item.expectedDeliveryDatePR == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePR.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDate = item.expectedDeliveryDatePO == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePO.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.date, item.code, item.sourceName, item.destinationName, item.packingList, item.itemCode, item.itemName, item.itemSize, item.itemArticleRealizationOrder, item.itemUom, item.Quantity, item.isReceived ? "Diterima" : "Belum Diterima");
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        public Expedition ReadById(int id)
        {
            var model = dbSet.Where(m => m.Id == id)
                 .Include(m => m.Items).ThenInclude(i => i.Details)
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

        public async Task<int> Create(Expedition model, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;
            
            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    double totalweight = 0;
                    string code = GenerateCode("BTQ-KB/EXP");
                    
                    model.Code = code;
                    model.Date = DateTimeOffset.Now;
                    foreach (var i in model.Items)
                    {
                        TransferOutDoc transferOutDoc = new TransferOutDoc();
                        i.Id = 0;
                        totalweight += i.Weight;
                        string CodeTransferOut = GenerateCode("BTQ-KB/EXP");
                        var SPK = dbContext.SPKDocs.Where(x => x.PackingList == i.PackingList).Single();
                        SPK.IsDistributed = true;
                        transferOutDoc.Code = CodeTransferOut;
                        transferOutDoc.Reference = model.Code;
                        transferOutDoc.DestinationId = i.DestinationId;
                        transferOutDoc.DestinationCode = i.DestinationCode;
                        transferOutDoc.DestinationName = i.DestinationName;
                        transferOutDoc.Remark = model.Remark;
                        transferOutDoc.SourceId = i.SourceId;
                        transferOutDoc.SourceCode = i.SourceCode;
                        transferOutDoc.SourceName = i.SourceName;
                        List<TransferOutDocItem> transferOutDocItems = new List<TransferOutDocItem>();
                        foreach (var d in i.Details)
                        {
                            d.Id = 0;
                            var inven = dbContext.Inventories.Where(x => x.ItemArticleRealizationOrder == d.ArticleRealizationOrder && x.ItemCode == d.ItemCode && x.ItemName == d.ItemName && x.StorageId == i.SourceId).Single();

                            InventoryMovement movement = new InventoryMovement
                            {
                                After = inven.Quantity - d.SendQuantity,
                                Before = inven.Quantity,
                                Date = DateTimeOffset.Now,
                                ItemArticleRealizationOrder = d.ArticleRealizationOrder,
                                ItemCode = d.ItemCode,
                                ItemDomesticCOGS = d.DomesticCOGS,
                                ItemDomesticRetail = d.DomesticRetail,
                                ItemDomesticSale = d.DomesticSale,
                                ItemDomesticWholeSale = d.DomesticWholesale,
                                ItemInternationalCOGS = 0,
                                ItemInternationalRetail = 0,
                                ItemInternationalSale = 0,
                                ItemInternationalWholeSale = 0,
                                ItemId = d.ItemId,
                                ItemName = d.ItemName,
                                ItemSize = d.Size,
                                Quantity = d.SendQuantity,
                                Reference = CodeTransferOut,
                                Remark = d.Remark,
                                StorageCode = i.SourceCode,
                                StorageIsCentral = i.SourceName.Contains("GUDANG") ? true : false,
                                StorageId = i.SourceId,
                                StorageName = i.SourceName,
                                Type = "OUT"
                            };

                            inven.Quantity = inven.Quantity - d.SendQuantity;
                            TransferOutDocItem transferItem = new TransferOutDocItem
                            {
                                ArticleRealizationOrder = d.ArticleRealizationOrder,
                                DomesticCOGS = d.DomesticCOGS,
                                DomesticRetail = d.DomesticRetail,
                                DomesticSale = d.DomesticSale,
                                DomesticWholeSale = d.DomesticWholesale,
                                ItemCode = d.ItemCode,
                                ItemId = d.ItemId,
                                ItemName = d.ItemName,
                                Quantity = d.SendQuantity,
                                Remark = d.Remark,
                                Size = d.Size,
                                Uom = d.Uom
                            };
                            transferOutDocItems.Add(transferItem);
                            //transferOutDoc.Items.Add(transferItem);
                            //transferOutDoc.Items.Add(new TransferOutDocItem
                            //{
                            //    ArticleRealizationOrder = d.ArticleRealizationOrder
                            //    DomesticCOGS = d.DomesticCOGS,
                            //    DomesticRetail = d.DomesticRetail,
                            //    DomesticSale = d.DomesticSale,
                            //    DomesticWholeSale = d.DomesticWholesale,
                            //    ItemCode = d.ItemCode,
                            //    ItemId = d.ItemId,
                            //    ItemName = d.ItemName,
                            //    Quantity = d.Quantity,
                            //    Remark = d.Remark,
                            //    Size = d.Size,
                            //    Uom = d.Uom


                            //});
                            EntityExtension.FlagForCreate(d, username, USER_AGENT);
                            EntityExtension.FlagForCreate(movement, username, USER_AGENT);
                            this.dbSetInventoryMovement.Add(movement);
                        }
                        EntityExtension.FlagForCreate(transferOutDoc, username, USER_AGENT);
                        this.dbSetTransfer.Add(transferOutDoc);

                        transferOutDoc.Items = transferOutDocItems;
                        foreach (var trfOut in transferOutDocItems)
                        {
                            EntityExtension.FlagForCreate(trfOut, username, USER_AGENT);
                        }

                        EntityExtension.FlagForCreate(i, username, USER_AGENT);

                    }
                    model.Weight = totalweight;
                    model.Remark = "";
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

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
