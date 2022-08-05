using Com.Moonlay.Models;
using System.ComponentModel.DataAnnotations;

namespace Com.Bateeq.Service.Warehouse.Lib.Utilities
{
    public abstract class BaseModel : StandardEntity<long>
    {
        [MaxLength(255)]
        public string UId { get; set; } /* Object Id MongoDb */
    }
}
