using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class InventoryFacade
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<Inventory> dbSet;
        private readonly DbSet<InventoryMovement> dbSetMovement;
        public readonly IServiceProvider serviceProvider;

       // private readonly string GarmentPreSalesContractUri = "merchandiser/garment-pre-sales-contracts/";

        public InventoryFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<Inventory>();
            this.dbSetMovement = dbContext.Set<InventoryMovement>();
        }

        public IQueryable<InventoryViewModel> GetQuery(string itemCode, string storageCode)
        {
            //GarmentCorrectionNote garmentCorrectionNote = new GarmentCorrectionNote();
            //var garmentCorrectionNotes = dbContext.Set<GarmentCorrectionNote>().AsQueryable();



            var Query = (from a in dbContext.Inventories


                         where
                         a.ItemCode == itemCode
                         && a.StorageCode == storageCode
                         //&& z.CodeRequirment == (string.IsNullOrWhiteSpace(category) ? z.CodeRequirment : category)


                         select new InventoryViewModel
                         {
                             item = new ViewModels.NewIntegrationViewModel.ItemViewModel {
                                 code = a.ItemCode,
                                 articleRealizationOrder = a.ItemArticleRealizationOrder

                             }, //a.ItemCode,
                             //ItemArticleRealization = a.ItemArticleRealizationOrder,
                             //ItemDomesticCOGS = a.ItemDomesticCOGS,
                             quantity = a.Quantity
                                
                             //Price = a.Price

                         });

            return Query;
        }

        public Tuple<List<InventoryViewModel>, int> GetItemPack(string itemCode, string storageCode, string order, int page, int size)
        {
            var Query = GetQuery(itemCode, storageCode);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            //if (OrderDictionary.Count.Equals(0))
            //{
            //	Query = Query.OrderByDescending(b => b.poExtDate);
            //}

            Pageable<InventoryViewModel> pageable = new Pageable<InventoryViewModel>(Query, page - 1, size);
            List<InventoryViewModel> Data = pageable.Data.ToList<InventoryViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public List<Inventory> getDatabyCode(string itemCode, int StorageId)
        {
            var inventory = dbSet.Where(x => x.ItemCode == itemCode && x.StorageId == StorageId).ToList();
            return inventory;

        }

        public List<Inventory> getDatabyName(string itemName, int StorageId)
        {
            var inventory = dbSet.Where(x => x.ItemName ==itemName && x.StorageId == StorageId).ToList();
            return inventory;

        }

        public Inventory getStock(int source, int item)
        {
            var inventory = dbSet.Where(x => x.StorageId == source && x.ItemId == item).FirstOrDefault();
            return inventory;
        }

        #region Monitoring By User
        public IQueryable<InventoriesReportViewModel> GetReportQuery(string storageId, string filter)
        {
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            //DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.Inventories
                         where a.IsDeleted == false
                         && a.StorageId == Convert.ToInt64((string.IsNullOrWhiteSpace(storageId) ? a.StorageId.ToString() :  storageId))
                         //&& a.StorageCode == (string.IsNullOrWhiteSpace(storageId) ? a.StorageCode : storageId)
                         //&& a.ItemName.Contains((string.IsNullOrWhiteSpace(filter) ? a.ItemName : filter))
                         //|| a.ItemArticleRealizationOrder.Contains((string.IsNullOrWhiteSpace(filter) ? a.ItemArticleRealizationOrder : filter))

                         select new InventoriesReportViewModel
                         {
                             ItemCode = a.ItemCode,
                             ItemName = a.ItemName,
                             ItemArticleRealizationOrder = a.ItemArticleRealizationOrder,
                             ItemSize = a.ItemSize,
                             ItemUom = a.ItemUom,
                             ItemDomesticSale = a.ItemDomesticSale,
                             Quantity = a.Quantity,
                             StorageId = a.StorageId,
                             StorageCode = a.StorageCode,
                             StorageName = a.StorageName
                         });

            var Query2 = (from a in Query
                          where a.ItemName.Contains((string.IsNullOrWhiteSpace(filter) ? a.ItemName : filter))
                          || a.ItemArticleRealizationOrder.Contains((string.IsNullOrWhiteSpace(filter) ? a.ItemArticleRealizationOrder : filter))
                          || a.ItemCode.Contains((string.IsNullOrWhiteSpace(filter) ? a.ItemArticleRealizationOrder : filter))

                          select new InventoriesReportViewModel
                          {
                              ItemCode = a.ItemCode,
                              ItemName = a.ItemName,
                              ItemArticleRealizationOrder = a.ItemArticleRealizationOrder,
                              ItemSize = a.ItemSize,
                              ItemUom = a.ItemUom,
                              ItemDomesticSale = a.ItemDomesticSale,
                              Quantity =  a.Quantity,
                              StorageId = a.StorageId,
                              StorageCode = a.StorageCode,
                              StorageName = a.StorageName
                          });

            return Query2;

        }

        //public Tuple<List<InventoryReportViewModel>, int> GetReport(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset, string username)
        public Tuple<List<InventoriesReportViewModel>, int> GetReport(string storageId, string filter, int page, int size, string Order, int offset, string username)
        {
            var Query = GetReportQuery(storageId, filter);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            // Pageable<InventoriesReportViewModel> pageable = new Pageable<InventoriesReportViewModel>(Query, page - 1, size);
            List<InventoriesReportViewModel> Data = Query.ToList<InventoriesReportViewModel>();
            return Tuple.Create(Data, Data.Count());
        }


        public MemoryStream GenerateExcelReportByUser(string storecode, string filter)
        {
            var Query = GetReportQuery(storecode, filter);
            // Query = Query.OrderByDescending(a => a.ReceiptDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Subtotal", DataType = typeof(String) });
           
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, 0, 0, 0, 0, 0);
            // to allow column name to be generated properly for empty data as template
            else
            {
                Dictionary<string, List<InventoriesReportViewModel>> dataByToko = new Dictionary<string, List<InventoriesReportViewModel>>();
                Dictionary<string, double> subTotalQty = new Dictionary<string, double>();
                Dictionary<string, double> subTotalAmount = new Dictionary<string, double>();

                foreach (InventoriesReportViewModel item in Query.ToList())
                {
                    string StoreName = item.StorageName;

                    if (!dataByToko.ContainsKey(StoreName)) dataByToko.Add(StoreName, new List<InventoriesReportViewModel> { });
                    dataByToko[StoreName].Add(new InventoriesReportViewModel
                    {
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        ItemArticleRealizationOrder = item.ItemArticleRealizationOrder,
                        ItemSize = item.ItemSize,
                        ItemUom = item.ItemUom,
                        ItemDomesticSale = item.ItemDomesticSale,
                        Quantity = item.Quantity,
                        StorageId = item.StorageId,
                        StorageCode = item.StorageCode,
                        StorageName = item.StorageName
                    });

                    if (!subTotalQty.ContainsKey(StoreName))
                    {
                        subTotalQty.Add(StoreName, 0);
                    };

                    if (!subTotalAmount.ContainsKey(StoreName))
                    {
                        subTotalAmount.Add(StoreName, 0);
                    };

                    double Quantity = item.Quantity;
                    double Amount = item.Quantity * item.ItemDomesticSale;

                    subTotalQty[StoreName] += Quantity;
                    subTotalAmount[StoreName] += Amount;
                }
                
                foreach (KeyValuePair<string, List<InventoriesReportViewModel>> StoreName in dataByToko)
                {
                    int index = 0;

                    result.Rows.Add("", "", "", "", "", "", "Total Quantity : " + Math.Round(subTotalQty[StoreName.Key], 2), "", "Total Rupiah : " + Math.Round(subTotalAmount[StoreName.Key], 2));

                    foreach (InventoriesReportViewModel item in StoreName.Value)
                    {
                        index++;
                        // string date = item.Date == null ? "-" : item.Date.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        //string pr_date = item.PRDate == null ? "-" : item.PRDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        //string do_date = item.DODate == null ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        
                        result.Rows.Add(index, item.StorageCode, item.StorageName, item.ItemCode, item.ItemName, item.ItemArticleRealizationOrder, item.Quantity, item.ItemDomesticSale, item.Quantity * item.ItemDomesticSale);
                        
                    }
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Data") }, true);
        }
        #endregion

        #region Monitoring By Search
        public IQueryable<InventoriesReportViewModel> GetSearchQuery(string itemCode, int offset, string username)
        {
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            //DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.Inventories
                         where a.IsDeleted == false
                         && a.ItemCode == (string.IsNullOrWhiteSpace(itemCode) ? a.ItemCode : itemCode)

                         select new InventoriesReportViewModel
                         {
                             ItemCode = a.ItemCode,
                             ItemName = a.ItemName,
                             ItemArticleRealizationOrder = a.ItemArticleRealizationOrder,
                             ItemSize = a.ItemSize,
                             ItemUom = a.ItemUom,
                             ItemDomesticSale = a.ItemDomesticSale,
                             Quantity = a.Quantity,
                             StorageId = a.StorageId,
                             StorageCode = a.StorageCode,
                             StorageName = a.StorageName
                         });
            return Query;
        }

        //public Tuple<List<InventoryReportViewModel>, int> GetReport(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset, string username)
        public Tuple<List<InventoriesReportViewModel>, int> GetSearch(string itemCode, int page, int size, string Order, int offset, string username)
        {
            var Query = GetSearchQuery(itemCode, offset, username);

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

            // Pageable<InventoriesReportViewModel> pageable = new Pageable<InventoriesReportViewModel>(Query, page - 1, size);
            List<InventoriesReportViewModel> Data = Query.ToList<InventoriesReportViewModel>();
            // int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, Data.Count());
        }

        public MemoryStream GenerateReportBySearchExcel(string itemCode, int offset, string username)
        {
            var Query = GetSearchQuery(itemCode, offset, username);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                
                Dictionary<string, List<InventoriesReportViewModel>> dataByToko = new Dictionary<string, List<InventoriesReportViewModel>>();
                Dictionary<string, double> subTotalQty = new Dictionary<string, double>();

                foreach (InventoriesReportViewModel item in Query.ToList())
                {
                    string ItemCode = item.ItemCode;

                    if (!dataByToko.ContainsKey(ItemCode)) dataByToko.Add(ItemCode, new List<InventoriesReportViewModel> { });
                    dataByToko[ItemCode].Add(new InventoriesReportViewModel
                    {
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        ItemArticleRealizationOrder = item.ItemArticleRealizationOrder,
                        ItemSize = item.ItemSize,
                        ItemUom = item.ItemUom,
                        ItemDomesticSale = item.ItemDomesticSale,
                        Quantity = item.Quantity,
                        StorageId = item.StorageId,
                        StorageCode = item.StorageCode,
                        StorageName = item.StorageName
                    });

                    if (!subTotalQty.ContainsKey(ItemCode))
                    {
                        subTotalQty.Add(ItemCode, 0);
                    };

                    double Quantity = item.Quantity;

                    subTotalQty[ItemCode] += Quantity;
                }

                foreach (KeyValuePair<string, List<InventoriesReportViewModel>> ItemCode in dataByToko)
                {
                    int index = 0;

                    result.Rows.Add("", "", "Total : " + Math.Round(subTotalQty[ItemCode.Key], 2));

                    foreach (InventoriesReportViewModel item in ItemCode.Value)
                    {
                        index++;
                        
                        result.Rows.Add(index, item.StorageName, item.Quantity);
                    }
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Barang") }, true);
        }
        
        #endregion

        public Inventory getStockPOS(string sourcecode, string itemCode)
        {
            var inventory = dbSet.Where(x => x.StorageCode == sourcecode && x.ItemCode == itemCode).FirstOrDefault();
            return inventory;

        }

        #region Monitoring Inventory Movements
        public IQueryable<InventoryMovementsReportViewModel> GetMovementQuery(string storageId, string itemCode)
        {
            //DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            //DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from c in dbContext.InventoryMovements
                         where c.IsDeleted == false
                         //&& c.StorageId == (string.IsNullOrWhiteSpace(storageId) ? c.StorageId : storageId)
                         && c.StorageId == Convert.ToInt64((string.IsNullOrWhiteSpace(storageId) ? c.StorageId.ToString() : storageId))
                         && c.ItemCode == (string.IsNullOrWhiteSpace(itemCode) ? c.ItemCode : itemCode)
                         //&& a.ItemName == (string.IsNullOrWhiteSpace(info) ? a.ItemName : info)

                         select new InventoryMovementsReportViewModel
                         {
                             Date = c.Date,
                             ItemCode = c.ItemCode,
                             ItemName = c.ItemName,
                             ItemArticleRealizationOrder = c.ItemArticleRealizationOrder,
                             ItemSize = c.ItemSize,
                             ItemUom = c.ItemUom,
                             ItemDomesticSale = c.ItemDomesticSale,
                             Quantity = c.Type == "OUT"? -c.Quantity : c.Quantity,
                             Before = c.Before,
                             After = c.After,
                             Type = c.Type,
                             Reference = c.Reference,
                             Remark = c.Remark,
                             StorageId = c.StorageId,
                             StorageCode = c.StorageCode,
                             StorageName = c.StorageName,
                             CreatedUtc = c.CreatedUtc,
                         });
            return Query;
        }

        //public Tuple<List<InventoryReportViewModel>, int> GetReport(string no, string unitId, string categoryId, string budgetId, string prStatus, string poStatus, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset, string username)
        public Tuple<List<InventoryMovementsReportViewModel>, int> GetMovements(string storageId, string itemCode, string info, string Order, int offset, string username, int page = 1, int size = 25)
        {
            var Query = GetMovementQuery(storageId, itemCode);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            if (OrderDictionary.Count.Equals(0))
            {
                Query = Query.OrderBy(b => b.LastModifiedUtc);
            }
            else
            {
                string Key = OrderDictionary.Keys.First();
                string OrderType = OrderDictionary[Key];

                Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
            }

            //Pageable<InventoryMovementsReportViewModel> pageable = new Pageable<InventoryMovementsReportViewModel>(Query, page - 1, size);
            //List<InventoriesReportViewModel> Data = Query.ToList<InventoriesReportViewModel>();
            List<InventoryMovementsReportViewModel> Data = Query.ToList<InventoryMovementsReportViewModel>();
            int TotalData = Query.Count();

            //return Tuple.Create(Data, Data.Count());
            return Tuple.Create(Data, TotalData);

        }


        public MemoryStream GenerateExcelReportByMovement(string storecode, string itemCode)
        {
            var Query = GetMovementQuery(storecode, itemCode);
            // Query = Query.OrderByDescending(a => a.ReceiptDate);
            DataTable result = new DataTable();
            //No	Unit	Budget	Kategori	Tanggal PR	Nomor PR	Kode Barang	Nama Barang	Jumlah	Satuan	Tanggal Diminta Datang	Status	Tanggal Diminta Datang Eksternal

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sebelum", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Setelah", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });




            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "",0, 0, 0,"");
            // to allow column name to be generated properly for empty data as template
            else
            {

                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.Date == null ? "-" : item.Date.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy - HH:mm:ss", new CultureInfo("id-ID"));
                    //string pr_date = item.PRDate == null ? "-" : item.PRDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string do_date = item.DODate == null ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                    result.Rows.Add(index, item.StorageCode, item.StorageName, item.ItemCode, item.ItemName, date, 
                        item.Reference, item.Type, item.Before, item.Quantity, item.After, item.Remark);



                }

            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

        #region Inventory Movement By Date

        //public IQueryable<InventoryMovementsReportViewModel> GetMovementByDateQuery(string storageId, string itemCode, DateTime firstDay, DateTime lastDay)
        //{
        //    var Query = (from c in dbContext.InventoryMovements
        //                 where c.IsDeleted == false
        //                 && c.StorageId == Convert.ToInt64((string.IsNullOrWhiteSpace(storageId) ? c.StorageId.ToString() : storageId))
        //                 && c.ItemCode == (string.IsNullOrWhiteSpace(itemCode) ? c.ItemCode : itemCode)
        //                 && c.CreatedUtc >= firstDay
        //                 && c.CreatedUtc <= lastDay

        //                 select new InventoryMovementsReportViewModel
        //                 {
        //                     Date = c.Date,
        //                     ItemCode = c.ItemCode,
        //                     ItemName = c.ItemName,
        //                     ItemArticleRealizationOrder = c.ItemArticleRealizationOrder,
        //                     ItemSize = c.ItemSize,
        //                     ItemUom = c.ItemUom,
        //                     ItemDomesticSale = c.ItemDomesticSale,
        //                     Quantity = c.Type == "OUT" ? -c.Quantity : c.Quantity,
        //                     Before = c.Before,
        //                     After = c.After,
        //                     Type = c.Type,
        //                     Reference = c.Reference,
        //                     Remark = c.Remark,
        //                     StorageId = c.StorageId,
        //                     StorageCode = c.StorageCode,
        //                     StorageName = c.StorageName,
        //                     CreatedUtc = c.CreatedUtc,
        //                 });
        //    return Query;
        //}

        //public Tuple<List<InventoryMovementsReportViewModel>, int> GetMovementsByDate(string storageId, string itemCode, string _month, string _year, string info, string Order, int offset, string username, int page = 1, int size = 25)
        //{
        //    var month = Convert.ToInt32(_month);
        //    var year = Convert.ToInt32(_year);

        //    var firstDay = new DateTime(year, month, 1);
        //    var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        //    var Query = GetMovementByDateQuery(storageId, itemCode, firstDay, lastDay);

        //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
        //    if (OrderDictionary.Count.Equals(0))
        //    {
        //        Query = Query.OrderByDescending(b => b.LastModifiedUtc);
        //    }
        //    else
        //    {
        //        string Key = OrderDictionary.Keys.First();
        //        string OrderType = OrderDictionary[Key];

        //        Query = Query.OrderBy(string.Concat(Key, " ", OrderType));
        //    }

        //    Pageable<InventoryMovementsReportViewModel> pageable = new Pageable<InventoryMovementsReportViewModel>(Query, page - 1, size);
        //    List<InventoryMovementsReportViewModel> Data = pageable.Data.ToList<InventoryMovementsReportViewModel>();
        //    int TotalData = pageable.TotalCount;

        //    return Tuple.Create(Data, TotalData);
        //}

        //public MemoryStream GenerateExcelReportMovementByDate(string storecode, string itemCode, string _month, string _year)
        //{

        //    var month = Convert.ToInt32(_month);
        //    var year = Convert.ToInt32(_year);

        //    var firstDay = new DateTime(year, month, 1);
        //    var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        //    var Query = GetMovementByDateQuery(storecode, itemCode, firstDay, lastDay);
        //    DataTable result = new DataTable();

        //    result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Nama", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Referensi", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Sebelum", DataType = typeof(double) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(double) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Setelah", DataType = typeof(double) });
        //    result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });

        //    if (Query.ToArray().Count() == 0)
        //        result.Rows.Add("", "", "", "", "", "", "", "", 0, 0, 0, "");
        //    // to allow column name to be generated properly for empty data as template
        //    else
        //    {
        //        int index = 0;
        //        foreach (var item in Query)
        //        {
        //            index++;
        //            string date = item.Date == null ? "-" : item.Date.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy - HH:mm:ss", new CultureInfo("id-ID"));
        //            result.Rows.Add(index, item.StorageCode, item.StorageName, item.ItemCode, item.ItemName, date,
        //                item.Reference, item.Type, item.Before, item.Quantity, item.After, item.Remark);
        //        }

        //    }

        //    return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        //}

        //public IQueryable<InventoryMovementsReportViewModel> GetMovementByDateQuery(DateTime firstDay, DateTime lastDay)
        //{
        //    var Query = (from c in dbContext.InventoryMovements
        //                 join d in dbContext.Inventories
        //                 on new { c.ItemCode, c.StorageCode } equals new { d.ItemCode, d.StorageCode }
        //                 where c.IsDeleted == false
        //                 && c.Date.AddHours(7).Date >= firstDay.Date
        //                 && c.Date.AddHours(7).Date <= lastDay.Date
        //                 orderby c.Date, c.StorageCode, c.ItemCode
        //                 select new InventoryMovementsReportViewModel
        //                 {
        //                     Date = c.Date,
        //                     ItemCode = c.ItemCode,
        //                     ItemName = c.ItemName,
        //                     ItemArticleRealizationOrder = d.ItemArticleRealizationOrder,
        //                     ItemSize = c.ItemSize,
        //                     ItemUom = c.ItemUom,
        //                     ItemDomesticSale = c.ItemDomesticSale,
        //                     Quantity = c.Type == "OUT" ? -c.Quantity : c.Quantity,
        //                     Before = c.Before,
        //                     After = c.After,
        //                     Type = c.Type,
        //                     Reference = c.Reference,
        //                     Remark = c.Remark,
        //                     StorageId = c.StorageId,
        //                     StorageCode = c.StorageCode,
        //                     StorageName = c.StorageName,
        //                     CreatedUtc = c.CreatedUtc,
        //                     SourceName = c.Type == "IN" ? dbContext.TransferInDocs.Where(a => a.Code == c.Reference).Select(a => a.SourceName).FirstOrDefault() : dbContext.TransferOutDocs.Where(a => a.Code == c.Reference).Select(a => a.SourceName).FirstOrDefault(),
        //                     DestinationName = c.Type == "IN" ? dbContext.TransferInDocs.Where(a => a.Code == c.Reference).Select(a => a.DestinationName).FirstOrDefault() : dbContext.TransferOutDocs.Where(a => a.Code == c.Reference).Select(a => a.DestinationName).FirstOrDefault()
        //                 }).OrderBy(a => a.Date.Date).ThenBy(a => a.SourceName).ThenBy(a => a.DestinationName).ThenBy(a => a.ItemCode);

        //    return Query;
        //}

        public IQueryable<InventoryMovementsMonthlyReportViewModel> GetMovementByDateQuery(DateTime firstDay, DateTime lastDay)
        {
            SqlConnection conn = new SqlConnection("Server=bru-db-server.database.windows.net,1433;Database=bru-db-warehouse;User=bru;password=Standar123.;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=true");
            conn.Open();
            SqlCommand command = new SqlCommand(
                "SELECT[After], a.CreatedUtc,[Before],  CONVERT(Date, a.Date) Date,[ItemArticleRealizationOrder],[ItemCode],[ItemDomesticSale],[ItemInternationalSale],[ItemName] " +
                ",[ItemSize],[ItemUom],[Quantity], a.[Reference], a.[Remark],[StorageCode],[StorageId],[StorageName],[Type]" +
                ", case when type = 'IN' then(select top 1 SourceName from TransferInDocs where isdeleted = 0 and TransferInDocs.code=a.reference) " +
                "		else (select top 1 SourceName from TransferOutDocs where isdeleted = 0 and TransferOutDocs.code=a.reference) end as SourceName" +
                ", case when type = 'IN' then(select top 1 DestinationName from TransferInDocs where isdeleted = 0 and TransferInDocs.code=a.reference)" +
                "		else (select top 1 DestinationName from TransferOutDocs where isdeleted = 0 and TransferOutDocs.code=a.reference) end as DestinationName" +
                " FROM[dbo].[InventoryMovements] a" +
                " where Month(a.Date) = " + lastDay.Month + " and Year(a.Date)= " + lastDay.Year + " and a.IsDeleted = 0", conn);
            List<InventoryMovementsMonthlyReportViewModel> dataList = new List<InventoryMovementsMonthlyReportViewModel>();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // var date = Convert.ToDateTime(reader["Date"].ToString());
                    InventoryMovementsMonthlyReportViewModel data = new InventoryMovementsMonthlyReportViewModel
                    {
                        Date = reader["Date"].ToString(),
                        ItemCode = reader["ItemCode"].ToString(),
                        ItemName = reader["ItemName"].ToString(),
                        ItemArticleRealizationOrder = reader["ItemArticleRealizationOrder"].ToString(),
                        ItemSize = reader["ItemSize"].ToString(),
                        ItemUom = reader["ItemUom"].ToString(),
                        ItemDomesticSale = Convert.ToDouble(reader["ItemDomesticSale"]),
                        Quantity = reader["Type"].ToString() == "OUT" ? -Convert.ToInt32(reader["Quantity"]) : Convert.ToInt32(reader["Quantity"]),
                        Before = Convert.ToDouble(reader["Before"]),
                        After = Convert.ToDouble(reader["After"]),
                        Type = reader["Type"].ToString(),
                        Reference = reader["Reference"].ToString(),
                        Remark = reader["Remark"].ToString(),
                        StorageId = Convert.ToInt32(reader["StorageId"]),
                        StorageCode = reader["StorageCode"].ToString(),
                        StorageName = reader["StorageName"].ToString(),
                        CreatedUtc = (Convert.ToDateTime(reader["CreatedUtc"])),
                        SourceName = reader["SourceName"].ToString(),
                        DestinationName = reader["DestinationName"].ToString()
                    };
                    dataList.Add(data);
                }
            }
            return dataList.AsQueryable().OrderBy(a => a.Date).ThenBy(a => a.SourceName).ThenBy(a => a.DestinationName).ThenBy(a => a.ItemCode);
        }

        public Tuple<List<InventoryMovementsMonthlyReportViewModel>, int> GetMovementsByDate(string _month, string _year, int page = 1, int size = 25)
        {
            var month = Convert.ToInt32(_month);
            var year = Convert.ToInt32(_year);

            var firstDay = new DateTime(year, month, 1);
            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var Query = GetMovementByDateQuery(firstDay, lastDay);

            Pageable<InventoryMovementsMonthlyReportViewModel> pageable = new Pageable<InventoryMovementsMonthlyReportViewModel>(Query, page - 1, size);
            List<InventoryMovementsMonthlyReportViewModel> Data = pageable.Data.ToList<InventoryMovementsMonthlyReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }

        public MemoryStream GenerateExcelReportMovementByDate(string _month, string _year)
        {

            var month = Convert.ToInt32(_month);
            var year = Convert.ToInt32(_year);

            var firstDay = new DateTime(year, month, 1);
            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var Query = GetMovementByDateQuery(firstDay, lastDay);

            DataTable result = new DataTable();
            var headers = new string[] { "No", "Tanggal", "Asal", "Tujuan", "Barcode", "Nama Barang", "RO", "Harga", "Tipe", "Sebelum", "Kuantitas", "Setelah", "Referensi", "Keterangan" };

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tanggal", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Asal", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tujuan", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Harga", DataType = typeof(int) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tipe", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Sebelum", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Setelah", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Keterangan", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", 0, "", 0, 0, 0, "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {

                var dateSpan = Query.ToArray();
                var q = Query.ToList();
                var index = 0;

                foreach (InventoryMovementsMonthlyReportViewModel temp in q)
                {
                    InventoryMovementsMonthlyReportViewModel dup = Array.Find(dateSpan, o => o.Date.ToString() == temp.Date.ToString());
                    if (dup != null)
                    {
                        if (dup.count == 0)
                        {
                            index++;
                            dup.count = index;
                        }
                    }
                    temp.count = dup.count;
                }

                foreach (var item in q)
                {
                    result.Rows.Add(item.count, item.Date, item.SourceName, item.DestinationName, item.ItemCode, item.ItemName,
                        item.ItemArticleRealizationOrder, item.ItemDomesticSale,
                        item.Type, item.Before, item.Quantity, item.After, item.Reference, item.Remark);
                }

            }

            ExcelPackage package = new ExcelPackage();
            bool styling = true;

            foreach (KeyValuePair<DataTable, String> item in new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") })
            {
                var sheet = package.Workbook.Worksheets.Add(item.Value);

                Dictionary<string, int> Date = new Dictionary<string, int>();
                Dictionary<string, int> DateStorage = new Dictionary<string, int>();
                Dictionary<string, int> DateStorageItem = new Dictionary<string, int>();

                var col = (char)('A' + (result.Columns.Count - 1));
                string tglawal = firstDay.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                string tglakhir = lastDay.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN MUTASI BARANG PER BULAN");
                sheet.Cells[$"A1:{col}1"].Merge = true;
                sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;

                sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
                sheet.Cells[$"A2:{col}2"].Merge = true;
                sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
                sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                sheet.Cells["A7"].LoadFromDataTable(item.Key, false, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);

                sheet.Cells["A5"].Value = headers[0];
                sheet.Cells["B5"].Value = headers[1];
                sheet.Cells["C5"].Value = headers[2];
                sheet.Cells["D5"].Value = headers[3];
                sheet.Cells["E5"].Value = headers[4];
                sheet.Cells["F5"].Value = headers[5];
                sheet.Cells["G5"].Value = headers[6];
                sheet.Cells["H5"].Value = headers[7];
                sheet.Cells["I5"].Value = headers[8];
                sheet.Cells["J5"].Value = headers[9];
                sheet.Cells["K5"].Value = headers[10];
                sheet.Cells["L5"].Value = headers[11];
                sheet.Cells["M5"].Value = headers[12];
                sheet.Cells["N5"].Value = headers[13];
                //sheet.Cells["O5"].Value = headers[14];

                var widths = new int[] { 5, 10, 10, 10, 15, 20, 10, 10, 5, 5, 5, 5, 15, 10 };

                foreach (var i in Enumerable.Range(0, 13))
                {
                    sheet.Column(i + 1).Width = widths[i];
                }

                var data = Query.ToArray();
                int value;

                foreach (var b in Query)
                {
                    if (Date.TryGetValue(b.Date.ToString(), out value))
                    {
                        Date[b.Date.ToString()]++;
                    }
                    else
                    {
                        Date[b.Date.ToString()] = 1;
                    }

                    if (DateStorage.TryGetValue(b.Date.ToString() + b.SourceName + b.DestinationName, out value))
                    {
                        DateStorage[b.Date.ToString() + b.SourceName + b.DestinationName]++;
                    }
                    else
                    {
                        DateStorage[b.Date.ToString() + b.SourceName + b.DestinationName] = 1;
                    }

                    if (DateStorageItem.TryGetValue(b.Date.ToString() + b.SourceName + b.DestinationName + b.ItemCode, out value))
                    {
                        DateStorageItem[b.Date.ToString() + b.SourceName + b.DestinationName + b.ItemCode]++;
                    }
                    else
                    {
                        DateStorageItem[b.Date.ToString() + b.SourceName + b.DestinationName + b.ItemCode] = 1;
                    }
                }

                int index_1 = 7;
                int index_2 = 7;
                int index_3 = 7;

                foreach (KeyValuePair<string, int> b in Date)
                {
                    sheet.Cells["A" + index_1 + ":A" + (index_1 + b.Value - 1)].Merge = true;
                    sheet.Cells["A" + index_1 + ":A" + (index_1 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["B" + index_1 + ":B" + (index_1 + b.Value - 1)].Merge = true;
                    sheet.Cells["B" + index_1 + ":B" + (index_1 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index_1 += b.Value;
                }

                foreach (KeyValuePair<string, int> b in DateStorage)
                {
                    sheet.Cells["C" + index_2 + ":C" + (index_2 + b.Value - 1)].Merge = true;
                    sheet.Cells["C" + index_2 + ":C" + (index_2 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["D" + index_2 + ":D" + (index_2 + b.Value - 1)].Merge = true;
                    sheet.Cells["D" + index_2 + ":D" + (index_2 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index_2 += b.Value;
                }

                foreach (KeyValuePair<string, int> b in DateStorageItem)
                {
                    sheet.Cells["E" + index_3 + ":E" + (index_3 + b.Value - 1)].Merge = true;
                    sheet.Cells["E" + index_3 + ":E" + (index_3 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["F" + index_3 + ":F" + (index_3 + b.Value - 1)].Merge = true;
                    sheet.Cells["F" + index_3 + ":F" + (index_3 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["G" + index_3 + ":G" + (index_3 + b.Value - 1)].Merge = true;
                    sheet.Cells["G" + index_3 + ":G" + (index_3 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    sheet.Cells["H" + index_3 + ":H" + (index_3 + b.Value - 1)].Merge = true;
                    sheet.Cells["H" + index_3 + ":H" + (index_3 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    //sheet.Cells["I" + index_3 + ":I" + (index_3 + b.Value - 1)].Merge = true;
                    //sheet.Cells["I" + index_3 + ":I" + (index_3 + b.Value - 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    index_3 += b.Value;
                }
            }

            MemoryStream stream = new MemoryStream();
            package.SaveAs(stream);
            return stream;
        }

        public Tuple<List<InventoryMovementsMonthlyReportViewModel>, int> GetMovementAll(string storageId, DateTime dateFrom, DateTime dateTo, int page = 1, int size = 25)
        {


            var Query = GetMovementAllQuery(storageId, dateFrom,dateTo);

            Pageable<InventoryMovementsMonthlyReportViewModel> pageable = new Pageable<InventoryMovementsMonthlyReportViewModel>(Query, page - 1, size);
            List<InventoryMovementsMonthlyReportViewModel> Data = pageable.Data.ToList<InventoryMovementsMonthlyReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public IQueryable<InventoryMovementsMonthlyReportViewModel> GetMovementAllQuery(string storageId, DateTime dateFrom, DateTime dateTo)
        {
            DateTime _dateTo = dateTo == new DateTime(0001,1,1) ? DateTime.Now : dateTo;
            DateTime _dateFrom = dateFrom == new DateTime(0001, 1, 1) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : dateTo;
            //var builder = new ConfigurationBuilder()
            //              .SetBasePath(Directory.GetCurrentDirectory())
            //              .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            //IConfiguration _configuration = builder.Build();
            //var myConnectionString1 = _configuration.GetConnectionString("DefaultConnection");
            //SqlConnection conn = new SqlConnection(myConnectionString1);
            SqlConnection conn = new SqlConnection("Server=bru-db-server.database.windows.net,1433;Database=bru-db-warehouse;User=bru;password=Standar123.;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=true");

            conn.Open();
            if (storageId != "0")
            {
                SqlCommand command = new SqlCommand(
               "select CreatedUtc,After,Before,ItemArticleRealizationOrder,ItemCode,ItemDomesticSale,ItemName,Quantity,Reference,StorageCode,StorageName,Type,Remark " +
               "from InventoryMovements where IsDeleted = 0 and StorageId = '" + storageId + "' and (CONVERT(Date, CreatedUtc) between '" + _dateFrom.Date + "' and '" + _dateTo.Date + "'  )", conn);
                List<InventoryMovementsMonthlyReportViewModel> dataList = new List<InventoryMovementsMonthlyReportViewModel>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // var date = Convert.ToDateTime(reader["Date"].ToString());
                        InventoryMovementsMonthlyReportViewModel data = new InventoryMovementsMonthlyReportViewModel
                        {
                            Date = Convert.ToDateTime( reader["CreatedUtc"]).ToString("MM/dd/yyyy hh:mm tt"),
                            ItemCode = reader["ItemCode"].ToString(),
                            ItemName = reader["ItemName"].ToString(),
                            ItemArticleRealizationOrder = reader["ItemArticleRealizationOrder"].ToString(),

                            ItemDomesticSale = Convert.ToDouble(reader["ItemDomesticSale"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Before = Convert.ToDouble(reader["Before"]),
                            After = Convert.ToDouble(reader["After"]),
                            Type = reader["Type"].ToString(),
                            Reference = reader["Reference"].ToString(),
                            Remark = reader["Remark"].ToString(),

                            StorageCode = reader["StorageCode"].ToString(),
                            StorageName = reader["StorageName"].ToString()

                        };
                        dataList.Add(data);
                    }
                }
                return dataList.AsQueryable().OrderBy(a => a.Date).ThenBy(a => a.SourceName).ThenBy(a => a.DestinationName).ThenBy(a => a.ItemCode);

            }
            else
            {
                SqlCommand command = new SqlCommand(
               "select CreatedUtc,After,Before,ItemArticleRealizationOrder,ItemCode,ItemDomesticSale,ItemName,Quantity,Reference,StorageCode,StorageName,Type,Remark " +
               "from InventoryMovements where IsDeleted = 0  and (CONVERT(Date, CreatedUtc) between '" + dateFrom.Date + "' and '" + _dateTo.Date + "'  )", conn);
                List<InventoryMovementsMonthlyReportViewModel> dataList = new List<InventoryMovementsMonthlyReportViewModel>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // var date = Convert.ToDateTime(reader["Date"].ToString());
                        InventoryMovementsMonthlyReportViewModel data = new InventoryMovementsMonthlyReportViewModel
                        {
                            Date = Convert.ToDateTime(reader["CreatedUtc"]).ToString("MM/dd/yyyy hh:mm tt"),
                            ItemCode = reader["ItemCode"].ToString(),
                            ItemName = reader["ItemName"].ToString(),
                            ItemArticleRealizationOrder = reader["ItemArticleRealizationOrder"].ToString(),

                            ItemDomesticSale = Convert.ToDouble(reader["ItemDomesticSale"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Before = Convert.ToDouble(reader["Before"]),
                            After = Convert.ToDouble(reader["After"]),
                            Type = reader["Type"].ToString(),
                            Reference = reader["Reference"].ToString(),
                            Remark = reader["Remark"].ToString(),

                            StorageCode = reader["StorageCode"].ToString(),
                            StorageName = reader["StorageName"].ToString()

                        };
                        dataList.Add(data);
                    }
                }
                return dataList.AsQueryable();

            }

        }
        public MemoryStream GenerateExcelReportMovementAll(string storageId, DateTime dateFrom, DateTime dateTo)
        {
 

            var Query = GetMovementAllQuery(storageId,dateFrom,dateTo);

            DataTable result = new DataTable();
            
            result.Columns.Add(new DataColumn() { ColumnName = "Date", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Before", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "After", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Item Code", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Item Domestic Sale", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Item Name", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Reference", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage Code", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage Name", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Type", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Remark", DataType = typeof(String) }); 

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", 0, 0, "", "", 0, "", 0, "", "", "",  "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                foreach (var item in Query)
                {
                    result.Rows.Add(Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy hh:mm tt"), item.Before , item.After , item.ItemArticleRealizationOrder, item.ItemCode,item.ItemDomesticSale, item.ItemName,
                          item.Quantity, item.Reference,item.StorageCode,item.StorageName,item.Type, item.Remark);
                }

            }
            bool styling = true;
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Sheet 1");

                
                var col = (char)('A' + (result.Columns.Count - 1));
                string tglawal = dateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                string tglakhir = dateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN MUTASI BARANG");
                sheet.Cells[$"A1:{col}1"].Merge = true;
                sheet.Cells[$"A1:{col}1"].Style.Font.Size = 15;
                sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;

                sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
                sheet.Cells[$"A2:{col}2"].Merge = true;
                sheet.Cells[$"A2:{col}2"].Style.Font.Size = 15;
                sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
                sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                sheet.Cells["A5"].LoadFromDataTable(result,true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                sheet.Cells["A5"].Style.Font.Bold = true;
                sheet.Cells["A" + 6 + ":M" + (Query.Count()-1) + ""].AutoFitColumns();
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);
                return stream;
            }

           
        }


        #endregion

        #region Stock Availability

        private List<StoreViewModel> getNearestStore(string code)
        {
            string itemUri = "master/stores/nearest-store";
            string queryUri = "?code=" + code;
            string uri = itemUri + queryUri;

            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{uri}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<StoreViewModel> viewModel = JsonConvert.DeserializeObject<List<StoreViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<InventoryViewModel2>> GetNearestStorageStock(string storageCode, string itemCode)
        {
            var itemList = new List<InventoryViewModel2>();
            var stores = getNearestStore(storageCode);

            if(stores != null)
            {
                foreach (var store in stores)
                {
                    var item = (from a in dbContext.Inventories
                                where a.StorageCode == store.Code
                                && a.ItemCode == itemCode
                                select new InventoryViewModel2
                                {
                                    ItemCode = a.ItemCode,
                                    ItemName = a.ItemName,
                                    StorageCode = a.StorageCode,
                                    StorageName = a.StorageName,
                                    City = store.City,
                                    Quantity = a.Quantity,
                                    LatestDate = new DateTimeOffset()
                                }).FirstOrDefault();
                    if (item != null)
                    {
                        if (item.StorageCode != storageCode && !item.StorageCode.Contains("GDG"))
                        {
                            itemList.Add(item);
                        }
                    }
                }

            }

            return itemList;
        }

        public IQueryable<InventoryViewModel> GetAllStockByStorageQuery(string storageId)
        {
            var Query = (from a in dbContext.Inventories
                         where a.StorageId == Convert.ToInt64(storageId)
                         select new InventoryViewModel
                         {
                             storage = new ViewModels.NewIntegrationViewModel.StorageViewModel
                             {
                                 code = a.StorageCode,
                                 name = a.StorageName
                             },
                             item = new ViewModels.NewIntegrationViewModel.ItemViewModel
                             {
                                 code = a.ItemCode,
                                 name = a.ItemName,
                                 articleRealizationOrder = a.ItemArticleRealizationOrder
                             },
                             quantity = a.Quantity
                         });
            return Query;
        }

        public List<InventoryViewModel> GetAllStockByStorageId (string storageId)
        {
            var Query = GetAllStockByStorageQuery(storageId);

            return Query.ToList();
        }
        public Tuple<List<InventoryMovementsMonthlyReportViewModel>, int> GetStockAll(string storageId, DateTime dateFrom, DateTime dateTo, int page = 1, int size = 25)
        {


            var Query = GetStockAllQuery(storageId, dateFrom, dateTo);

            Pageable<InventoryMovementsMonthlyReportViewModel> pageable = new Pageable<InventoryMovementsMonthlyReportViewModel>(Query, page - 1, size);
            List<InventoryMovementsMonthlyReportViewModel> Data = pageable.Data.ToList<InventoryMovementsMonthlyReportViewModel>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData);
        }
        public IQueryable<InventoryMovementsMonthlyReportViewModel> GetStockAllQuery(string storageId, DateTime dateFrom, DateTime dateTo)
        {
            DateTime _dateTo = dateTo == new DateTime(0001, 1, 1) ? DateTime.Now : dateTo;
            DateTime _dateFrom = dateFrom == new DateTime(0001, 1, 1) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : dateTo;
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var myConnectionString1 = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection conn = new SqlConnection(myConnectionString1);
            //SqlConnection conn = new SqlConnection("Server=bru-db-server.database.windows.net,1433;Database=bru-db-warehouse;User=bru;password=Standar123.;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=true");

            conn.Open();
            if (storageId != "0")
            {
                SqlCommand command = new SqlCommand(
               "select ItemCode,ItemName, ItemDomesticSale,Quantity,CreatedUtc,StorageCode,StorageName " +
               "from Inventories where IsDeleted = 0  and (CONVERT(Date, CreatedUtc) between '" + dateFrom.Date + "' and '" + _dateTo.Date + "'  ) and Storageid= " + storageId, conn);
                List<InventoryMovementsMonthlyReportViewModel> dataList = new List<InventoryMovementsMonthlyReportViewModel>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // var date = Convert.ToDateTime(reader["Date"].ToString());
                        InventoryMovementsMonthlyReportViewModel data = new InventoryMovementsMonthlyReportViewModel
                        {
                            Date = Convert.ToDateTime(reader["CreatedUtc"]).ToString("MM/dd/yyyy hh:mm tt"),
                            ItemCode = reader["ItemCode"].ToString(),
                            ItemName = reader["ItemName"].ToString(),
                            ItemDomesticSale = Convert.ToDouble(reader["ItemDomesticSale"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            StorageCode = reader["StorageCode"].ToString(),
                            StorageName = reader["StorageName"].ToString()

                        };
                        dataList.Add(data);
                    }
                }
                return dataList.AsQueryable().OrderBy(a => a.Date).ThenBy(a => a.ItemCode);

            }
            else
            {
                SqlCommand command = new SqlCommand(
               "Select ItemCode,ItemName, ItemDomesticSale,Quantity,CreatedUtc,StorageCode,StorageName " +
               "from Inventories where IsDeleted = 0  and (CONVERT(Date, CreatedUtc) between '" + dateFrom.Date + "' and '" + _dateTo.Date + "'  )", conn);
                List<InventoryMovementsMonthlyReportViewModel> dataList = new List<InventoryMovementsMonthlyReportViewModel>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // var date = Convert.ToDateTime(reader["Date"].ToString());
                        InventoryMovementsMonthlyReportViewModel data = new InventoryMovementsMonthlyReportViewModel
                        {
                            Date = Convert.ToDateTime(reader["CreatedUtc"]).ToString("MM/dd/yyyy hh:mm tt"),
                            ItemCode = reader["ItemCode"].ToString(),
                            ItemName = reader["ItemName"].ToString(),
                            ItemDomesticSale = Convert.ToDouble(reader["ItemDomesticSale"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            StorageCode = reader["StorageCode"].ToString(),
                            StorageName = reader["StorageName"].ToString()

                        };
                        dataList.Add(data);
                    }
                }
                return dataList.AsQueryable();

            }

        }
        public MemoryStream GenerateExcelReportStockAll(string storageId, DateTime dateFrom, DateTime dateTo)
        {


            var Query = GetStockAllQuery(storageId, dateFrom, dateTo);

            DataTable result = new DataTable();
             
            result.Columns.Add(new DataColumn() { ColumnName = "Item Code", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Item Name", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Item Domestic Sale", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Date", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage Code", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Storage Name", DataType = typeof(String) });
            

            if (Query.ToArray().Count() == 0)
                result.Rows.Add( "", "", 0, 0,  "", "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                foreach (var item in Query)
                {
                    result.Rows.Add( item.ItemCode, item.ItemName, item.ItemDomesticSale,
                          item.Quantity, Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy hh:mm tt"), item.StorageCode, item.StorageName);
                }

            }
            bool styling = true;
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Sheet 1");


                var col = (char)('A' + (result.Columns.Count - 1));
                string tglawal = dateFrom.ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                string tglakhir = dateTo.ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                sheet.Cells[$"A1:{col}1"].Value = string.Format("LAPORAN INVENTORI BARANG");
                sheet.Cells[$"A1:{col}1"].Merge = true;
                sheet.Cells[$"A1:{col}1"].Style.Font.Size = 15;
                sheet.Cells[$"A1:{col}1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A1:{col}1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                sheet.Cells[$"A1:{col}1"].Style.Font.Bold = true;

                sheet.Cells[$"A2:{col}2"].Value = string.Format("Periode {0} - {1}", tglawal, tglakhir);
                sheet.Cells[$"A2:{col}2"].Merge = true;
                sheet.Cells[$"A2:{col}2"].Style.Font.Size = 15;
                sheet.Cells[$"A2:{col}2"].Style.Font.Bold = true;
                sheet.Cells[$"A2:{col}2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[$"A2:{col}2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                sheet.Cells["A5"].LoadFromDataTable(result, true, (styling == true) ? OfficeOpenXml.Table.TableStyles.Light16 : OfficeOpenXml.Table.TableStyles.None);
                sheet.Cells["A5"].Style.Font.Bold = true;
                //sheet.Cells["A" + 6 + ":M" + (Query.Count() - 1) + ""].AutoFitColumns();
                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);
                return stream;
            }


        }
        #endregion

        #region Monthly Stock
        public List<MonthlyStockViewModel> GetOverallMonthlyStock(string _year, string _month)
        {
            var month = Convert.ToInt32(_month);
            var year = Convert.ToInt32(_year);

            var firstDay = new DateTime(year, month, 1);
            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            List<MonthlyStockViewModel> monthlyStock = GetMonthlyStockQuery(firstDay, lastDay).ToList();

            return monthlyStock;
        }

        public IEnumerable<MonthlyStockViewModel> GetMonthlyStockQuery(DateTime firstDay, DateTime lastDay)
        {
            var movementStock = (from a in dbContext.InventoryMovements
                                     // where a.CreatedUtc <= lastDay
                                 where a.CreatedUtc.Date <= lastDay.Date
                                && a.IsDeleted == false
                                 select new
                                 {
                                     ItemCode = a.ItemCode,
                                     ItemName = a.ItemName,
                                     StorageCode = a.StorageCode,
                                     StorageName = a.StorageName,
                                     CreatedUtc = a.CreatedUtc,
                                     After = a.After,
                                     ItemDomesticCOGS = a.ItemDomesticCOGS,
                                     ItemInternationalCOGS = a.ItemInternationalCOGS,
                                     ItemDomesticSale = a.ItemDomesticSale,
                                     ItemInternationalSale = a.ItemInternationalSale

                                 }).ToList();

            var earlyStock = (from a in movementStock
                              orderby a.CreatedUtc descending
                               where a.CreatedUtc.Date < firstDay.Date
                               group a by new { a.ItemCode, a.StorageCode, a.StorageName } into aa

                              select new StockPerItemViewModel
                              {
                                  ItemCode = aa.Key.ItemCode,
                                  ItemName = aa.FirstOrDefault().ItemName,
                                  StorageCode = aa.Key.StorageCode,
                                  StorageName = aa.Key.StorageName,
                                  Quantity = aa.FirstOrDefault().After,
                                  HPP = (aa.FirstOrDefault().ItemDomesticCOGS > 0 ? aa.FirstOrDefault().ItemDomesticCOGS : aa.FirstOrDefault().ItemInternationalCOGS) * aa.FirstOrDefault().After,
                                  Sale = (aa.FirstOrDefault().ItemDomesticSale > 0 ? aa.FirstOrDefault().ItemDomesticSale : aa.FirstOrDefault().ItemInternationalSale) * aa.FirstOrDefault().After

                              });

            var overallEarlyStock = (from b in earlyStock
                                     group b by new { b.StorageCode, b.StorageName } into bb

                                     select new MonthlyStockViewModel
                                     {
                                         StorageCode = bb.Key.StorageCode,
                                         StorageName = bb.Key.StorageName,
                                         EarlyQuantity = bb.Sum(x => x.Quantity),
                                         EarlyHPP = bb.Sum(x => x.HPP),
                                         EarlySale = bb.Sum(x => x.Sale),
                                         LateQuantity = 0,
                                         LateHPP = 0,
                                         LateSale = 0
                                     });

            var lateStock = (from a in movementStock
                             orderby a.CreatedUtc descending
                             //where a.CreatedUtc <= lastDay
                             where a.CreatedUtc.Date <= lastDay.Date
                             group a by new { a.ItemCode, a.StorageCode, a.StorageName } into aa

                             select new StockPerItemViewModel
                             {
                                 ItemCode = aa.Key.ItemCode,
                                 ItemName = aa.FirstOrDefault().ItemName,
                                 StorageCode = aa.Key.StorageCode,
                                 StorageName = aa.Key.StorageName,
                                 Quantity = aa.FirstOrDefault().After,
                                 HPP = (aa.FirstOrDefault().ItemDomesticCOGS > 0 ? aa.FirstOrDefault().ItemDomesticCOGS : aa.FirstOrDefault().ItemInternationalCOGS) * aa.FirstOrDefault().After,
                                 Sale = (aa.FirstOrDefault().ItemDomesticSale > 0 ? aa.FirstOrDefault().ItemDomesticSale : aa.FirstOrDefault().ItemInternationalSale) * aa.FirstOrDefault().After
                             });

            var overallLateStock = (from b in lateStock
                                    group b by new { b.StorageCode, b.StorageName } into bb

                                    select new MonthlyStockViewModel
                                    {
                                        StorageCode = bb.Key.StorageCode,
                                        StorageName = bb.Key.StorageName,
                                        EarlyQuantity = 0,
                                        EarlyHPP = 0,
                                        EarlySale = 0,
                                        LateQuantity = bb.Sum(x => x.Quantity),
                                        LateHPP = bb.Sum(x => x.HPP),
                                        LateSale = bb.Sum(x => x.Sale)
                                    });

            var overallMonthlyStock = overallEarlyStock.Union(overallLateStock).ToList();

            var data = (from query in overallMonthlyStock
                        group query by new { query.StorageCode, query.StorageName } into groupdata

                        select new MonthlyStockViewModel
                        {
                            StorageCode = groupdata.Key.StorageCode,
                            StorageName = groupdata.Key.StorageName,
                            EarlyQuantity = groupdata.Sum(x => x.EarlyQuantity),
                            EarlyHPP = groupdata.Sum(x => x.EarlyHPP),
                            EarlySale = groupdata.Sum(x => x.EarlySale),
                            LateQuantity = groupdata.Sum(x => x.LateQuantity),
                            LateHPP = groupdata.Sum(x => x.LateHPP),
                            LateSale = groupdata.Sum(x => x.LateSale)
                        });

            return data.AsQueryable();
        }

        public List<StockPerItemViewModel> GetOverallStorageStock(string code, string _year, string _month)
        {
            var month = Convert.ToInt32(_month);
            var year = Convert.ToInt32(_year);

            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            List<StockPerItemViewModel> latestStockByStorage = GetLatestStockByStorageQuery(code, lastDay).ToList();

            return latestStockByStorage;
        }

        public IEnumerable<StockPerItemViewModel> GetLatestStockByStorageQuery(string code, DateTime date)
        {
            var LatestStock = (from a in dbContext.InventoryMovements
                               orderby a.CreatedUtc descending
                               //where a.CreatedUtc <= date
                               where a.CreatedUtc.Date <= date.Date 
                               && a.StorageCode == code
                               group a by new { a.ItemCode } into aa

                               select new StockPerItemViewModel
                               {
                                   ItemCode = aa.FirstOrDefault().ItemCode,
                                   ItemName = aa.FirstOrDefault().ItemName,
                                   StorageCode = aa.FirstOrDefault().StorageCode,
                                   StorageName = aa.FirstOrDefault().StorageName,
                                   Quantity = aa.FirstOrDefault().After,
                                   HPP = (aa.FirstOrDefault().ItemDomesticCOGS > 0 ? aa.FirstOrDefault().ItemDomesticCOGS : aa.FirstOrDefault().ItemInternationalCOGS) * aa.FirstOrDefault().After,
                                   Sale = (aa.FirstOrDefault().ItemDomesticSale > 0 ? aa.FirstOrDefault().ItemDomesticSale : aa.FirstOrDefault().ItemInternationalSale) * aa.FirstOrDefault().After
                               });

            //var _LatestStock = (from b in LatestStock
            //                    where b.Quantity > 0
            //                    select b);

            return LatestStock.AsQueryable();
        }

        public MemoryStream GenerateExcelForLatestStockByStorage(string code, string _month, string _year)
        {
            var month = Convert.ToInt32(_month);
            var year = Convert.ToInt32(_year);

            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var Query = GetLatestStockByStorageQuery(code, lastDay);

            DataTable result = new DataTable();

            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Bulan", DataType = typeof(Int32) });
            result.Columns.Add(new DataColumn() { ColumnName = "Tahun", DataType = typeof(Int32) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total HPP", DataType = typeof(double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Total Sale", DataType = typeof(double) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", 0, 0, "", "", 0, 0, 0);

            else
            {
                int index = 0;

                foreach (var item in Query)
                {
                    index++;

                    result.Rows.Add(index, item.StorageCode, month, year, item.ItemCode, item.ItemName, item.Quantity, item.HPP, item.Sale);

                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

        #region By RO

        public List<InventoryByRoReportViewModel> GetInventoryReportByRo(string articleRealizationOrder)
        {

            var Query = GetInventoryByRoQuery(articleRealizationOrder);
            return Query.ToList();
        }

        public IEnumerable<InventoryByRoReportViewModel> GetInventoryByRoQuery(string articleRealizationOrder)
        {
            var Query = (from a in dbContext.Inventories
                         join b in dbContext.ExpeditionDetails on a.ItemArticleRealizationOrder equals b.ArticleRealizationOrder
                         join bb in dbContext.ExpeditionItems on b.ExpeditionItemId equals bb.Id
                         join bbb in dbContext.Expeditions on bb.ExpeditionId equals bbb.Id

                         where a.ItemArticleRealizationOrder == articleRealizationOrder
                         && a.StorageCode == bb.DestinationCode
                         && a.IsDeleted == false
                         && b.IsDeleted == false
                         && bb.IsDeleted == false
                         && bbb.IsDeleted == false

                         orderby bbb.CreatedUtc descending

                         group new { a, b, bb, bbb } by new { a.StorageCode, a.StorageName, a.ItemCode, a.ItemArticleRealizationOrder, a.ItemSize, a.Quantity, bb.DestinationCode, bbb.CreatedUtc } into data

                         select new InventoryByRoReportViewModel {
                             //StorageId = data.FirstOrDefault().a.StorageId,
                             code = data.Key.StorageCode,
                             storageName = data.Key.StorageName,
                             itemCode = data.Key.ItemCode,
                             //age = (data.FirstOrDefault().bbb.CreatedUtc).ToString(),
                             age = Math.Truncate(Convert.ToDecimal(DateTime.Now.Subtract(data.Key.CreatedUtc).TotalDays)).ToString(),

                             ro = data.Key.ItemArticleRealizationOrder,
                             destinationCode = data.Key.DestinationCode,
                             size = data.Key.ItemSize,
                             quantityOnInventory = data.Key.Quantity,
                         });

            var Query2 = GetSalesPerRo(articleRealizationOrder);

            var Total = (from a in Query
                         join b in Query2 on a.ro equals b.ItemArticleRealizationOrder

                         where b.ItemArticleRealizationOrder == articleRealizationOrder
                         && b.StoreCode == a.destinationCode
                         && b.size == a.size
                         && b.ItemCode == a.itemCode
                         && a.IsDeleted == false
                         && b.IsDeleted == false
                         
                         orderby a.age descending
                         
                         //group new { a, b } by new { a.code, a.storageName, a.destinationCode, a.ro, a.size, b.ItemCode} into data

                         select new InventoryByRoReportViewModel
                         {
                             code = a.code,
                             storageName = a.storageName,
                             itemCode = a.itemCode,
                             //age = (data.FirstOrDefault().bbb.CreatedUtc).ToString(),
                             //age = Math.Truncate(Convert.ToDecimal(DateTime.Now.Subtract(data.Key.CreatedUtc).TotalDays)).ToString(),
                             age = a.age,
                             ro = a.ro,
                             destinationCode = a.destinationCode,
                             size = a.size,
                             quantityOnInventory = a.quantityOnInventory,
                             quantityOnSales = b.quantityOnSales,
                         });

            //var Query2 = getSalesPerRo(articleRealizationOrder).AsQueryable();

            return Total;
            
        }

        private List<SalesDocByRoViewModel> GetSalesPerRo(string articleRealizationOrder)
        {
            string itemUri = "sales-docs/readbyro";
            //string queryUri = "?ro=" + articleRealizationOrder;
            //string posUri = itemUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.POS}{itemUri}/{articleRealizationOrder}").Result;
            
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<SalesDocByRoViewModel> viewModel = JsonConvert.DeserializeObject<List<SalesDocByRoViewModel>>(result.GetValueOrDefault("data").ToString());
                return viewModel;
            }
            else
            {
                List<SalesDocByRoViewModel> viewModel = null;
                return viewModel;
            }
            
        }

        public MemoryStream GenerateExcelStokByRO (string ro)
        {
            var Query = GetInventoryByRoQuery(ro);
            var data = Query.ToList();

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Size", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stok RO", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Stok RO Terjual", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Size", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Umur", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Total Stok", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Total Stok Terjual", DataType = typeof(double) });

            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                //int index = 0;

                Dictionary<string, List<InventoryByRoReportViewModel>> dataByRo = new Dictionary<string, List<InventoryByRoReportViewModel>>();
                Dictionary<string, double> totalStok = new Dictionary<string, double>();
                Dictionary<string, double> totalTerjual = new Dictionary<string, double>();
                foreach (InventoryByRoReportViewModel item in Query.ToList())
                {
                    //index++;
                    string StorageName = item.storageName;

                    if (!dataByRo.ContainsKey(StorageName)) dataByRo.Add(StorageName, new List<InventoryByRoReportViewModel> { });
                    dataByRo[StorageName].Add(new InventoryByRoReportViewModel
                    {
                        storageName = item.storageName,
                        destinationCode = item.destinationCode,
                        itemCode = item.itemCode,
                        ro = item.ro,
                        age = item.age,
                        size = item.size,
                        quantityOnInventory = item.quantityOnInventory,
                        quantityOnSales = item.quantityOnSales,
                    });
                    
                    if (!totalStok.ContainsKey(StorageName))
                    {
                        totalStok.Add(StorageName, 0);
                    };

                    if (!totalTerjual.ContainsKey(StorageName))
                    {
                        totalTerjual.Add(StorageName, 0);
                    };
                    
                    totalStok[StorageName] += item.quantityOnInventory;
                    totalTerjual[StorageName] += item.quantityOnSales;
                }

                int rowPosition = 1;

                foreach (KeyValuePair<string, List<InventoryByRoReportViewModel>> RoSize in dataByRo)
                {
                    int index = 0;
                    foreach (InventoryByRoReportViewModel item in RoSize.Value)
                    {
                        index++;
                        
                        string sizeStock = string.Format("{0}", item.quantityOnInventory);
                        string sizeTerjual = string.Format("{0}", item.quantityOnSales);

                        result.Rows.Add(index, item.storageName, item.ro, item.size, sizeStock, sizeTerjual, item.age + " hari ");
                        
                    }

                    result.Rows.Add("", "", "",  "", "Total Stok : " + Math.Round(totalStok[RoSize.Key], 2), "Total Stok Terjual : " + Math.Round(totalTerjual[RoSize.Key], 2));

                    rowPosition += 1;
                }
            }
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }

        #endregion

        #region Age Inventory

        public IEnumerable<InventoryAgeReportViewModel> GetInventoryAgeQuery(int storageId, string keyword)
        {
            var Date = DateTime.Now;

            var QueryOverallStocks = (from a in dbContext.Inventories
                                      where a.IsDeleted == false
                                      && a.StorageId == (storageId == 0 ? a.StorageId : storageId)

                                      select new
                                      {
                                          ItemCode = a.ItemCode,
                                          ItemName = a.ItemName,
                                          ItemArticleRealizationOrder = a.ItemArticleRealizationOrder,
                                          Quantity = a.Quantity,
                                          StorageCode = a.StorageCode,
                                          StorageName = a.StorageName,
                                      }).ToList();

            var QueryOverallStocks1 = (from b in QueryOverallStocks
                                       where b.ItemName.Contains(string.IsNullOrWhiteSpace(keyword) ? b.ItemName : keyword)
                                       || b.ItemArticleRealizationOrder.Contains(string.IsNullOrWhiteSpace(keyword) ? b.ItemArticleRealizationOrder : keyword)
                                       || b.ItemCode.Contains(string.IsNullOrWhiteSpace(keyword) ? b.ItemCode : keyword)

                                       select new InventoryAgeReportViewModel
                                       {
                                           ItemCode = b.ItemCode,
                                           ItemName = b.ItemName,
                                           Quantity = b.Quantity,
                                           StorageCode = b.StorageCode,
                                           StorageName = b.StorageName,
                                           DateDiff = 0
                                       }).AsEnumerable();

            //var QueryAgeCurrentItems = (from c in dbContext.InventoryMovements
            //                            where c.IsDeleted == false
            //                            && c.StorageId == (storageId == 0 ? c.StorageId : storageId)
            //                            && c.Type == "IN"

            //                            orderby c.CreatedUtc descending

            //                            select new
            //                            {
            //                                ItemCode = c.ItemCode,
            //                                ItemName = c.ItemName,
            //                                ItemArticleRealizationOrder = c.ItemArticleRealizationOrder,
            //                                StorageCode = c.StorageCode,
            //                                StorageName = c.StorageName,
            //                                //CreatedUtc = c.CreatedUtc,
            //                                //DateDiff = Math.Truncate(Convert.ToDecimal(Date.Subtract(c.CreatedUtc).TotalDays))
            //                            }).ToList();

            //var QueryAgeCurrentItems1 = (from d in QueryAgeCurrentItems
            //                             where d.ItemName.Contains(string.IsNullOrWhiteSpace(keyword) ? d.ItemName : keyword)
            //                             || d.ItemArticleRealizationOrder.Contains(string.IsNullOrWhiteSpace(keyword) ? d.ItemArticleRealizationOrder : keyword)
            //                             || d.ItemCode.Contains(string.IsNullOrWhiteSpace(keyword) ? d.ItemCode : keyword)
            //                             group d by new {d.ItemCode, d.ItemName, d.StorageCode, d.StorageName } into dd
            //                             select new InventoryAgeReportViewModel
            //                             {
            //                                 ItemCode = dd.Key.ItemCode,
            //                                 ItemName = dd.Key.ItemName,
            //                                 Quantity = 0,
            //                                 StorageCode = dd.Key.StorageCode,
            //                                 StorageName = dd.Key.StorageName,
            //                                 //DateDiff = (decimal)d.DateDiff
            //                                 DateDiff = 0
            //                             }).AsEnumerable();
            ////.Select(g => g.OrderByDescending(c => c.CreatedUtc).FirstOrDefault()).ToList();

            //foreach (var i in QueryAgeCurrentItems1)

            var itemCodeList = QueryOverallStocks1.Select(x => x.ItemCode).Distinct().ToList();
            var storageCodeList = QueryOverallStocks1.Select(x => x.StorageCode).Distinct().ToList();

            var QueryAgeInventory = from aa in (from a in dbContext.InventoryMovements
                                                where a.IsDeleted == false
                                                && a.Type == "IN"
                                                && itemCodeList.Contains(a.ItemCode)
                                                && storageCodeList.Contains(a.StorageCode)
                                                orderby a.CreatedUtc descending
                                                select new
                                                {
                                                    ItemCode = a.ItemCode,
                                                    StorageCode = a.StorageCode,
                                                    CreatedUtc = a.CreatedUtc
                                                })

                                     group aa by new { aa.ItemCode, aa.StorageCode } into groupdata
                                     select new
                                     {
                                         ItemCode = groupdata.Key.ItemCode,
                                         StorageCode = groupdata.Key.StorageCode,
                                         CreatedUtc = groupdata.FirstOrDefault().CreatedUtc
                                     };

            //foreach (var i in QueryOverallStocks1)
            //{
            //    var CreatedDate = (from e in dbContext.InventoryMovements
            //                       where e.IsDeleted == false
            //                       && e.Type == "IN"
            //                       && e.ItemCode == i.ItemCode
            //                       && e.StorageCode == i.StorageCode
            //                       orderby e.CreatedUtc descending
            //                       select e.CreatedUtc).FirstOrDefault();

            //    i.DateDiff = Math.Truncate(Convert.ToDecimal(Date.Subtract(CreatedDate).TotalDays));
            //}

            var Query = from a in QueryOverallStocks1
                        join b in QueryAgeInventory on new { a.ItemCode, a.StorageCode } equals new { b.ItemCode, b.StorageCode }
                        select new InventoryAgeReportViewModel
                        {
                            ItemCode = a.ItemCode,
                            ItemName = a.ItemName,
                            Quantity = a.Quantity,
                            StorageCode = a.StorageCode,
                            StorageName = a.StorageName,
                            DateDiff = Math.Truncate(Convert.ToDecimal(Date.Subtract(b.CreatedUtc).TotalDays))
                        };


            //List<InventoryAgeReportViewModel> dataAll = QueryOverallStocks1.Union(QueryAgeCurrentItems1).ToList();
            //List < InventoryAgeReportViewModel > dataAll = QueryOverallStocks1.ToList();
            List<InventoryAgeReportViewModel> dataAll = Query.ToList();

            var data = (from query in dataAll
                        group query by new { query.StorageCode, query.StorageName, query.ItemCode, query.ItemName } into groupdata

                        select new InventoryAgeReportViewModel
                        {
                            ItemCode = groupdata.Key.ItemCode,
                            ItemName = groupdata.Key.ItemName,
                            StorageCode = groupdata.Key.StorageCode,
                            StorageName = groupdata.Key.StorageName,
                            Quantity = groupdata.Sum(x => x.Quantity),
                            DateDiff = groupdata.Sum(x => x.DateDiff)
                        });

            var data1 = (from aa in data
                         where aa.Quantity > 0

                         select aa);

            return data1.AsQueryable();
        }

        public List<InventoryAgeReportViewModel> GetInventoryAge(int storageId, string keyword)
        {
            var Query = GetInventoryAgeQuery(storageId, keyword);
            return Query.ToList();
        }

        public MemoryStream GenerateExcelInventoryAge(int storageId, string keyword)
        {
            var Query = GetInventoryAgeQuery(storageId, keyword);

            DataTable result = new DataTable();
            result.Columns.Add(new DataColumn() { ColumnName = "No", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kode Toko", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Umur Barang (Hari)", DataType = typeof(String) });

            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "");
            // to allow column name to be generated properly for empty data as template
            else
            {
                Dictionary<string, List<InventoryAgeReportViewModel>> dataByToko = new Dictionary<string, List<InventoryAgeReportViewModel>>();
                Dictionary<string, double> subTotalQty = new Dictionary<string, double>();

                foreach (InventoryAgeReportViewModel item in Query.ToList())
                {
                    string StoreName = item.StorageName;

                    if (!dataByToko.ContainsKey(StoreName)) dataByToko.Add(StoreName, new List<InventoryAgeReportViewModel> { });
                    dataByToko[StoreName].Add(new InventoryAgeReportViewModel
                    {
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        StorageCode = item.StorageCode,
                        StorageName = item.StorageName,
                        Quantity = item.Quantity,
                        DateDiff = item.DateDiff,
                    });

                    if (!subTotalQty.ContainsKey(StoreName))
                    {
                        subTotalQty.Add(StoreName, 0);
                    };
                    
                    double Quantity = item.Quantity;

                    subTotalQty[StoreName] += Quantity;
                }

                foreach (KeyValuePair<string, List<InventoryAgeReportViewModel>> StoreName in dataByToko)
                {
                    int index = 0;
                    foreach (InventoryAgeReportViewModel item in StoreName.Value)
                    {
                        index++;
                        // string date = item.Date == null ? "-" : item.Date.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        //string pr_date = item.PRDate == null ? "-" : item.PRDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                        //string do_date = item.DODate == null ? "-" : item.ReceiptDate.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));

                        result.Rows.Add(index, item.StorageCode, item.ItemCode, item.ItemName, item.Quantity, item.DateDiff);

                    }

                    result.Rows.Add("", "", "", "", "Total Quantity : " + Math.Round(subTotalQty[StoreName.Key], 2), "");

                }
            }
            
            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Age") }, true);
        }

        #endregion
    }
}
