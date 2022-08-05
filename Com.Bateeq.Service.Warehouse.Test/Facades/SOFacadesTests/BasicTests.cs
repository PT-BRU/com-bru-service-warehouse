using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SODataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel;
using Microsoft.Extensions.Primitives;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.SOFacadesTests
{
    public class BasicTests
    {
        private const string ENTITY = "MMPkpbjFacade";

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
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":1,\"code\":\"code\",\"name\":\"name\",\"address\":\"name\",\"city\":\"name\",\"date\":\"2018/10/20\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"}}}");
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
            message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":{\"Id\":1,\"code\":\"code\",\"name\":\"name\",\"address\":\"name\",\"city\":\"name\",\"date\":\"2018/10/20\"},\"info\":{\"count\":1,\"page\":1,\"size\":1,\"total\":2,\"order\":{\"date\":\"desc\"}}}");
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

        private SODataUtil dataUtil(SOFacade facade, string testName)
        {
            var soFacade = new SOFacade(GetServiceProvider().Object, _dbContext(testName));

            var inventoryFacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            var inventoryDataUtil = new InventoryDataUtil(inventoryFacade, _dbContext(testName));

            return new SODataUtil(facade, inventoryDataUtil);
        }

        [Fact]
        public async Task Should_Success_Process_Data()
        {

            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = await facade.Process(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Upload_Data()
        {

            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var Response = facade.UploadData(model, USERNAME);
            Assert.NotEqual(0,  Response.Id);
        }

        [Fact]
        public async Task Should_Success_Process_Data_IsAdjusted()
        {

            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataIsAdjusted();
            var Response = await facade.Process(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Process_Data_IsAdjusted_Qty()
        {

            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestDataIsAdjustedQty();
            var Response = await facade.Process(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Error_Process_Data()
        {

            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            model.StorageId = 0;
            Exception e = await Assert.ThrowsAsync<Exception>(async () => await facade.Process(model, USERNAME));
            Assert.NotNull(e.Message);
        }


        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_GetById()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Delete()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Delete((int)model.Id, USERNAME);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_GetTemplate()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.DownloadTemplate();
            Assert.NotNull(Response);
        }

        [Fact]
        public void Should_Success_Validate_Data_empty()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            SODocsCsvViewModel ViewModel = new SODocsCsvViewModel
            {
                
            };
            List<SODocsCsvViewModel> VmList = new List<SODocsCsvViewModel>() { ViewModel };
            var Response = facade.UploadValidate(ref VmList, It.IsAny<List<KeyValuePair<string, StringValues>>>(), It.IsAny<string>());

            Assert.True(Response.Item2.Count() > 0);
        }

        [Fact]
        public void Should_Success_Validate_Data_duplicate()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            SODocsCsvViewModel ViewModel = new SODocsCsvViewModel
            {
                name="n",
                code="c",
                quantity="a",
                
            };
            SODocsCsvViewModel ViewModel2 = new SODocsCsvViewModel
            {
                name = "n",
                code = "c",
                quantity = null,

            };
            List<SODocsCsvViewModel> VmList = new List<SODocsCsvViewModel>() { ViewModel, ViewModel2 };
            var Response = facade.UploadValidate(ref VmList, It.IsAny<List<KeyValuePair<string, StringValues>>>(), It.IsAny<string>());

            Assert.True(Response.Item2.Count() > 0);
        }

        [Fact]
        public async void Should_Success_Validate_Data_True()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            SODocsCsvViewModel ViewModel = new SODocsCsvViewModel
            {
                name = "n",
                code = model.Code,
                quantity = "2",
            };
            List<SODocsCsvViewModel> VmList = new List<SODocsCsvViewModel>() { ViewModel };
            var Response = facade.UploadValidate(ref VmList, It.IsAny<List<KeyValuePair<string, StringValues>>>(), It.IsAny<string>());

            Assert.True(Response.Item2.Count() == 0);
        }

        [Fact]
        public async Task Should_Success_MapToViewModel()
        {
            SOFacade facade = new SOFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            SODocsCsvViewModel ViewModel = new SODocsCsvViewModel
            {
                name = "n",
                code = model.Code,
                quantity = "2",
            };
            List<SODocsCsvViewModel> VmList = new List<SODocsCsvViewModel>() { ViewModel };
            var Response = facade.MapToViewModel(VmList, It.IsAny<string>());
            Assert.NotNull(Response);
        }
    }
}
