using System;
using System.Collections.Generic;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.SPKDocControllerFacades
{
    public class SPKDocItemsFromFinihsingOutsViewModelTest
    {
        [Fact]
        public void should_Success_Instantiate()
        {
            var dateTimeOffset = DateTimeOffset.Now.Date; 
            
            SPKDocItemsFromFinihsingOutsViewModel item = new SPKDocItemsFromFinihsingOutsViewModel()
            {
                FinishingInDate = dateTimeOffset,
                FinishingInId = "1",
                FinishingInItemId = "1",
                Product = new Product()
                {
                    Code = "1",
                    Id = 1,
                    Name = "name"
                },
                Quantity = 20,
                Size = new SizeObj()
                {
                    Id = 12345,
                    Size = "S"
                },
                Uom = new Uom()
                {
                    Id = 1,
                    Unit = "PCS"
                },
                BasicPrice = 1000,
                ComodityPrice = 10000,
                IsDifferentSize = true,
                Details = new List<Details>()
                {
                    new Details()
                    {
                        ParentProduct = new Product()
                        {
                            Id = 1,
                            Name = "Baju",
                            Code = "1231"
                        },
                        Size = new SizeObj()
                        {
                            Id = 1,
                            Size = "S"
                        },
                        Uom = new Uom()
                        {
                            Id = 1,
                            Unit = "PCS"
                        },
                        Quantity = 10
                    },
                    new Details()
                    {
                        ParentProduct = new Product()
                        {
                            Id = 1,
                            Name = "Baju",
                            Code = "1231"
                        },
                        Size = new SizeObj()
                        {
                            Id = 2,
                            Size = "M"
                        },
                        Uom = new Uom()
                        {
                            Id = 1,
                            Unit = "PCS"
                        },
                        Quantity = 10
                    }
                },
                IsSave = true,
                Price = 10,
                RemainingQuantity = 2,
                TotalQuantity = 10,
                TotalFinishingOutQuantity = 10
            };
            
            Assert.Equal(item.FinishingInDate, dateTimeOffset);
            Assert.NotNull(item.FinishingInId);
            Assert.NotNull(item.FinishingInItemId);
            Assert.NotNull(item.Product);
            Assert.NotNull(item.Quantity);
            Assert.NotNull(item.Size);
            Assert.NotNull(item.Uom);
            Assert.NotNull(item.BasicPrice);
            Assert.NotNull(item.ComodityPrice);
            Assert.NotNull(item.IsDifferentSize);
            Assert.NotEmpty(item.Details);
            Assert.Equal(item.IsSave, true);
            Assert.Equal(item.Price, 10);
            Assert.NotSame(item.Price, 0);
            Assert.NotSame(item.TotalFinishingOutQuantity, 0);
            Assert.NotSame(item.TotalQuantity, 0);
        }
    }
}