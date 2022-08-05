using System;
using System.Collections.Generic;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel
{
    public class SPKDocItemsFromFinihsingOutsViewModel
    {
        public DateTimeOffset FinishingInDate { get; set; }
        public string FinishingInItemId { get; set; }
        public string FinishingInId { get; set; }
        public double Quantity { get; set; }
        public Product Product { get; set; }
        public SizeObj Size { get; set; }
        public Uom Uom { get; set; }
        public double BasicPrice { get; set; }
        public double ComodityPrice { get; set; }
        public bool IsDifferentSize { get; set; }
        public bool IsSave { get; set; }
        public double RemainingQuantity { get; set; }
        public double Price { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalFinishingOutQuantity { get; set; }
        public List<Details> Details { get; set; }
    }

    public class Details
    {
        public Product ParentProduct { get; set; }
        public SizeObj Size { get; set; }
        public Uom Uom { get; set; }
        public double Quantity { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class SizeObj
    {
        public int Id { get; set; }
        public string Size { get; set; }
    }

    public class Uom
    {
        public int Id { get; set; }
        public string Unit { get; set; }
    }
}
