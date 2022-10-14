using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryByRoReportViewModel : BaseViewModel
    {
        public string code { get; set; }
        public string storageName { get; set; }
        public string destinationCode { get; set; }
        public string ro { get; set; }
        public string size { get; set; }
        public string age { get; set; }
        public string itemCode { get; set; }
        //public decimal DateDiff { get; set; }
        public double quantityOnInventory { get; set; }
        public double quantityOnSales { get; set; }

        public SalesDocByRoViewModel sales { get; set; }

    }
}
