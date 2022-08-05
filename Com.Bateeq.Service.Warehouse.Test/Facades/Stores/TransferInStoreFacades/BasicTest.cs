using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.ExpeditionDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
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
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.Stores.TransferInStoreFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMTransferInStoreFacade";

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

        private TransferInStoreDataUtil dataUtilTransfer(TransferFacade facade, string testName)
        {
            var pkbbjfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(ServiceProvider, _dbContext(testName));
            var expeditionfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.ExpeditionFacade(ServiceProvider, _dbContext(testName));
            var inventoryfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.InventoryFacade(ServiceProvider, _dbContext(testName));

            var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
            var inventoryDataUtil = new InventoryDataUtil(inventoryfacade, _dbContext(testName));
            var expeditionDataUtil = new ExpeditionDataUtil(expeditionfacade, inventoryDataUtil, sPKDocDataUtil);

            var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
            //var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);
            var tranferInStore = new TransferInStoreDataUtil(transferFacade, expeditionDataUtil);

            return new TransferInStoreDataUtil(facade, expeditionDataUtil);
        }

        private TransferInDoc TransInModel
        {
            get
            {
                return new TransferInDoc
                {
                    Code = "code",
                    Date = DateTimeOffset.Now,
                    DestinationId = 1,
                    DestinationCode = "code",
                    DestinationName = "name",
                    Reference = "GDG.",
                    SourceId = 1,
                    SourceCode = "1",
                    SourceName = "source",
                    
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
                    IsDistributed = true,
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
        public async Task Should_Success_Create_Data()
        {

            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferInStoreFacade facadestore = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtilTransfer(facade, GetCurrentMethod()).GetNewData();
            var Response = await facadestore.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }

        [Fact]
        public async Task Should_Success_Create_Data_2()
        {
            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            Inventory inventory = new Inventory
            {
                ItemArticleRealizationOrder = "art1",
                ItemCode = "code",
                ItemDomesticCOGS = 0,
                ItemDomesticRetail = 0,
                ItemDomesticSale = 0,
                ItemDomesticWholeSale = 0,
                ItemId = 1,
                ItemInternationalCOGS = 0,
                ItemInternationalRetail = 0,
                ItemInternationalSale = 0,
                ItemInternationalWholeSale = 0,
                ItemName = "name",
                ItemSize = "size",
                Quantity = 1,
                ItemUom = "uom",
                StorageCode = "code",
                StorageId = 1,
                StorageIsCentral = false,
                StorageName = "name",

            };
            dbSetInventory.Add(inventory);
            var Created = _dbContext(GetCurrentMethod()).SaveChangesAsync();

            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferInStoreFacade facadestore = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtilTransfer(facade, GetCurrentMethod()).GetNewData();
            model.DestinationId = inventory.StorageId;
            model.DestinationName = inventory.StorageName;
            model.DestinationCode = inventory.StorageCode;
            foreach (var item in model.Items)
            {
                item.ItemId = inventory.ItemId;
            }
            var Response = await facadestore.Create(model, USERNAME);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            DbSet<TransferInDoc> dbSet = _dbContext(GetCurrentMethod()).Set<TransferInDoc>();

            dbSet.Add(this.TransInModel);
            await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            TransferInStoreFacade facade = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Response = facade.Read();
            Assert.NotEqual(null, Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            TransferFacade facade = new TransferFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            TransferInStoreFacade facadestore = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtilTransfer(facade, GetCurrentMethod()).GetNewData();
            await facadestore.Create(model, USERNAME);
            var Response = facadestore.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_All_Data_Pending()
        {
            DbSet<SPKDocs> dbSetSpk = _dbContext(GetCurrentMethod()).Set<SPKDocs>();

            dbSetSpk.Add(spkDocsModel);
            await _dbContext(GetCurrentMethod()).SaveChangesAsync();
            TransferInStoreFacade facade = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

            var Response = facade.ReadPending();
            Assert.NotEqual(null, Response);
        }

		[Fact]
		public async Task Should_Success_Get_All_Data_PendingStore()
		{

			DbSet<SPKDocs> dbSetSpk = _dbContext(GetCurrentMethod()).Set<SPKDocs>();

			dbSetSpk.Add(spkDocsModel);
			await _dbContext(GetCurrentMethod()).SaveChangesAsync();
			TransferInStoreFacade facade = new TransferInStoreFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));

			var Response = facade.ReadPendingStore("code");
			Assert.NotEqual(null, Response);

		}
	}
}
