using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions
{
    public class ExpeditionDetail : BaseModel
    {
        [MaxLength(255)]
        public string ArticleRealizationOrder { get; set; }
        public double DomesticCOGS { get; set; }
        public double DomesticRetail { get; set; }
        public double DomesticSale { get; set; }
        public double DomesticWholesale { get; set; }
        [MaxLength(255)]
        public string ItemCode { get; set; }
        public long ItemId { get; set; }
        [MaxLength(1000)]
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double SendQuantity { get; set; }
        [MaxLength(255)]
        public string Size { get; set; }
        public string Remark { get; set; }
        public int SPKDocsId { get; set; }
        [MaxLength(255)]
        public string Uom { get; set; }
        public virtual long ExpeditionItemId { get; set; }
        [ForeignKey("ExpeditionItemId")]
        public virtual ExpeditionItem ExpeditionItems { get; set; }
    }
}
