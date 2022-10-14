using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.TransferStockControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Xunit;
using System.Net;
using Newtonsoft.Json;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.Store.TransferStockControllerTest
{
    public class TransferStockReportControllerTest
    {
        protected TransferStockReportController GetController(IdentityService identityService, IMapper mapper, TransferStockFacade service)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            TransferStockReportController controller = new TransferStockReportController(mapper, service, identityService);
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

        public TransferOutDoc GetTestData(WarehouseDbContext dbContext)
        {
            SPKDocs data = new SPKDocs();
            data.Reference = "BTQ-KB/RTT";
            data.CreatedBy = "unittestusername";
            data.Id = 1;
            data.SourceCode = "GDG.01";
            data.DestinationCode = "code";
            dbContext.SPKDocs.Add(data);

            TransferOutDoc tf = new TransferOutDoc();
            tf.Code = data.Reference;

            dbContext.TransferOutDocs.Add(tf);
            dbContext.SaveChanges();

            return tf;
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
        public void Should_InternalServerError_Get_Data()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferStockFacade service = new TransferStockFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferStockFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
            var identityService = new IdentityService();

            TransferOutDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(identityService, imapper.Object, service).GetReport(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Should_Success_Get_Data()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferStockFacade service = new TransferStockFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferStockFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
            var identityService = new IdentityService();

            TransferOutDoc testData = GetTestData(dbContext);

            //Act
            IActionResult response = GetController(identityService, imapper.Object, service).GetReport(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2), "Semua", testData.Code, 1, 25, "{}");

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Should_Success_Get_Data_withOrder()
        {
            //Setup
            WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
            Mock<IServiceProvider> serviceProvider = GetServiceProvider();
            Mock<IMapper> imapper = new Mock<IMapper>();

            TransferStockFacade service = new TransferStockFacade(serviceProvider.Object, dbContext);

            serviceProvider.Setup(s => s.GetService(typeof(TransferStockFacade))).Returns(service);
            serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
            var identityService = new IdentityService();

            TransferOutDoc testData = GetTestData(dbContext);
            Dictionary<string, string> order = new Dictionary<string, string>()
            {
                {"DestinationCode", "asc" }
            };

            //Act
            IActionResult response = GetController(identityService, imapper.Object, service).GetReport(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2), "Semua", testData.Code, 1, 25, JsonConvert.SerializeObject(order));

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

    }
}
