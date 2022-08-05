using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.TransferInFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMTransferIn";

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

        private TransferDataUtil dataUtil(TransferFacade facade, string testName)
        {
            var pkbbjfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(ServiceProvider, _dbContext(testName));
            var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
            var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
            var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

            return new TransferDataUtil(facade, sPKDocDataUtil);
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

        //[Fact]
        public async Task Should_Success_Create_Data()
        {
            SPKDocsControllerFacade spkDocsControllerFacade = new SPKDocsControllerFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            
            await spkDocsControllerFacade.Create(this.ViewModel, "username", "Bearer");
            
            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData2();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        //[Fact]
        //public async Task Should_Success_Create_Data_2()
        //{
        //    DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
        //    Inventory inventory = new Inventory
        //    {
        //        ItemArticleRealizationOrder = "art1",
        //        ItemCode = "code",
        //        ItemDomesticCOGS = 0,
        //        ItemDomesticRetail = 0,
        //        ItemDomesticSale = 0,
        //        ItemDomesticWholeSale = 0,
        //        ItemId = 1,
        //        ItemInternationalCOGS = 0,
        //        ItemInternationalRetail = 0,
        //        ItemInternationalSale = 0,
        //        ItemInternationalWholeSale = 0,
        //        ItemName = "name",
        //        ItemSize = "size",
        //        Quantity = 1,
        //        ItemUom = "uom",
        //        StorageCode = "code",
        //        StorageId = 1,
        //        StorageIsCentral = false,
        //        StorageName = "name",

        //    };
        //    dbSetInventory.Add(inventory);
        //    var Created = _dbContext(GetCurrentMethod()).SaveChangesAsync();

        //    TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
        //    model.DestinationId = inventory.StorageId;
        //    model.DestinationName = inventory.StorageName;
        //    model.DestinationCode = inventory.StorageCode;
        //    foreach(var item in model.Items)
        //    {
        //        item.ItemId = inventory.ItemId;

        //    }
        //    var Response = await facade.Create(model, USERNAME);
        //    Assert.NotEqual(0, Response);
        //}

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
                    PackingList = "EFR-FN",
                    Password = "1",
                    Reference = "EFR-FN",
                    SourceId = 1,
                    SourceCode = "1",
                    SourceName = "source",
                    Weight = 0,
                    FinishingOutIdentity = "00123"
                };
            }
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            DbSet<SPKDocs> dbSet = _dbContext(GetCurrentMethod()).Set<SPKDocs>();

            dbSet.Add(this.spkDocsModel);
            await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Response = facade.Read();
            Assert.NotEqual(null, Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }
    }
}
