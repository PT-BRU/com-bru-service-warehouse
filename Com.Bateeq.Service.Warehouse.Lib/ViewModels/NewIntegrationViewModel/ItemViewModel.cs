using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel
{
    public class ItemViewModel
    {
        public long _id { get; set; }
        public string articleRealizationOrder { get; set; }
        public string code { get; set; }
        public double domesticCOGS { get; set; }
        public double domesticRetail { get; set; }
        public double domesticSale { get; set; }
        public double domesticWholesale { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string uom { get; set; }
    }
}
