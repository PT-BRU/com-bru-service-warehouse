using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel
{
    public class AdjustmentDocsViewModel : BaseViewModel, IValidatableObject
    {
        public string UId { get; set; }
        public string code { get; set; }
       
        public StorageViewModel storage { get; set; }

        public List<AdjustmentDocsItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(this.storage == null)
            {
                yield return new ValidationResult("Storage is required", new List<string> { "storage" });
            }

            int itemErrorCount = 0;

            if (this.items.Count.Equals(0))
            {
                yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";

                foreach (AdjustmentDocsItemViewModel item in items)
                {
                    itemError += "{";

                    if (item.item == null || item.item._id == 0)
                    {
                        itemErrorCount++;
                        itemError += "Item: 'Item is required', ";
                    }
                    else
                    {
                        var itemsExist = items.Where(i => i.item._id != 0 && i.item._id.Equals(item.item._id)).Count();
                        if (itemsExist > 1)
                        {
                            itemErrorCount++;
                            itemError += "name: 'Item is duplicate', ";
                        }
                    }

                    if(item.type == "OUT") {
                        if (item.qtyBeforeAdjustment - item.qtyAdjustment < 0)
                        {
                            itemErrorCount++;
                            itemError += "quantity: 'Quantity after adjustment cant be below 0', ";
                        }
                    }

                    if (item.qtyAdjustment == 0)
                    {
                        itemErrorCount++;
                        itemError += "qtyAdjustment: 'Adjustment quantity must be greater than 0', ";
                    }

                    if (string.IsNullOrWhiteSpace(item.remark))
                    {
                        itemErrorCount++;
                        itemError += "remark: 'Remark cant be null ', ";
                    }

                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "items" });
            }
        }
    }
}
