using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.SPKDocControllerFacades
{
    public class SPKDocControllerFacedeTests
    {
        private const string ENTITY = "MMInventory";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }
        
        private Mock<IServiceProvider> GetServiceProvider()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":7,\"code\":\"USD\",\"rate\":13700.0,\"date\":\"2018/10/20\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            HttpResponseMessage messagePost = new HttpResponseMessage();
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);
            HttpClientService
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(messagePost);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(HttpClientService.Object);

            return serviceProvider;
        }
        
        private WarehouseDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<WarehouseDbContext> optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            WarehouseDbContext dbContext = new WarehouseDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private SPKDocsFromFinihsingOutsViewModel ViewModel
        {
            get
            {
                return new SPKDocsFromFinihsingOutsViewModel
                {
                    FinishingOutDate = DateTimeOffset.Now,
                    UnitTo = new UnitObj
                    {
                        Id = 1,
                        code = "code",
                        name = "name"
                    },
                    Unit = new UnitObj
                    {
                        code = "code",
                        name = "name",
                        Id = 1
                    },
                    PackingList = "0001/FER/08/21",
                    Password = "pass",
                    IsDifferentSize = false,
                    Weight = 0,
                    Comodity = new Comodity()
                    {
                        code = "code",
                        name = "name",
                        id = 1
                    },
                    RONo = "2110003",
                    Items = new List<SPKDocItemsFromFinihsingOutsViewModel>
                    {
                        new SPKDocItemsFromFinihsingOutsViewModel
                        {
                            Quantity = 20,
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
                            BasicPrice = 1000,
                            ComodityPrice = 10000,
                            IsDifferentSize = false,
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
                    },
                    counters = new ItemArticleCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    subCounters = new ItemArticleSubCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    SourceId = 1,
                    RoCreatedUtc = "2110",
                    materials = new ItemArticleMaterialViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    }
                };
            }
        }
        
        private SPKDocsFromFinihsingOutsViewModel ViewModelItemExist
        {
            get
            {
                return new SPKDocsFromFinihsingOutsViewModel
                {
                    FinishingOutDate = DateTimeOffset.Now,
                    UnitTo = new UnitObj
                    {
                        Id = 1,
                        code = "code",
                        name = "name"
                    },
                    Unit = new UnitObj
                    {
                        code = "code",
                        name = "name",
                        Id = 1
                    },
                    PackingList = "0001/FER/08/21",
                    Password = "pass",
                    IsDifferentSize = false,
                    Weight = 0,
                    Comodity = new Comodity()
                    {
                        code = "code",
                        name = "name",
                        id = 1
                    },
                    RONo = "2110003",
                    Items = new List<SPKDocItemsFromFinihsingOutsViewModel>
                    {
                        new SPKDocItemsFromFinihsingOutsViewModel
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
                            IsDifferentSize = false,
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
                    },
                    counters = new ItemArticleCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    subCounters = new ItemArticleSubCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    SourceId = 1,
                    RoCreatedUtc = "2110",
                    materials = new ItemArticleMaterialViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    }
                };
            }
        }
        
        private SPKDocsFromFinihsingOutsViewModel ViewModelDifferentSize
        {
            get
            {
                return new SPKDocsFromFinihsingOutsViewModel
                {
                    FinishingOutDate = DateTimeOffset.Now,
                    UnitTo = new UnitObj
                    {
                        Id = 1,
                        code = "code",
                        name = "name"
                    },
                    Unit = new UnitObj
                    {
                        code = "code",
                        name = "name",
                        Id = 1
                    },
                    PackingList = "0001/FER/08/21",
                    Password = "pass",
                    IsDifferentSize = true,
                    Weight = 0,
                    Comodity = new Comodity()
                    {
                        code = "code",
                        name = "name",
                        id = 1
                    },
                    RONo = "2110003",
                    Items = new List<SPKDocItemsFromFinihsingOutsViewModel>
                    {
                        new SPKDocItemsFromFinihsingOutsViewModel
                        {
                            Quantity = 20,
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
                    },
                    counters = new ItemArticleCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    subCounters = new ItemArticleSubCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    SourceId = 1,
                    RoCreatedUtc = "2110",
                    materials = new ItemArticleMaterialViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    }
                };
            }
        }
        
        private SPKDocsFromFinihsingOutsViewModel ViewModelDifferentSizeItemExist
        {
            get
            {
                return new SPKDocsFromFinihsingOutsViewModel
                {
                    FinishingOutDate = DateTimeOffset.Now,
                    UnitTo = new UnitObj
                    {
                        Id = 1,
                        code = "code",
                        name = "name"
                    },
                    Unit = new UnitObj
                    {
                        code = "code",
                        name = "name",
                        Id = 1
                    },
                    PackingList = "0001/FER/08/21",
                    Password = "pass",
                    IsDifferentSize = true,
                    Weight = 0,
                    Comodity = new Comodity()
                    {
                        code = "code",
                        name = "name",
                        id = 1
                    },
                    RONo = "2110003",
                    Items = new List<SPKDocItemsFromFinihsingOutsViewModel>
                    {
                        new SPKDocItemsFromFinihsingOutsViewModel
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
                                        Id = 12345,
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
                                        Id = 23456,
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
                    },
                    counters = new ItemArticleCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    subCounters = new ItemArticleSubCounterViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    SourceId = 1,
                    RoCreatedUtc = "2110",
                    materials = new ItemArticleMaterialViewModel()
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    }
                };
            }
        }
        
        private Inventory inventoryModel
        {
            get
            {
                return new Inventory
                {
                    ItemArticleRealizationOrder = "123",
                    ItemCode = "112345",
                    ItemDomesticCOGS = 50000,
                    ItemDomesticSale = 500000,
                    ItemId = 1,
                    ItemName = "name",
                    ItemSize = "S",
                    ItemUom = "PCS",
                    Quantity = 10,
                    ItemDomesticRetail = 0,
                    ItemDomesticWholeSale = 0,
                    ItemInternationalCOGS = 0,
                    ItemInternationalRetail = 0,
                    ItemInternationalSale = 0,
                    ItemInternationalWholeSale = 0,
                    StorageId = 2,
                    StorageCode = "code",
                    StorageName = "name",
                    StorageIsCentral = false,
                };
            }
        }
        
        private SPKDocs spkDocsModel
        {
            get
            {
                return new SPKDocs
                {
                    Code = "code",
                    Date = DateTimeOffset.Now,
                    DestinationId = 1,
                    DestinationCode = "code",
                    DestinationName = "name",
                    IsDistributed = false,
                    IsDraft = false,
                    IsReceived = false,
                    PackingList = "ERF-12",
                    Password = "1",
                    Reference = "ERF-12",
                    SourceId = 1,
                    SourceCode = "1",
                    SourceName = "source",
                    Weight = 0,
                    FinishingOutIdentity = "00123"
                };
            }
        }
        
        [Fact]
        public async Task Should_Success_Create()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            SPKDocsControllerFacade facade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            var Response = await facade.Create(this.ViewModel, "username", "Bearer");
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Item_Exist()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            SPKDocsControllerFacade facade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            dbSetInventory.Add(this.inventoryModel);
            var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            
            var Response = await facade.Create(this.ViewModelItemExist, "username", "Bearer");
            Assert.NotEqual(0, Response);
        }
        
        [Fact]
        public async Task Should_Success_Create_DifferentSize()
        {
            string itemUri = "items/finished-goods/Code";
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            SPKDocsControllerFacade facade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = null
                });
            var Response = await facade.Create(this.ViewModelDifferentSize, "username", "Bearer");
            Assert.NotEqual(0, Response);
        }
        
        [Fact]
        public async Task Should_Success_Create_DifferentSize_Item_Exist()
        {
            string itemUri = "items/finished-goods/Code";
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            SPKDocsControllerFacade facade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var mockHttpClient = new Mock<IHttpClientService>();
            mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = null
                });
            var Response = await facade.Create(this.ViewModelDifferentSizeItemExist, "username", "Bearer");
            Assert.NotEqual(0, Response);
        }
        
        [Fact]
        public async Task Should_Success_ReadByFinishingOutIdentity()
        {
            DbSet<SPKDocs> dbSet = _dbContext(GetCurrentMethod()).Set<SPKDocs>();
            
            dbSet.Add(this.spkDocsModel);
            await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            SPKDocsControllerFacade facade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            var Response =  facade.ReadByFinishingOutIdentity("00123");
            Assert.NotEqual(null, Response);
        }


    }
}