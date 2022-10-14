using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.ReturnToCenterInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.TransferStocksInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Stores.ReturnToCenterController;
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
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.Store.ReturnToCenterTests
{
	public class ReturnToCenterControllerTest
	{
		private TransferOutDocViewModel ViewModel
		{
			get
			{
				return new TransferOutDocViewModel
				{
					code = "code",
					reference = ""
				};
			}
		}
		private TransferOutReadViewModel readViewModel
		{
			get
			{
				return new TransferOutReadViewModel
				{
					code = "code"
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

		private ReturnToCenterController GetController(IServiceProvider serviceProvider, IMapper mapper, ReturnToCenterFacade service)
		{
			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
						new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);

			var servicePMock = GetServiceProvider();
		 
			ReturnToCenterController controller = new ReturnToCenterController(servicePMock.Object, mapper,service)
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
			TransferOutDoc data = new TransferOutDoc();
			dbContext.TransferOutDocs.Add(data);
			dbContext.SaveChanges();
			return data;

		}
		private TransferOutDoc Model
		{
			get
			{
				return new TransferOutDoc
				{
					Code = "Code",
					Items = new List<TransferOutDocItem>
					{
						new TransferOutDocItem
						{
							ArticleRealizationOrder = "RO"
						}
					}
				};
			}
		}
		[Fact]
		public void Should_Error_Get()
		{
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			ReturnToCenterFacade service = new ReturnToCenterFacade(serviceProvider.Object, dbContext);
			serviceProvider.Setup(s => s.GetService(typeof(ReturnToCenterFacade))).Returns(service);
			Mock<IMapper> imapper = new Mock<IMapper>();

			//Act
			IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Get(1);

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Error_GetById()
		{
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			ReturnToCenterFacade service = new ReturnToCenterFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(ReturnToCenterFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

			TransferOutDoc testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Get(1);

			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		}
		[Fact]
		public void Should_POST_OK()
		{
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<TransferOutDoc>(ViewModel))
				.Returns(Model);

			ReturnToCenterFacade service = new ReturnToCenterFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(ReturnToCenterFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);

			TransferOutDoc testData = GetTestData(dbContext);
			//Act
			IActionResult response = GetController(serviceProvider.Object, mockMapper.Object, service).Post(ViewModel).Result;

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.NotEqual((int)HttpStatusCode.NotFound, statusCode);

		}
		[Fact]
		public void POST_InternalServerError()
		{
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			ReturnToCenterFacade service = new ReturnToCenterFacade(serviceProvider.Object, dbContext);

			//Act
			IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).Post(ViewModel).Result;

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void getExcel()
		{
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			ReturnToCenterFacade service = new ReturnToCenterFacade(serviceProvider.Object, dbContext);

			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);
			 

			IActionResult response = GetController(serviceProvider.Object, imapper.Object, service).GetXls(It.IsAny<int>());
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}
		 
	}
}
