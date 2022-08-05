using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel
{
    class StoreViewModel : BaseViewModel
    {
        public string Address { get; set; }
        public string City { get; set; }
        public DateTimeOffset ClosedDate { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public float MonthlyTotalCost { get; set; }
        public string Name { get; set; }
        public string OnlineOffline { get; set; }
        public DateTimeOffset OpenedDate { get; set; }
        public string Pic { get; set; }
        public string Phone { get; set; }
        public float SalesCapital { get; set; }
        public string SalesCategory { get; set; }
        public float SalesTarget { get; set; }
        public string Status { get; set; }
        public string StoreArea { get; set; }
        public string StoreCategory { get; set; }
        public string StoreWide { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
