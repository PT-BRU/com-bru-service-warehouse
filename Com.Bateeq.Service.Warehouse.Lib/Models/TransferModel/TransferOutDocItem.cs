using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel
{
    public class TransferOutDocItem : BaseModel
    {
        [MaxLength(255)]
        public string ArticleRealizationOrder { get; set; }

        public double DomesticCOGS { get; set; }

        public double DomesticRetail { get; set; }

        public double DomesticSale { get; set; }

        public double DomesticWholeSale { get; set; }

        [MaxLength(255)]
        public string ItemCode { get; set; }

        public long ItemId { get; set; }

        [MaxLength(255)]
        public string ItemName { get; set; }

        public double Quantity { get; set; }

        [MaxLength(1000)]
        public string Remark { get; set; }

        public string Size { get; set; }

        public virtual long TransferOutDocsId { get; set; }
        [ForeignKey("TransferOutDocsId")]
        public virtual TransferOutDoc TransferOutDocs { get; set; }

        public string Uom { get; set; }
    }
}
