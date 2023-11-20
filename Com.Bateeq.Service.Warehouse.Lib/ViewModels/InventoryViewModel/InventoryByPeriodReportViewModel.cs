using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryByPeriodReportViewModel
    {
        public string Brand { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public string Collection { get; set; }
        public string SeasonCode { get; set; }
        public string SeasonYear { get; set; }
        public string ItemArticleRealizationOrder { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public double Quantity { get; set; }
        public string ReceivedDate { get; set; }
        public string Date { get; set; }
        public string ItemName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double OriginalCost { get; set; }
        public double Gross { get; set; }
        public double TotalOriCost { get; set; }
        public double TotalGross { get; set; }
    }
}
