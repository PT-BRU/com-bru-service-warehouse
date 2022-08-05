using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel
{
    public class SPKDocsItem : StandardEntity<long>
    {
        //[MaxLength(255)]
        //public string ArticleRealizationOrder { get; set; }
        //public double DomesticCOGS { get; set; }
        //public double DomesticRetail { get; set; }
        //public double DomesticSale { get; set; }
        //public double DomesticWholesale { get; set; }

        /*Item*/
        [MaxLength(255)]
        public string ItemArticleRealizationOrder { get; set; }
        [MaxLength(255)]
        public string ItemCode { get; set; }
        public double ItemDomesticCOGS { get; set; }
        public double ItemDomesticRetail { get; set; }
        public double ItemDomesticSale { get; set; }
        public double ItemDomesticWholesale { get; set; }
        public long ItemId { get; set; }
        [MaxLength(1000)]
        public string ItemName { get; set; }
        [MaxLength(255)]
        public string ItemSize { get; set; }
        [MaxLength(255)]
        public string ItemUom { get; set; }

        public double Quantity { get; set; }
        public string Remark { get; set; }
        public double SendQuantity { get; set; }

        //[MaxLength(255)]
        //public string Size { get; set; }
        //[MaxLength(255)]
        //public string Uom { get; set; }
        [MaxLength(255)]
        public string UId { get; set; }

        public virtual long SPKDocsId { get; set; }
        [ForeignKey("SPKDocsId")]
        public virtual SPKDocs SPKDocs { get; set; }
    }
}
