using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.ExpeditionDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.ExpeditionFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMExpeditionFacade";

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

        private Mock<IServiceProvider> GetServiceProvidernulldataget()
        {
            HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{ }}");
            HttpResponseMessage messagePost = new HttpResponseMessage();
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);
            //HttpClientService
            //    .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            //    .ReturnsAsync(messagePost);
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

        private ExpeditionDataUtil dataUtil(ExpeditionFacade facade, string testName)
        {
            var expeditionFacade = new ExpeditionFacade(ServiceProvider, _dbContext(testName));
            var inventoryFacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            var pkpbjFacade = new PkpbjFacade(ServiceProvider, _dbContext(testName));
            var sPKDocDataUtil = new SPKDocDataUtil(pkpbjFacade);
            var inventoryDataUtil = new InventoryDataUtil(inventoryFacade, _dbContext(testName));
            
            //var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

            return new ExpeditionDataUtil(expeditionFacade,inventoryDataUtil,sPKDocDataUtil);
        }

        private InventoryDataUtil invendataUtil(InventoryFacade facade, string testName, WarehouseDbContext dbContext)
        {
            var pkbbjfacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            //var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
            //var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
            //var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

            return new InventoryDataUtil(facade, dbContext);
        }

		[Fact]
		public async Task Should_Success_Create_Data()
		{

			ExpeditionFacade facade = new ExpeditionFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetNewData2();
			
			var Response = await facade.Create(model, USERNAME);
			Assert.NotEqual(0, Response);
		}

		[Fact]
        public async Task Should_Exceptioon_Create_Data()
        {

            ExpeditionFacade facade = new ExpeditionFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();

            foreach (var item in model.Items)
            {
                item.PackingList = null;
            }

            //var Response = await facade.Create(model, USERNAME);

            await Assert.ThrowsAnyAsync<Exception>(() => facade.Create(model, USERNAME));
            //Assert.Equal(0, Response);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            ExpeditionFacade facade = new ExpeditionFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData2();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Error_Get_Data_By_Id()
        {
            ExpeditionFacade facade = new ExpeditionFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = facade.ReadById((int)model.Id);
            Assert.Null(Response);
        }

        [Fact]
        public async Task Should_Success_Validate_DataAsync()
        {

            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();

            ExpeditionViewModel ViewModel = new ExpeditionViewModel
            {

            };
            Assert.True(ViewModel.Validate(null).Count() > 0);

            ExpeditionViewModel expservice = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                

            };
            Assert.True(expservice.Validate(null).Count() > 0);

            ExpeditionViewModel itemViewModel = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                items = new List<ExpeditionItemViewModel> {
                    new ExpeditionItemViewModel
                    {
                        spkDocsViewModel = new Lib.ViewModels.SpkDocsViewModel.SPKDocsViewModel
                        {
                            code = "test",
                            Weight = 0
                        }
                    }
                }

            };
            Assert.True(itemViewModel.Validate(null).Count() > 0);

            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await invendataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
            ExpeditionViewModel detailViewModel = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                items = new List<ExpeditionItemViewModel> {
                    new ExpeditionItemViewModel
                    {
                        spkDocsViewModel = new Lib.ViewModels.SpkDocsViewModel.SPKDocsViewModel
                        {
                            code = "test",
                            Weight = 5,
                            source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                            {
                                code = model.StorageCode,
                                name = model.StorageName,
                                _id = model.StorageId
                            }

                        },
                        details = new List<ExpeditionDetailViewModel>{
                            new ExpeditionDetailViewModel
                            {
                                sendQuantity = 0,
                                item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                                {
                                    articleRealizationOrder = model.ItemArticleRealizationOrder,
                                    code = model.ItemCode,
                                    name = model.ItemName,

                                }
                            }
                        }
                    }
                }
            };

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext)))
                  .Returns(_dbContext(GetCurrentMethod()));

            var validationContext = new ValidationContext(detailViewModel, mockServiceProvider.Object, null);

            Assert.True(detailViewModel.Validate(validationContext).Count() > 0);

            ExpeditionViewModel detailViewModel2 = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                items = new List<ExpeditionItemViewModel> {
                    new ExpeditionItemViewModel
                    {
                        spkDocsViewModel = new Lib.ViewModels.SpkDocsViewModel.SPKDocsViewModel
                        {
                            code = "test",
                            Weight = 5,
                            source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                            {
                                code = model.StorageCode,
                                name = model.StorageName,
                                _id = model.StorageId
                            }

                        },
                        details = new List<ExpeditionDetailViewModel>{
                            new ExpeditionDetailViewModel
                            {
                                sendQuantity = 2,
                                item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                                {
                                    articleRealizationOrder = model.ItemArticleRealizationOrder,
                                    code = model.ItemCode,
                                    name = model.ItemName,
                                    
                                }
                            }
                        }
                    }
                }
            };
            Assert.True(detailViewModel2.Validate(validationContext).Count() > 0);

            ExpeditionViewModel detailViewModel3 = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                items = new List<ExpeditionItemViewModel> {
                    new ExpeditionItemViewModel
                    {
                        spkDocsViewModel = new Lib.ViewModels.SpkDocsViewModel.SPKDocsViewModel
                        {
                            code = "test",
                            Weight = 5,
                            source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                            {
                                code = model.StorageCode,
                                name = model.StorageName,
                                _id = model.StorageId
                            }

                        },
                        details = new List<ExpeditionDetailViewModel>{
                            new ExpeditionDetailViewModel
                            {
                                sendQuantity = model.Quantity,
                                item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                                {
                                    articleRealizationOrder = model.ItemArticleRealizationOrder,
                                    code = model.ItemCode,
                                    name = model.ItemName,

                                },
                                quantity = model.Quantity + 1
                                
                            }
                        }
                    }
                }

            };
            Assert.True(detailViewModel3.Validate(validationContext).Count() > 0);

            ExpeditionViewModel detailViewModel4 = new ExpeditionViewModel
            {
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },
                items = new List<ExpeditionItemViewModel> {
                    new ExpeditionItemViewModel
                    {
                        spkDocsViewModel = new Lib.ViewModels.SpkDocsViewModel.SPKDocsViewModel
                        {
                            code = "test",
                            Weight = 5,
                            source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                            {
                                code = model.StorageCode,
                                name = model.StorageName,
                                _id = model.StorageId
                            }

                        },
                        details = new List<ExpeditionDetailViewModel>{
                            new ExpeditionDetailViewModel
                            {
                                sendQuantity = model.Quantity,
                                item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                                {
                                    articleRealizationOrder = model.ItemArticleRealizationOrder,
                                    code = model.ItemCode,
                                    name = model.ItemName,

                                },
                                quantity = model.Quantity,
                                remark = ""

                            }
                        }
                    }
                }

            };
            Assert.True(detailViewModel4.Validate(validationContext).Count() > 0);
        }
    }
}
