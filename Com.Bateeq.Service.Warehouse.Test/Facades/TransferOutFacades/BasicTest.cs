using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferOutDataUtils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using System.Linq;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using System.ComponentModel.DataAnnotations;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.TransferOutFacades
{
    public class BasicTest
    {
        private const string ENTITY = "MMTransferOut";

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

        private TransferOutDataUtil dataUtil(TransferOutFacade facade, string testName)
        {
            var transferOutFacade = new TransferOutFacade(ServiceProvider, _dbContext(testName));
            var inventoryFacade = new InventoryFacade(ServiceProvider, _dbContext(testName));
            var inventoryDataUtil = new InventoryDataUtil(inventoryFacade,_dbContext(testName));

            return new TransferOutDataUtil(facade, inventoryDataUtil);
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
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetNewData();
            var viewmodel = dataUtil(facade, GetCurrentMethod()).MapToViewModel(model);
            var Response = await facade.Create(viewmodel, model, USERNAME);
            Assert.NotEqual(0, Response);
        }
        [Fact]
        public async Task Should_Success_Get_All_Data()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_All_Data_For_Expedition()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadForRetur();
            Assert.NotEmpty(Response.Item1);
        }
        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadById((int)model.Id);
            Assert.NotNull(Response);
        }
        [Fact]
        public async Task Should_Success_Get_Excel_By_Id()
        {
            TransferOutFacade facade = new TransferOutFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.GenerateExcel((int)model.Id);
            Assert.IsType<MemoryStream>(Response);
        }
        [Fact]
        public async Task Should_Success_Validate_Data()
        {
            TransferOutDocViewModel ViewModel = new TransferOutDocViewModel
            {

            };
            Assert.True(ViewModel.Validate(null).Count() > 0);

            TransferOutDocViewModel ViewModeldest = new TransferOutDocViewModel
            {
                destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                }
            };
            Assert.True(ViewModel.Validate(null).Count() > 0);

            TransferOutDocViewModel ViewModelitem = new TransferOutDocViewModel
            {
                destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },

                source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                }
            };
            Assert.True(ViewModel.Validate(null).Count() > 0);

            DbSet<Inventory> dbSetInventory = _dbContext(GetCurrentMethod()).Set<Inventory>();
            InventoryFacade facade = new InventoryFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
            var model = await invendataUtil(facade, GetCurrentMethod(), _dbContext(GetCurrentMethod())).GetTestData();

            var mockServiceProvider = GetServiceProvider();
            mockServiceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext)))
                .Returns(_dbContext(GetCurrentMethod()));

            var validationContext = new ValidationContext(ViewModelitem, mockServiceProvider.Object, null);

            TransferOutDocViewModel ViewModelitemquantity = new TransferOutDocViewModel
            {
                destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },

                source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                {
                    code = model.StorageCode,
                    name = model.StorageName,
                    _id = model.StorageId
                },
                items = new List<TransferOutDocItemViewModel>
                {
                    new TransferOutDocItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = model.ItemId,
                            
                        },
                        quantity = model.Quantity + 1,
                        remark = "remark"
                    }
                }
                
            };
            Assert.True(ViewModelitemquantity.Validate(validationContext).Count() > 0);

            TransferOutDocViewModel ViewModelitemremark = new TransferOutDocViewModel
            {
                destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                {
                    code = "code",
                    name = "name",
                    _id = 1
                },

                source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                {
                    code = model.StorageCode,
                    name = model.StorageName,
                    _id = model.StorageId
                },
                items = new List<TransferOutDocItemViewModel>
                {
                    new TransferOutDocItemViewModel
                    {
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            _id = model.ItemId,

                        },
                        quantity = model.Quantity,
                        remark = "",
                    }
                   
                    
                }

            };
            Assert.True(ViewModelitemquantity.Validate(validationContext).Count() > 0);
        }
    }
}
