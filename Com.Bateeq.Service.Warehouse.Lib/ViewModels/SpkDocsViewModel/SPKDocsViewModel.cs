using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel
{
    public class SPKDocsViewModel : BaseViewModel, IValidatableObject
    {
        public string uId { get; set; }
        public string code { get; set; }
        public DateTimeOffset? date { get; set; }
       
        public DestinationViewModel destination { get; set; }
		public string destinationName { get; set; }
		//public UnitViewModel unit { get; set; }
		//public CategoryViewModel category { get; set; }
		public bool isDistributed { get; set; }
        public bool isDraft { get; set; }
        public bool isReceived { get; set; }
		public string status { get; set; }

		public string packingList { get; set; }
        public string password { get; set; }
        public string reference { get; set; }

        public SourceViewModel source { get; set; }
		public string sourceName { get; set; }
		public string sourceCode { get; set; }
		public string type { get; set; }

        public int Weight { get; set; }
        public List<SPKDocsItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.date.Equals(DateTimeOffset.MinValue) || this.date == null)
            {
                yield return new ValidationResult("Date is required", new List<string> { "date" });
            }
            if (this.destination == null)
            {
                yield return new ValidationResult("Destination is required", new List<string> { "destination" });
            }
            if (this.source == null)
            {
                yield return new ValidationResult("Source is required", new List<string> { "source" });
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
