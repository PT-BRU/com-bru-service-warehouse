using Com.DanLiris.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class SPKDocsFacade
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        private readonly DbSet<Inventory> dbSet;
        private readonly IServiceProvider serviceProvider;

        public SPKDocsFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<Inventory>();
        }

        #region Monitoring By User
        public IQueryable<SPKDocsReportViewModel> GetReportQuery(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int offset, string username)
        {
            DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : (DateTime)dateFrom;
            DateTime DateTo = dateTo == null ? DateTime.Now : (DateTime)dateTo;

            var Query = (from a in dbContext.SPKDocs
                         join b in dbContext.SPKDocsItems on a.Id equals b.SPKDocsId
                         where a.IsDeleted == false
                             && b.IsDeleted == false
                             && a.DestinationCode == (string.IsNullOrWhiteSpace(destinationCode) ? a.DestinationCode : destinationCode)
                             && a.CreatedBy == (string.IsNullOrWhiteSpace(username) ? a.CreatedBy : username)
                             && !a.Reference.Contains("RTT")
                             // && a.Code == (string.IsNullOrWhiteSpace(code) ? a.Code : code)
                             && a.Date.AddHours(offset).Date >= DateFrom.Date
                             && a.Date.AddHours(offset).Date <= DateTo.Date
                             && a.IsDistributed == status
                             && (transaction == 0 ? a.SourceCode == "GDG.01" : a.SourceCode != "GDG.01" && !a.Reference.Contains("RTP"))
                             && a.PackingList.Contains(string.IsNullOrWhiteSpace(packingList) ? a.PackingList : packingList)

                         select new SPKDocsReportViewModel
                         {
                             no = a.Code,
                             date = a.Date,
                             sourceCode = a.SourceCode,
                             sourceName = a.SourceName,
                             destinationCode = a.DestinationCode,
                             destinationName = a.DestinationName,
                             isReceived = a.IsReceived,
                             isDistributed = a.IsDistributed,
                             packingList = a.PackingList,
                             password = a.Password,
                             itemCode = b.ItemCode,
                             itemName = b.ItemName,
                             itemSize = b.ItemSize,
                             itemUom = b.ItemUom,
                             itemArticleRealizationOrder = b.ItemArticleRealizationOrder,
                             Quantity = b.Quantity,
                             itemDomesticSale = b.ItemDomesticSale,
                             LastModifiedUtc = b.LastModifiedUtc
                         });
            //return Query;
            return Query.AsQueryable();
        }

        public Tuple<List<SPKDocsReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int page, int size, string Order, int offset, string username)
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

            Pageable<SPKDocsReportViewModel> pageable = new Pageable<SPKDocsReportViewModel>(Query, page - 1, size);
            List<SPKDocsReportViewModel> Data = pageable.Data.ToList<SPKDocsReportViewModel>();
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
            result.Columns.Add(new DataColumn() { ColumnName = "Ekspedisi", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Diminta", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Satuan Diminta", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Tanggal diminta datang PO Eksternal", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Jumlah Deal PO Eksternal", DataType = typeof(double) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Satuan Deal PO Eksternal", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Status PR", DataType = typeof(String) });
            //result.Columns.Add(new DataColumn() { ColumnName = "Status Barang", DataType = typeof(String) });
            if (Query.ToArray().Count() == 0)
                result.Rows.Add("", "", "", "", "", "", "", "", "", 0, "", ""); // to allow column name to be generated properly for empty data as template
            else
            {
                int index = 0;
                foreach (var item in Query)
                {
                    index++;
                    string date = item.date == null ? "-" : item.date.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string prDate = item.expectedDeliveryDatePR == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePR.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    //string epoDate = item.expectedDeliveryDatePO == new DateTime(1970, 1, 1) ? "-" : item.expectedDeliveryDatePO.ToOffset(new TimeSpan(offset, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    result.Rows.Add(item.date, item.sourceName, item.destinationName, item.packingList, item.itemCode, item.itemName, item.itemSize, item.itemArticleRealizationOrder, item.itemUom, item.Quantity, item.isReceived, item.isDistributed);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>>() { new KeyValuePair<DataTable, string>(result, "Territory") }, true);
        }
        #endregion

    }
}