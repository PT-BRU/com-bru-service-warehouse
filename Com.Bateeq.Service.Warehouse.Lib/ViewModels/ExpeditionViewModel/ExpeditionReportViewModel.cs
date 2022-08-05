using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.DanLiris.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel
{
    public class ExpeditionReportViewModel : BaseViewModel
    {
        public string code { get; set; }
        public DateTimeOffset date { get; set; }
        public string expeditionServiceName { get; set; }
        public long sourceId { get; set; }
        public string sourceCode { get; set; }
        public string sourceName { get; set; }
        public long destinationId { get; set; }
        public string destinationCode { get; set; }
        public string destinationName { get; set; }
        public bool isDistributed { get; set; }
        public bool isReceived { get; set; }
        public string packingList { get; set; }
        public string password { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemSize { get; set; }
        public string itemUom { get; set; }
        public string itemArticleRealizationOrder { get; set; }
        public double Quantity { get; set; }
        public double itemDomesticSale { get; set; }
    }
}