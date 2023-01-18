using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel
{
    public class ExpeditionItemViewModel : BaseViewModel
    {
        public SPKDocsViewModel spkDocsViewModel { get; set; }
        //public string code { get; set; }
        //public DateTimeOffset date { get; set; }
        //public DestinationViewModel destination { get; set; }
        //public bool isDistributed { get; set; }
        //public bool isDraft { get; set; }
        //public bool isReceived { get; set; }

        //public string packingList { get; set; }
        public string remark { get; set; }
        //public string password { get; set; }

        //public string reference { get; set; }
        //public SourceViewModel source { get; set; }
        //public int sPKDocsId { get; set; }
        public double weight { get; set; }

        public List<ExpeditionDetailViewModel> details { get; set; }
    }
}
