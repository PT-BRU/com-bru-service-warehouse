using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferOutDataUtils;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.Stores.TransferStockFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMTransferStock";

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
            var data = new StorageViewModel2
            {
                Code = "GTM.01",
                Id = 8,
                IsCentral = true,
                Name = "GUDANG TRANSFER STOCK MAJOR MINOR"
            };
            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);
            var exp = new ExpeditionServiceViewModel
            {
                code = "Dikirim Sendiri",
                name = "Dikirim Sendiri",
                _id = 2
            };
            Dictionary<string, object> result2 =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(exp);
            //HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            //message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{\"Code\":MMS.01,\"Name\":\"MAJOR MINOR SHOP/ONLINE\",\"Description\":null,\"_id\":\"1\"}],\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"},\"select\":[\"Id\",\"code\",\"rate\",\"date\"]}}");
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("storages/code"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result) )});
            HttpClientService
                .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("expedition-service-routers/all/code"))))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result2)) });


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

        private TransferOutDataUtil dataUtil(TransferOutFacade facade, string testName)
        {
            var transferOutFacade = new TransferOutFacade(ServiceProvider, _dbContext(testName));
            var inventoryFacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            var inventoryDataUtil = new InventoryDataUtil(inventoryFacade, _dbContext(testName));

            return new TransferOutDataUtil(facade, inventoryDataUtil);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferStockFacade returnToCenterFacade = new TransferStockFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
            var Response = await returnToCenterFacade.Create(viewmodel, model, USERNAME);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Create_Data_GTM_AlredyExist()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferStockFacade returnToCenterFacade = new TransferStockFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewDataForTransfer();
            var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
            var Response = await returnToCenterFacade.Create(viewmodel, model, USERNAME);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferStockFacade returnToCenterFacade = new TransferStockFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
            await returnToCenterFacade.Create(viewmodel, model, USERNAME);
            var Response = returnToCenterFacade.Read();
            Assert.NotEmpty(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferStockFacade returnToCenterFacade = new TransferStockFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
            await returnToCenterFacade.Create(viewmodel, model, USERNAME);
            var Response = returnToCenterFacade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }
		[Fact]
		public async Task Should_Success_ReadModel()
		{
			TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			TransferStockFacade returnToCenterFacade = new TransferStockFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
			var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
			await returnToCenterFacade.Create(viewmodel, model, USERNAME);
			var Response = returnToCenterFacade.ReadModel((int)model.Id);
			Assert.NotNull(Response);
		}

	}
}
