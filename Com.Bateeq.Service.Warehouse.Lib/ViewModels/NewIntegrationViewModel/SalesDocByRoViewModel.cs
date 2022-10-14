using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel
{
    public class SalesDocByRoViewModel : BaseViewModel
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreStorageCode { get; set; }
        public string StoreStorageName { get; set; }
        public string ItemCode { get; set; }
        public string ItemArticleRealizationOrder { get; set; }
        public string size { get; set; }
        public double quantityOnSales { get; set; }
    }
}
