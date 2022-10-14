using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Com.MM.Service.Warehouse.WebApi.Controllers.v1.InventoryControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.InvetoriesTests
{
	public class InventoryReportControllerTest
	{
		private InventoriesReportViewModel ViewModel
		{
			get
			{
				return new InventoriesReportViewModel
				{
					StorageCode = "a",
					StorageId = 1

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

		protected InventoryReportController GetController(IdentityService identityService, IMapper mapper, InventoryFacade service)
		{
			var user = new Mock<ClaimsPrincipal>();
			var claims = new Claim[]
			{
				new Claim("username", "unittestusername")
			};
			user.Setup(u => u.Claims).Returns(claims);

			InventoryReportController controller = new InventoryReportController(mapper, service, identityService);
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

		public Inventory GetTestData(WarehouseDbContext dbContext)
		{
			Inventory data = new Inventory();
			data.ItemCode = "code";
			data.StorageId = 1;
			data.StorageName = "name";
			data.StorageCode = "code";
			data.ItemName = "name";
			data.StorageName = "name";
			data.ItemArticleRealizationOrder = "ro";
			data.ItemSize = "xl";
			data.Quantity = 1;
			dbContext.Inventories.Add(data);
			dbContext.SaveChanges();

			return data;
		}

		public InventoryMovement GetTestDataMovement(WarehouseDbContext dbContext)
		{
			InventoryMovement data = new InventoryMovement();
			data.ItemCode = "code";
			data.StorageId = 1;
			data.ItemName = "name";
			data.StorageName = "name";
			data.StorageCode = "code";
			dbContext.InventoryMovements.Add(data);
			dbContext.SaveChanges();

			return data;
		}

		public Expedition GetTestDataExpedition(WarehouseDbContext dbContext)
		{
			Expedition data = new Expedition()
			{
				Code = "code",
				Items = new List<ExpeditionItem>()
				{
					new ExpeditionItem()
					{
						DestinationCode = "code",
						Details = new List<ExpeditionDetail>()
						{
							new ExpeditionDetail()
							{
								ItemCode = "code",
								ItemName = "name",
								ArticleRealizationOrder = "ro",
								Size = "xl",
								SendQuantity = 1
							}
						}

					}
				}
			};

			dbContext.Expeditions.Add(data);
			dbContext.SaveChanges();

			return data;
		}

		protected int GetStatusCode(IActionResult response)
		{
			return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
		}

		Mock<IServiceProvider> GetServiceProvider()
		{
			var data = new List<SalesDocByRoViewModel>
			{
				new SalesDocByRoViewModel()
				{
					StoreCode = "code",
					StoreName = "name",
					StoreStorageCode = "code",
					StoreStorageName = "name",
					ItemCode = "code",
					ItemArticleRealizationOrder = "ro",
					size = "xl",
					quantityOnSales = 1,
				}
			};

			Dictionary<string, object> result =
				new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
				.Ok(data);

			Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			var httpClientService = new Mock<IHttpClientService>();
			httpClientService
			   .Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("sales-docs/readbyro"))))
			   .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result)) });

			var validateService = new Mock<IValidateService>();

			serviceProvider
			  .Setup(s => s.GetService(typeof(IValidateService)))
			  .Returns(validateService.Object);

			serviceProvider
			   .Setup(x => x.GetService(typeof(IHttpClientService)))
			   .Returns(httpClientService.Object);

			return serviceProvider;
		}

		Mock<IServiceProvider> GetServiceProviderReturnNull()
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

		#region by-search

		[Fact]
		public void Should_InternalServerError_Get_Data_BySearch()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_BySearch()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(testData.ItemCode, It.IsAny<int>(), It.IsAny<int>(), "{}");

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_BySearch_with_order()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);
			Dictionary<string, string> order = new Dictionary<string, string>()
			{
				{"ItemCode", "asc" }
			};

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(testData.ItemCode, It.IsAny<int>(), It.IsAny<int>(), JsonConvert.SerializeObject(order));

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_BySearch_GetXls()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReportXls(testData.ItemCode, It.IsAny<int>(), It.IsAny<string>());

			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}

		#endregion

		#region by-user
		[Fact]
		public void Should_InternalServerError_Get_Data_ByUser()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(testData.StorageId.ToString(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByUser()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(testData.StorageId.ToString(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), "{}");

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByUser_with_order()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);
			Dictionary<string, string> order = new Dictionary<string, string>()
			{
				{"ItemCode", "asc" }
			};

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetReport(testData.StorageId.ToString(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), JsonConvert.SerializeObject(order));

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByUser_GetXls()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetXls(testData.StorageId.ToString(), It.IsAny<string>());

			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}
		#endregion

		#region by-movement
		[Fact]
		public void Should_InternalServerError_Get_Data_ByMovement()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetMovements(testData.StorageId.ToString(), testData.ItemCode, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByMovement()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetMovements(testData.StorageId.ToString(), testData.ItemCode, It.IsAny<string>(), 1, 25, "{}");

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByMovement_with_order()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);
			Dictionary<string, string> order = new Dictionary<string, string>()
			{
				{"ItemCode", "asc" }
			};

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetMovements(testData.StorageId.ToString(), testData.ItemCode, It.IsAny<string>(), 1, 25, JsonConvert.SerializeObject(order));

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_ByMovement_GetXls()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetMovementXls(testData.StorageId.ToString(), testData.ItemCode);

			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}
		#endregion
		#region stockAvailability

		[Fact]
		public void Should_Success_GetAllStockByStorageId()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetAllStockByStorageId(testData.StorageId.ToString());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		#endregion
		#region InventoryAge

		[Fact]
		public void Should_Success_GetInventoryAge()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			Inventory testData = GetTestData(dbContext);
			long id = 1;
			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetInventoryAge((int)testData.StorageId, "");

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}
		[Fact]
		public void Should_Success_GetXlsAge()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetAgeXls((int)testData.StorageId, testData.ItemCode);

			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));

		}


		#endregion
		#region Monthly Stock
		[Fact]
		public void Should_InternalServerError_Get_Data_Monthly()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();
			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetOverallMonthlyStock(It.IsAny<string>(), It.IsAny<string>());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_Monthly()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetOverallMonthlyStock(testData.Date.Month.ToString(), testData.Date.Year.ToString());
			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_InternalServerError_Get_Data_Monthly_Storage()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();
			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetOverallStorageStock(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_Monthly_Storage()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();
			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetOverallStorageStock(testData.StorageCode, testData.Date.Month.ToString(), testData.Date.Year.ToString());

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]
		public void Should_Success_Get_Data_Monthly_GetXls()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			InventoryMovement testData = GetTestDataMovement(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GenerateOverallStorageStockExcel(testData.StorageCode, testData.Date.Month.ToString(), testData.Date.Year.ToString());
			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}
		#endregion
		#region by-ro

		[Fact]
		public void Should_Success_Get_Data_ByRo()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();


			var inventory = GetTestData(dbContext);
			var expedition = GetTestDataExpedition(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetInventoryStockByRo(inventory.ItemArticleRealizationOrder);

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.OK, statusCode);
		}

		[Fact]

		public void Should_Error_Get_Data_ByRo()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProviderReturnNull();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();


			var inventory = GetTestData(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetInventoryStockByRo(inventory.ItemArticleRealizationOrder);

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}

		[Fact]

		public void Should_Success_Get_Xls_ByRo()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProvider();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();


			var inventory = GetTestData(dbContext);
			var expedition = GetTestDataExpedition(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetXls(inventory.ItemArticleRealizationOrder);

			//Assert
			Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
		}

		[Fact]
		public void Should_Error_Get_Xls_ByRo()
		{
			//Setup
			WarehouseDbContext dbContext = _dbContext(GetCurrentAsyncMethod());
			Mock<IServiceProvider> serviceProvider = GetServiceProviderReturnNull();
			Mock<IMapper> imapper = new Mock<IMapper>();

			InventoryFacade service = new InventoryFacade(serviceProvider.Object, dbContext);

			serviceProvider.Setup(s => s.GetService(typeof(InventoryFacade))).Returns(service);
			serviceProvider.Setup(s => s.GetService(typeof(WarehouseDbContext))).Returns(dbContext);
			var identityService = new IdentityService();

			var inventory = GetTestData(dbContext);
			var expedition = GetTestDataExpedition(dbContext);

			//Act
			IActionResult response = GetController(identityService, imapper.Object, service).GetXls(inventory.ItemArticleRealizationOrder);

			//Assert
			int statusCode = this.GetStatusCode(response);
			Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
		}
		#endregion
	}
}
