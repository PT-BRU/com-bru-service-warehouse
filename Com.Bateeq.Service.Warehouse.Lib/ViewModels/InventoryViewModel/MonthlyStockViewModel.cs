using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class MonthlyStockViewModel : BaseViewModel
    {
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public double EarlyQuantity { get; set; }
        public double EarlyHPP { get; set; }
        public double EarlySale { get; set; }
        public double LateQuantity { get; set; }
        public double LateHPP { get; set; }
        public double LateSale { get; set; }
    }

    public class StockPerItemViewModel : BaseViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public double Quantity { get; set; }
        public double HPP { get; set; }
        public double Sale { get; set; }
    }

    public class StockPerStorageViewModel : BaseViewModel
    {
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public double Quantity { get; set; }
        public double HPP { get; set; }
        public double Sale { get; set; }
    }
}
