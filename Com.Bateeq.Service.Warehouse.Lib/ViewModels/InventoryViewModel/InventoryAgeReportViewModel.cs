using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryAgeReportViewModel : BaseViewModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public decimal DateDiff { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
    }
}
