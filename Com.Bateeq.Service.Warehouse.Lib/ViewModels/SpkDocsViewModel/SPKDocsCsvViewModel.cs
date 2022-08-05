using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel
{
    public class SPKDocsCsvViewModel : BaseViewModel
    {
        public string PackingList { get; set; }
        public string Password { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public dynamic domesticSale { get; set; }
        public string uom { get; set; }
        public dynamic quantity { get; set; }
        public string articleRealizationOrder { get; set; }
        public dynamic domesticCOGS { get; set; }
    }
}