using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.SOModel
{
    public class SODocsItem : StandardEntity<long>
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
        [MaxLength(255)]
        public string ItemName { get; set; }

        public double ItemDomesticCOGS { get; set; }
        public double ItemDomesticRetail { get; set; }
        public double ItemDomesticSale { get; set; }
        public double ItemDomesticWholeSale { get; set; }
        public double ItemInternationalCOGS  { get; set; }
        public double ItemInternationalRetail { get; set; }
        public double ItemInternationalSale { get; set; }
        public double ItemInternationalWholeSale { get; set; }
        public long ItemId { get; set; }
        public string ItemSize { get; set; }
        public string ItemUom { get; set; }
        [MaxLength(1000)]
        public double QtyBeforeSO { get; set; }
        public double QtySO { get; set; }
        public string Remark { get; set; }
        [MaxLength(255)]
        public string UId { get; set; }
        public bool IsAdjusted { get; set; }
        [ForeignKey("SODocsId")]
        public virtual long SODocsId { get; set; }
        public virtual SODocs SODocs { get; set; }
    }
}
