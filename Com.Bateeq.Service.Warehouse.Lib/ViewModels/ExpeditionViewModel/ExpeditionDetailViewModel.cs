using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel
{
    public class ExpeditionDetailViewModel : BaseViewModel
    {
        public ItemViewModel item { get; set; }
        public double quantity { get; set; }
        public double sendQuantity { get; set; }
        public string remark { get; set; }
        public int sPKDocsId { get; set; }
    }
}
