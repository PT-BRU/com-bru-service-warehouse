using System;
using System.Collections.Generic;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.SPKDocControllerFacades
{
    public class SPKDocsFromFinihsingOutsViewModelTest
    {
        [Fact]
        public void should_Success_Instantiate()
        {
            var dateTimeOffset = DateTimeOffset.Now.Date; 
            
            var listItem = new List<SPKDocItemsFromFinihsingOutsViewModel>()
            {
                new SPKDocItemsFromFinihsingOutsViewModel()
                {
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
                    }
                }
            };
            
            SPKDocsFromFinihsingOutsViewModel dto = new SPKDocsFromFinihsingOutsViewModel
            {
                FinishingOutDate = dateTimeOffset,
                UnitTo = new UnitObj()
                {
                    Id = 1,
                    code = "1",
                    name = "name"
                },
                Unit = new UnitObj(){
                    Id = 1,
                    code = "1",
                    name = "name"
                },
                PackingList = "123",
                Password = "1",
                IsDifferentSize = true,
                Weight = 0,
                Comodity = new Comodity()
                {
                    code = "1",
                    id = 1,
                    name = "name"
                },
                process = new ItemArticleProcesViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                materialCompositions = new ItemArticleMaterialCompositionViewModel()
                {
                _id = 1,
                code = "1",
                name = "name"
                },
                materials = new ItemArticleMaterialViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                collections = new ItemArticleCollectionViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                seasons = new ItemArticleSeasonViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                counters = new ItemArticleCounterViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                subCounters = new ItemArticleSubCounterViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                categories = new ItemArticleCategoryViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                color = new ItemArticleColorViewModel()
                {
                    _id = 1,
                    code = "1",
                    name = "name"
                },
                RONo = "123",
                ImagePath = "/sales/",
                ImgFile = "imgFile",
                Items = listItem,
                SourceStorageId = 1,
                SourceStorageName = "storage",
                SourceStorageCode = "code",
                RoCreatedUtc = "2110",
                SourceId = 1,
                DestinationStorageName = "destination",
                DestinationStorageCode = "code",
                DestinationStorageId = 1
            };
            
            Assert.Equal(dto.FinishingOutDate, dateTimeOffset);
            Assert.NotNull(dto.UnitTo);
            Assert.NotNull(dto.Unit);
            Assert.Equal(dto.PackingList, "123");
            Assert.Equal(dto.Password, "1");
            Assert.Equal(dto.IsDifferentSize, true);
            Assert.Equal(dto.Weight, 0);
            Assert.NotNull(dto.Comodity);
            Assert.NotNull(dto.process);
            Assert.NotNull(dto.materialCompositions);
            Assert.NotNull(dto.materials);
            Assert.NotNull(dto.collections);
            Assert.NotNull(dto.seasons);
            Assert.NotNull(dto.counters);
            Assert.NotNull(dto.subCounters);
            Assert.NotNull(dto.categories);
            Assert.NotNull(dto.color);
            Assert.Equal(dto.RONo,"123");
            Assert.Equal(dto.ImagePath,"/sales/");
            Assert.Equal(dto.ImgFile,"imgFile");
            Assert.NotEmpty(dto.Items);
            Assert.Equal(dto.SourceStorageId,1);
            Assert.Equal(dto.SourceStorageName,"storage");
            Assert.Equal(dto.SourceStorageCode,"code");
            Assert.Equal(dto.RoCreatedUtc,"2110");
            Assert.Equal(dto.SourceId,1);
            Assert.Equal(dto.DestinationStorageId,1);
            Assert.Equal(dto.DestinationStorageName,"destination");
            Assert.Equal(dto.DestinationStorageCode,"code");
        }
    }
}