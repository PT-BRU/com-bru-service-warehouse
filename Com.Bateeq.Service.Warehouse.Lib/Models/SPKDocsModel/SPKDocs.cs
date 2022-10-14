using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel
{
    public class SPKDocs : BaseModel
    {
        [MaxLength(255)]
        public string Code { get; set; }
        public DateTimeOffset Date { get; set; }

        public long DestinationId { get; set; }
        [MaxLength(255)]
        public string DestinationCode { get; set; }
        [MaxLength(1000)]
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

        /*Source*/
        public long SourceId { get; set; }
        [MaxLength(255)]
        public string SourceCode { get; set; }
        [MaxLength(1000)]
        public string SourceName { get; set; }

        public int Weight { get; set; }

        [MaxLength(255)]
        public string FinishingOutIdentity { get; set; }

        public virtual ICollection<SPKDocsItem> Items { get; set; }
    }
}
