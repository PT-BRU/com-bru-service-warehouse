using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Stores.TransferIn;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.Store.TransferInStoreTests
{
    public class TransferInStoreControllerTest
    {
        private TransferInDocViewModel ViewModel
        {
            get
            {
                return new TransferInDocViewModel
                {
                    code = "Code",
                    destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                    {
                        code = "code",
                        name = "name",
                        _id = 1,

                    },
                    reference = "reference",
                    items = new List<TransferInDocItemViewModel>
                    {
                        new TransferInDocItemViewModel
                        {
                            sendquantity = 0,
                            item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                            {
                                code = "code",
                                domesticCOGS = 0,
                            },
                            remark = "remark"
                        }
                    }
                };
            }
        }
        private TransferInDoc Model
        {
            get
            {
                return new TransferInDoc
                {
                    Code = "Code",
                    Items = new List<TransferInDocItem>
                    {
                        new TransferInDocItem
                        {
                            ArticleRealizationOrder = "RO"
                        }
                    }
                };
            }
        }

        private SPKDocsViewModel SpkViewModel
        {
            get
            {
                return new SPKDocsViewModel
                {
                    code = "Code",
                    date = DateTimeOffset.Now,
                    destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                    {
                        code = "Codedest",
                        name = "namecodetest",
                        _id = 1
                    },
                    isDistributed = false,
                    isDraft = false,
                    isReceived = false,
                    packingList = "packinglist",
                    password = "pass",
                    reference = "ref",
                    source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                    {
                        code = "code",
                        name = "name",
                        _id = 1
                    },
                    items = new List<SPKDocsItemViewModel>
                    {
                        new SPKDocsItemViewModel
                        {
                            item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                            {
                                _id = 1
                            },

                        }
                    }
                };
            }
        }

        protected TransferInController GetController(IServiceProvider serviceProvider, IMapper mapper,TransferInStoreFacade service)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            TransferInController controller = new TransferInController(serviceProvider, mapper,service);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        private WarehouseDbContext _dbContext(string testName)
        {
            var serviceProvider = new ServiceCollection()
              .AddEntityFrameworkInMemoryDatabase()
              .BuildServiceProvider();

            DbContextOptionsBuilder<WarehouseDbContext> optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseInternalServiceProvider(serviceProvider);

            WarehouseDbContext dbContext = new WarehouseDbContext(optionsBuilder.Options);

            return dbContext;
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;

        }

        public TransferInDoc GetTestData(WarehouseDbContext dbContext)
        {
            TransferInDoc data = new TransferInDoc();
            dbContext.TransferInDocs.Add(data);
            dbContext.SaveChanges();

            return data;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }


        Mock<IServiceProvider> GetServiceProvider()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            var validateService = new Mock<IValidateService>();
            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);
            return serviceProvider;
        }

        [Fact]
        public void Get_Return_OK()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

            TransferInDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void Get_InternalServerError()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);
            serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
            Mock<IMapper> imapper = new Mock<IMapper>();

            //Act
            IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Get();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetById_Return_OK()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();
            imapper.Setup(x => x.Map<TransferInDocViewModel>(Model))
                .Returns(ViewModel);

            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

            TransferInDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Get(Convert.ToInt32( testData.Id));

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void POST_Return_OK()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferInDoc>(ViewModel))
                .Returns(Model);

            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

            TransferInDoc testData = GetTestData(dbContext);
            //Act
            IActionResult response = GetController(serviceProvider.Object, mockMapper.Object, service).Post(ViewModel).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public void POST_InternalServerError()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

            //Act
            IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Post(ViewModel).Result;

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public async void GetPending_Return_OK()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());

            DbSet<SPKDocs> dbSetSpk = _dbContext(GetCurrentAsyncMethod()).Set<SPKDocs>();

            dbSetSpk.Add(new SPKDocs());
            await _dbContext(GetCurrentAsyncMethod()).SaveChangesAsync();
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();
            imapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(It.IsAny<List<SPKDocsViewModel>>());

            TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

            TransferInDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).GetPending();

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        }

        
        //[Fact]
        //public void POST_BadRequest()
        //{
        //    //Setup
        //    WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
        //    Mock<IServiceProvider> serviceProvider = GetServiceProvider();

        //    Mock<IMapper> imapper = new Mock<IMapper>();
        //    imapper.Setup(x => x.Map<TransferInDoc>(It.IsAny<TransferInDocViewModel>()))
        //        .Returns(Model);

        //    TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

        //    serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
        //    serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

        //    var validateServiceMock = new Mock<IValidateService>();
        //    validateServiceMock.Setup(v => v.Validate(It.IsAny<TransferInDoc>())).Verifiable();


        //    serviceProvider.Setup(sp => sp.GetService(typeof(IValidateService))).Returns(validateServiceMock.Object);
        //    //Act
        //    IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Post(ViewModel).Result;

        //    //Assert
        //    int statusCode = this.GetStatusCode(response);
        //    Assert.Equal((int)HttpStatusCode.BadRequest, statusCode);
        //}

        //[Fact]
        //public void Delete_Success()
        //{
        //    //Setup
        //    WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
        //    Mock<IServiceProvider> serviceProvider = GetServiceProvider();
        //    Mock<IMapper> imapper = new Mock<IMapper>();

        //    TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object, dbContext);

        //    serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
        //    serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

        //    TransferInDoc testData = GetTestData(dbContext);
        //    //Act
        //    IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Delete(testData.Id).Result;

        //    //Assert
        //    int statusCode = this.GetStatusCode(response);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        //}


        //[Fact]
        //public void PUT_Return_OK()
        //{
        //    //Setup
        //    CoreDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
        //    Mock<IServiceProvider> serviceProvider = GetServiceProvider();

        //    TransferInStoreFacade service = new TransferInStoreFacade(serviceProvider.Object);

        //    serviceProvider.Setup(s => s.GetService(typeof(TransferInStoreFacade))).Returns(service);
        //    serviceProvider.Setup(s => s.GetService(typeof(CoreDbContext))).Returns(dbContext);

        //    TransferInDoc testData = GetTestData(dbContext);
        //    var dataVM = service.MapToViewModel(testData);

        //    //Act
        //    IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Put(testData.Id, dataVM).Result;

        //    //Assert
        //    int statusCode = this.GetStatusCode(response);
        //    Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);
        //}
    }
}
