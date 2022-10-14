using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel
{
    public class SODocsViewModel : BaseViewModel
    {
        public string UId { get; set; }
        public string code { get; set; }
       
        public StorageViewModel storage { get; set; }

        public bool isProcessed { get; set; }

        public List<SODocsItemViewModel> items { get; set; }
    }
}
