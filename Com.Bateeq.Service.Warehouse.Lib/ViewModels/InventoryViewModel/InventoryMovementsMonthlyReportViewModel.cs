using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryMovementsMonthlyReportViewModel : BaseViewModel
    {
        public double count { get; set; }
        public string ItemArticleRealizationOrder { get; set; }
        public string ItemCode { get; set; }
        public double ItemDomesticSale { get; set; }
        public string Date { get; set; }
        public string ItemName { get; set; }
        public string ItemSize { get; set; }
        public string ItemUom { get; set; }
        public double Quantity { get; set; }
        public double After { get; set; }
        public double Before { get; set; }
        public string Reference { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }

        public long StorageId { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string DestinationName { get; set; }
        public string SourceName { get; set; }
    }
}
