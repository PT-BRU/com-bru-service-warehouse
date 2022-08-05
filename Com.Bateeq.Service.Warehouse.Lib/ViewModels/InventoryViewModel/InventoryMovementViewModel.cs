using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel
{
    public class InventoryMovementViewModel : BaseViewModel
    {
        public double after { get; set; }

        public double before { get; set; }

        public DateTimeOffset date { get; set; }

        public string itemCode { get; set; }

        public double itemDomesticCOGS { get; set; }

        public double itemDomesticRetail { get; set; }

        public double itemDomesticSale { get; set; }

        public double itemDomesticWholeSale { get; set; }

        public long itemId { get; set; }

        public double itemInternationalCOGS { get; set; }

        public double itemInternationalRetail { get; set; }

        public double itemInternationalSale { get; set; }

        public double itemInternationalWholeSale { get; set; }

        public string itemName { get; set; }

        public string itemSize { get; set; }

        public string itemUom { get; set; }

        public double quantity { get; set; }

        public string storageCode { get; set; }

        public long storageId { get; set; }

        public bool storageIsCentral { get; set; }

        public string storageName { get; set; }

        public string type { get; set; }
    }
}
