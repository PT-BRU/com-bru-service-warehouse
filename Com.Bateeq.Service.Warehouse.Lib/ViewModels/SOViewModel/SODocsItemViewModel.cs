using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System.ComponentModel.DataAnnotations;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel
{
    public class SODocsItemViewModel : BaseViewModel
    {
        public ItemViewModel item { get; set; }
        public double qtyBeforeSO { get; set; }
        public double qtySO { get; set; }
        public string remark { get; set; }
        public bool isAdjusted { get; set; }

        [MaxLength(255)]
        public string UId { get; set; }
    }

}
