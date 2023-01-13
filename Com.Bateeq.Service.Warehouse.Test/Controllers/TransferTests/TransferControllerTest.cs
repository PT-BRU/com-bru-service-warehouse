using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Transfer;
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
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.TransferTests
{
    public class TransferControllerTest
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
        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
        private TransferController GetController(Mock<ITransferInDoc> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();
            if (validateM != null)
            {
                servicePMock
                    .Setup(x => x.GetService(typeof(IValidateService)))
                    .Returns(validateM.Object);
            }

            TransferController controller = new TransferController(servicePMock.Object, mapper.Object, facadeM.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }
        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferInDoc>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferInDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferInDocViewModel>>(It.IsAny<List<TransferInDoc>>()))
                .Returns(new List<TransferInDocViewModel> { ViewModel });

            TransferController controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferInDoc>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferInDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferInDocViewModel>>(It.IsAny<List<TransferInDoc>>()))
                .Returns(new List<TransferInDocViewModel> { ViewModel });

            TransferController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_Retur_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferInDoc>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferInDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferInDocViewModel>>(It.IsAny<List<TransferInDoc>>()))
                .Returns(new List<TransferInDocViewModel> { ViewModel });

            TransferController controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetRetur();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_All_Data_Retur_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferInDoc>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferInDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferInDocViewModel>>(It.IsAny<List<TransferInDoc>>()))
                .Returns(new List<TransferInDocViewModel> { ViewModel });

            TransferController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetRetur();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferInDoc>(It.IsAny<TransferInDocViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<ITransferInDoc>();
            mockFacade.Setup(x => x.Create(It.IsAny<TransferInDoc>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<ITransferInDoc>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }
        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferInDocViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferInDoc>(It.IsAny<TransferInDocViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<ITransferInDoc>();
            mockFacade.Setup(x => x.Create(It.IsAny<TransferInDoc>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<ITransferInDoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferInDoc());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferInDocViewModel>(It.IsAny<TransferInDoc>()))
                .Returns(ViewModel);

            TransferController controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<ITransferInDoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferInDoc());

            var mockMapper = new Mock<IMapper>();

            TransferController controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_error_Get_Data_GetXls()
        {
            var mockFacade = new Mock<ITransferInDoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferInDoc());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferInDocViewModel>(It.IsAny<TransferInDoc>()))
                .Returns(ViewModel);

            TransferController controller = new TransferController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object);
            var response = controller.GetXls(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
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
            data.Reference = "ref";
            data.CreatedBy = "unittestusername";
            data.Id = 1;
            data.SourceCode = "GDG.01";
            data.DestinationCode = "code";
            data.SourceName = "GDG.01";
            data.DestinationName = "code";
            
            return data;
        }
        protected TransferController GetController(IdentityService identityService, IMapper mapper, TransferFacade service)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            TransferController controller = new TransferController(GetServiceProvider().Object,mapper,service);
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

        [Fact]
        public void Should_Success_Get_Data_GetXls()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferFacade service = new TransferFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(SPKDocsFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
            var identityService = new IdentityService();

            TransferInDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(identityService, imapper.Object, service).GetXls(It.IsAny<int>());

            //Assert
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

    }
}
