using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel
{
    public class PkbjByUserItemViewModel : BaseViewModel
    {
        public ItemViewModel item { get; set; }
        public double itemInternationalCOGS { get; set; }
        public double itemInternationalRetail { get; set; }
        public double itemInternationalSale { get; set; }
        public double itemInternationalWholeSale { get; set; }
        public double quantity { get; set; }
        public double sendquantity { get; set; }
        public string remark { get; set; }
        //public StorageViewModel storage { get; set; }

    }

    //public class inventoryviewmodel : BasicViewModel
    //{
        

    //}
}
