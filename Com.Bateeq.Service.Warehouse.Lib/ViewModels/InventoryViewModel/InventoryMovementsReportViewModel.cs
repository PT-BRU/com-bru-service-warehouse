using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryMovementsReportViewModel : BaseViewModel
    {
        //public string ItemId { get; set; }
        public string ItemArticleRealizationOrder { get; set; }
        public string ItemCode { get; set; }
        //public double ItemDomesticCOGS { get; set; }
        //public double ItemDomesticRetail { get; set; }
        public double ItemDomesticSale { get; set; }
        public DateTimeOffset Date { get; set; }
        //public double ItemDomesticWholeSale { get; set; }
        //public double ItemInternationalCOGS { get; set; }
        //public double ItemInternationalRetail { get; set; }
        //public double ItemInternationalSale { get; set; }
        //public double ItemInternationalWholeSale { get; set; }
        public string ItemName { get; set; }
        public string ItemSize { get; set; }
        public string ItemUom { get; set; }
        public double Quantity { get; set; }
        public double After { get; set; }
        public double Before { get; set; }
        public string Reference { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }

        public long StorageId { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        //public bool StorageisCentral { get; set; }
    }
}