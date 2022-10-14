using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels
{
    public class TransferOutDocViewModel : BaseViewModel, IValidatableObject
    {
        public string code { get; set; }

        public DateTimeOffset date { get; set; }

        public DestinationViewModel destination { get; set; }

        public string reference { get; set; }

        public ExpeditionServiceViewModel expeditionService { get; set; }

        public string remark { get; set; }

        public SourceViewModel source { get; set; }

        public List<TransferOutDocItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            //{
            //    yield return new ValidationResult("Date is required", new List<string> { "date" });
            //}
            if (this.destination == null)
            {
                yield return new ValidationResult("Destination is required", new List<string> { "destination" });
            }
            if (this.source == null)
            {
                yield return new ValidationResult("Source is required", new List<string> { "source" });
            }
            //if (this. == null || this.reference == "")
            //{
            //    yield return new ValidationResult("Reference is required", new List<string> { "reference" });
            //}
            int itemErrorCount = 0;
            if (this.items == null || this.items.Count == 0)
            {
                yield return new ValidationResult("Item is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";
                foreach (var item in items)
                {
                    itemError += "{";

                    //if (item.item == null)
                    //{
                    //    itemErrorCount++;
                    //    itemError += "item: 'No item selected', ";
                    //}

                    WarehouseDbContext warehouseDbContext = (WarehouseDbContext)validationContext.GetService(typeof(WarehouseDbContext));
                    var qtyinven = warehouseDbContext.Inventories.Where(x => x.ItemCode == item.item.code && x.StorageId == source._id).Select(x => x.Quantity).FirstOrDefault();
                    if (item.quantity > qtyinven) {
                        itemErrorCount++;
                        itemError += "quantity: 'Qty can't more than inven quantity', ";
                    }

                    //if(item.remark == null || item.remark == "")
                    //{
                    //    itemErrorCount++;
                    //    itemError += "remark: 'remark is required', ";
                    //}

                    itemError += "}, ";
                }

                itemError += "]";

                if (itemErrorCount > 0)
                    yield return new ValidationResult(itemError, new List<string> { "items" });
            }
            //int itemErrorCount = 0;

            //if (this.items.Count.Equals(0))
            //{
            //    yield return new ValidationResult("Items is required", new List<string> { "itemscount" });
            //}
            //else
            //{
            //    string itemError = "[";

            //    foreach (PurchaseRequestItemViewModel item in items)
            //    {
            //        itemError += "{";

            //        if (item.product == null || string.IsNullOrWhiteSpace(item.product._id))
            //        {
            //            itemErrorCount++;
            //            itemError += "product: 'Product is required', ";
            //        }
            //        else
            //        {
            //            var itemsExist = items.Where(i => i.product != null && item.product != null && i.product._id.Equals(item.product._id)).Count();
            //            if (itemsExist > 1)
            //            {
            //                itemErrorCount++;
            //                itemError += "product: 'Product is duplicate', ";
            //            }
            //        }

            //        if (item.quantity <= 0)
            //        {
            //            itemErrorCount++;
            //            itemError += "quantity: 'Quantity should be more than 0'";
            //        }

            //        itemError += "}, ";
            //    }

            //    itemError += "]";

            //    if (itemErrorCount > 0)
            //        yield return new ValidationResult(itemError, new List<string> { "items" });
            //}
        }
    }
}
