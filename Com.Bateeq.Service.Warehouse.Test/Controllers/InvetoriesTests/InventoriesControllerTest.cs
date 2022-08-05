using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.InventoriesControlles;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.InvetoriesTests
{
    public class InventoriesControllerTest
    {
        private InventoryViewModel ViewModel
        {
            get
            {
                return new InventoryViewModel
                {
                    item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                    {
                        _id = 1
                    },
                    itemInternationalCOGS = 0,
                    itemInternationalRetail = 0,
                    itemInternationalSale = 0,
                    itemInternationalWholeSale = 0,
                    quantity = 1,
                    storage = new Lib.ViewModels.NewIntegrationViewModel.StorageViewModel
                    {
                        code = "Codedest",
                        name = "namecodetest",
                        _id = 1
                    },

                };
                
            }
        }

        private Inventory Model
        {
            get
            {
                return new Inventory
                {
                    ItemId = 1

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

        private InventoriesController GetController(Mock<InventoryFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            InventoriesController controller = new InventoriesController(mapper.Object, facadeM.Object, servicePMock.Object)
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
        public void Should_Error_Get_Code()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.GetItemPack(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<InventoryViewModel>(), 0));

            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
            //    .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            InventoriesController controller = new InventoriesController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetCode(It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Code()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.GetItemPack(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Tuple.Create(new List<InventoryViewModel>(), 0));

            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
            //    .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            InventoriesController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetCode(It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_by_Code()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getDatabyCode(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<Inventory> { Model });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = new InventoriesController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetName(It.IsAny<string>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_by_Code()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getDatabyCode(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<Inventory> { Model });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetName(It.IsAny<string>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_by_Name()
        {
            var validateMock = new Mock<IValidateService>();

            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getDatabyName(It.IsAny<string>(),It.IsAny<int>()))
                .Returns(new List<Inventory> { Model });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = new InventoriesController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetbyCode(It.IsAny<string>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_By_Name()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getDatabyName(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<Inventory> { Model });

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetbyCode(It.IsAny<string>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        [Fact]
        public void Should_Error_Get_Stock()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getStock(It.IsAny<int>(),It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = new InventoriesController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetStock(It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
        [Fact]
        public void Should_Success_Get_Stock()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<InventoryViewModel>())).Verifiable();

            var mockFacade = new Mock<InventoryFacade>();


            mockFacade.Setup(x => x.getStock(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Model);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { ViewModel });

            InventoriesController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetStock(It.IsAny<int>(), It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

    }
}
