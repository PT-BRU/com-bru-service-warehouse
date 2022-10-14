using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel
{
    public class AdjustmentDocs : BaseModel
    {

        [MaxLength(255)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string StorageCode { get; set; }

        public long StorageId { get; set; }

        [MaxLength(255)]
        public string StorageName { get; set; }

        public virtual ICollection<AdjustmentDocsItem> Items { get; set; }
    }
}
