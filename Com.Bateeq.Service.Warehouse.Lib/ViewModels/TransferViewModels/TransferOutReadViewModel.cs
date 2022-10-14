using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels
{
    public class TransferOutReadViewModel
    {
        public int _id { get; set; }

        public string code { get; set; }

        public DateTimeOffset date { get; set; }

        public DestinationViewModel destination { get; set; }

        public string packingList { get; set; }

        public string password { get; set; }

        public bool isReceived { get; set; }

        public string reference { get; set; }

        public ExpeditionServiceViewModel expeditionService { get; set; }

        public string createdby { get; set; }

        public SourceViewModel source { get; set; }

        
    }
}
