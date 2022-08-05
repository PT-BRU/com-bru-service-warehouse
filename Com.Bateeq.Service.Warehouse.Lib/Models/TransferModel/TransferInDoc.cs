using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel
{
    public class TransferInDoc : BaseModel
    {
        public string Code { get; set; }

        public DateTimeOffset Date { get; set; }

        public long DestinationId { get; set; }

        [MaxLength(255)]
        public string DestinationCode { get; set; }

        [MaxLength(255)]
        public string DestinationName { get; set; }

        [MaxLength(255)]
        public string Reference { get; set; }

        [MaxLength(1000)]
        public string Remark { get; set; }

        public long SourceId { get; set; }

        [MaxLength(255)]
        public string SourceCode { get; set; }

        [MaxLength(255)]
        public string SourceName { get; set; }

        public virtual ICollection<TransferInDocItem> Items { get; set; }

    }
}
