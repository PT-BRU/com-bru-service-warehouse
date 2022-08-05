using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Facades.AdjustmentFacade;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.AdjustmentDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.NewIntegrationDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.AdjustmentFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMAdjustmentFacade";

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
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Id\":1,\"Code\":\"storagecode\",\"Name\":\"storagename\"}],\"info\":{\"total\":2}}}");
            HttpResponseMessage messagePost = new HttpResponseMessage();

            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);
            HttpClientService
               .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("master/storages/code"))))
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new StorageDataUtil().GetResultFormatterOkString()) });
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

        private AdjustmentDataUtil dataUtil(AdjustmentFacade facade, string testName)
        {
            var pkbbjfacade = new AdjustmentFacade(ServiceProvider, _dbContext(testName));
            var inventoryFacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            var inventoryDataUtil = new InventoryDataUtil(inventoryFacade, _dbContext(testName));
            //var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
            //var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
            //var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

            return new AdjustmentDataUtil(facade, inventoryDataUtil);
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

            AdjustmentFacade facade = new AdjustmentFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewDataAsync();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data2()
        {

            AdjustmentFacade facade = new AdjustmentFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewDataAsync2();
            var Response = await facade.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {

            AdjustmentFacade facade = new AdjustmentFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = new AdjustmentDocs();
                //await dataUtil(facade, GetCurrentMethod()).GetNewDataAsync();

            //var Response = await facade.Create(model, USERNAME);

            await Assert.ThrowsAnyAsync<Exception>(() => facade.Create(model, USERNAME));
            //Assert.Equal(0, Response);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            AdjustmentFacade facade = new AdjustmentFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            AdjustmentFacade facade = new AdjustmentFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Validate_DataAsync()
        {
            var ViewModel = new AdjustmentDocsViewModel
            {
                items = new List<AdjustmentDocsItemViewModel>
                {

                }
            };

            Assert.True(ViewModel.Validate(null).Count() > 0);

            var itemViewModel = new AdjustmentDocsViewModel
            {
                storage = new Lib.ViewModels.NewIntegrationViewModel.StorageViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 0
                },
                items = new List<AdjustmentDocsItemViewModel> {
                    new AdjustmentDocsItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = 0
                        },
                        type = "OUT",
                        qtyAdjustment = 5,
                        qtyBeforeAdjustment = 4,
                        remark = null
                    },
                    new AdjustmentDocsItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = 0
                        },
                        type = "OUT",
                        qtyAdjustment = 0,
                        qtyBeforeAdjustment = 4,
                        remark = null
                    }
                }
            };

            Assert.True(itemViewModel.Validate(null).Count() > 0);

            var itemViewModel1 = new AdjustmentDocsViewModel
            {
                storage = new Lib.ViewModels.NewIntegrationViewModel.StorageViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 0
                },
                items = new List<AdjustmentDocsItemViewModel> {
                    new AdjustmentDocsItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = 1
                        },
                        type = "OUT",
                        qtyAdjustment = 5,
                        qtyBeforeAdjustment = 4,
                        remark = null
                    },
                    new AdjustmentDocsItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = 1
                        },
                        type = "OUT",
                        qtyAdjustment = 0,
                        qtyBeforeAdjustment = 4,
                        remark = null
                    }
                }
            };

            Assert.True(itemViewModel1.Validate(null).Count() > 0);

        }



    }
}
