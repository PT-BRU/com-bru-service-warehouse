using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel
{
    public class PkbjByUserViewModel : BaseViewModel, IValidatableObject
    {
        public string code { get; set; }
        public DateTimeOffset date { get; set; }
        public DestinationViewModel destination { get; set; }
        public bool isDistributed { get; set; }
        public bool isDraft { get; set; }
        public bool isReceived { get; set; }
        public string packingList { get; set; }
        public string password { get; set; }
        public string reference { get; set; }
        public SourceViewModel source { get; set; }
        public int weight { get; set; }
        public List<PkbjByUserItemViewModel> items { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.source == null)
            {
                yield return new ValidationResult("Source is required", new List<string> { "source" });
            }
            if (this.destination == null)
            {
                yield return new ValidationResult("Destination is required", new List<string> { "destination" });
            }
            int itemErrorCount = 0;
            if (this.items == null || this.items.Count == 0)
            {
                yield return new ValidationResult("items is required", new List<string> { "itemscount" });
            }
            else
            {
                string itemError = "[";
                foreach (var item in items)
                {
                    itemError += "{";

                    if (item == null)
                    {
                        itemErrorCount++;
                        itemError += "item: 'No item selected', ";
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
