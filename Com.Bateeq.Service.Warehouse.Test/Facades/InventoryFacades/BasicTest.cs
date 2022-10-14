using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.InventoryFacades
{
    public class BasicTest
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
            var HttpClientService = new Mock<IHttpClientService>();
            HttpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(message);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(Lib.Interfaces.IHttpClientService)))
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

        private InventoryDataUtil dataUtil(InventoryFacade facade, string testName, WarehouseDbContext dbContext)
        {
            var pkbbjfacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
         
            //var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
            //var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

            return new InventoryDataUtil(facade, dbContext);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
            //dbSetInventory.Add(model);
            //var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            //InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GetItemPack(model.ItemCode, model.StorageCode,"{}",1,25);
            Assert.NotNull(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_Code()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
            //dbSetInventory.Add(model);
            //var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            //InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.getDatabyCode(model.ItemCode, (int)model.StorageId);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_Name()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
            //dbSetInventory.Add(model);
            //var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            //InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.getDatabyName(model.ItemName, (int)model.StorageId);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Stock()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
            //dbSetInventory.Add(model);
            //var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            //InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            //var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.getStock((int)model.StorageId, (int)model.ItemId);
            Assert.NotNull(Response);
        }
		[Fact]
		public async Task Should_Success_Get_Report()
		{
			DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
			InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
			//dbSetInventory.Add(model);
			//var Created = await _dbContext(GetCurrentMethod()).SaveChangesAsync();
			//InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			//var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var Response = facade.GetAllStockByStorageId( model.StorageId.ToString());
			Assert.NotNull(Response);
		}

		[Fact]
		public async Task Should_Success_Get_ReportAge()
		{
			DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
			InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
			
			var Response = facade.GetInventoryAge((int)model.StorageId,model.ItemName);
			Assert.NotNull(Response);
		}

		[Fact]
		public async Task Should_Success_Get_XlsAge()
		{
			DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
			DbSet<InventoryMovement> dbSetInventoryMove = _dbContext(GetCurrentMethod()).Set<InventoryMovement>();
			InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model1 = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();
			var model = await dataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetInventoryAgeData();
		    var Response = facade.GenerateExcelInventoryAge((int)model.StorageId, model.ItemName);
			Assert.NotNull(Response);
		}
	}
}
