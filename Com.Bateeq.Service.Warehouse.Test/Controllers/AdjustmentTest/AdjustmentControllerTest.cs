using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.AdjustmentInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Adjustment;
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


namespace Com.Bateeq.Service.Warehouse.Test.Controllers.AdjustmentTest
{
    public class AdjustmentControllerTest
    {
        private AdjustmentDocsViewModel adjustmentVM
        {
            get
            {
                return new AdjustmentDocsViewModel
                {
                    Id = 1,
                    code = "code",
                    storage = new Lib.ViewModels.NewIntegrationViewModel.StorageViewModel
                    {
                        _id = 1,
                        code = "code",
                        name = "name"
                    },
                    items = new List<AdjustmentDocsItemViewModel>
                    {
                        new AdjustmentDocsItemViewModel
                        {
                            Id = 1
                        }   
                    }
                };
            }
        }

        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(adjustmentVM, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
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

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        protected string GetCurrentAsyncMethod([CallerMemberName] string methodName = "")
        {
            var method = new StackTrace()
                .GetFrames()
                .Select(frame => frame.GetMethod())
                .FirstOrDefault(item => item.Name == methodName);

            return method.Name;
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            var validateService = new Mock<IValidateService>();
            //validateService.Setup(s => s.Validate(It.IsAny<ExpeditionViewModel>())).Verifiable();

            serviceProvider
              .Setup(s => s.GetService(typeof(IValidateService)))
              .Returns(validateService.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            serviceProvider
                .Setup(x => x.GetService(typeof(WarehouseDbContext)))
                .Returns(_dbContext(GetCurrentAsyncMethod()));

            return serviceProvider;
        }

        private AdjustmentController GetController(Mock<IAdjustmentDoc> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapperM)
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

            AdjustmentController controller = new AdjustmentController(servicePMock.Object, mapperM.Object, facadeM.Object)
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

        [Fact]
        public void Should_Success_Get_All_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IAdjustmentDoc>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<AdjustmentDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<AdjustmentDocsViewModel>>(It.IsAny<List<AdjustmentDocs>>()))
                .Returns(new List<AdjustmentDocsViewModel> { });

            IActionResult response = GetController(mockFacade, mockValidate, mockMapper).Get();

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IAdjustmentDoc>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<AdjustmentDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();

            IActionResult response = GetController(mockFacade, mockValidate, mockMapper).Get();

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IAdjustmentDoc>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new AdjustmentDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<AdjustmentDocsViewModel>(It.IsAny<AdjustmentDocs>()))
                .Returns(new AdjustmentDocsViewModel());

            IActionResult response = GetController(mockFacade, mockValidate, mockMapper).Get(It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IAdjustmentDoc>();
            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new AdjustmentDocs());

            var mockMapper = new Mock<IMapper>();

            IActionResult response = GetController(mockFacade, mockValidate, mockMapper).Get(It.IsAny<int>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<AdjustmentDocs>(It.IsAny<AdjustmentDocsViewModel>()))
                .Returns(new AdjustmentDocs());

            var mockFacade = new Mock<IAdjustmentDoc>();
            mockFacade.Setup(x => x.Create(It.IsAny<AdjustmentDocs>(), "unittestusername", 7))
                .ReturnsAsync(1);

            var controller = GetController(mockFacade, mockValidate, mockMapper);

            var response = await controller.Post(adjustmentVM);

            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IAdjustmentDoc>();

            var controller = GetController(mockFacade, mockValidate, mockMapper);

            var response = await controller.Post(new AdjustmentDocsViewModel());

            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var mockValidate = new Mock<IValidateService>();
            //mockValidate.Setup(s => s.Validate(It.IsAny<AdjustmentDocsViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IAdjustmentDoc>();
            //mockFacade.Setup(x => x.Create(It.IsAny<AdjustmentDocs>(), "unittestusername", 7))
            //    .ReturnsAsync(0);

            var controller = GetController(mockFacade, mockValidate, mockMapper);

            var response = await controller.Post(new AdjustmentDocsViewModel());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


    }
}
