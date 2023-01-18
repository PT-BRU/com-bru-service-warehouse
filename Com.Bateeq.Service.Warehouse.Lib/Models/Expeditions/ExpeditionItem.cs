using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions
{
    public class ExpeditionItem : BaseModel
    {
        [MaxLength(255)]
        public string Code { get; set; }
        public DateTimeOffset Date { get; set; }
        public int DestinationId { get; set; }
        [MaxLength(255)]
        public string DestinationCode { get; set; }
        [MaxLength(255)]
        public string DestinationName { get; set; }
        public bool IsDistributed { get; set; }
        public bool IsDraft { get; set; }
        public bool IsReceived { get; set; }
        [MaxLength(255)]
        public string PackingList { get; set; }
        [MaxLength(255)]
        public string Password { get; set; }
        [MaxLength(255)]
        public string Reference { get; set; }
        public int SourceId { get; set; }
        [MaxLength(255)]
        public string SourceCode { get; set; }
        [MaxLength(255)]
        public string SourceName { get; set; }
        public int SPKDocsId { get; set; }
        public double Weight { get; set; }
        public virtual ICollection<ExpeditionDetail> Details { get; set; }
        public virtual long ExpeditionId { get; set; }
        [ForeignKey("ExpeditionId")]
        public virtual Expedition Expeditions { get; set; }
        


    }
}
