using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.InventoryLoaderInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.InventoriesControlles;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.SpkDocsControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.InventoryLoader
{
    public class InventoryLoaderTest
    {
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
        
        private InventoriesLoaderController GetController(Mock<IInventoryLoader> iInventoryLoader, Mock<IMapper> mapper)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var asst = new Mock<IdentityService>();

            InventoriesLoaderController controller = new InventoriesLoaderController(mapper.Object, (IdentityService)servicePMock.Object.GetService(typeof(IdentityService)), iInventoryLoader.Object )
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
        
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }
        
        [Fact]
        public async Task Should_Success_Get_Data()
        {
            var mockFacade = new Mock<IInventoryLoader>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Tuple.Create(new List<Inventory>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { });
            
            var controller = GetController(mockFacade, mockMapper);

            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }
        
        [Fact]
        public async Task Should_Error_Get_Data()
        {
            var mockFacade = new Mock<IInventoryLoader>();
            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<InventoryViewModel>>(It.IsAny<List<Inventory>>()))
                .Returns(new List<InventoryViewModel> { });
            
            var controller = GetController(mockFacade, mockMapper);

            var response = controller.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}