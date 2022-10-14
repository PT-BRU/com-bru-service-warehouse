using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.NewIntegrationDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils.SPKDocDataUtil;

namespace Com.Bateeq.Service.Warehouse.Test.Facades.PkpbjFacades
{
	public class UploadTest
	{
		private const string ENTITY = "MMPkpbjFacade";

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
			HttpResponseMessage messagePost = new HttpResponseMessage();
			var HttpClientService = new Mock<IHttpClientService>();
			HttpClientService
				.Setup(x => x.GetAsync(It.IsAny<string>()))
				.ReturnsAsync(message);
			HttpClientService
				.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains("items/finished-goods/Code"))))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new ItemDataUtil().GetResultFormatterOkString()) });
			HttpClientService
				.Setup(x => x.PostAsync(It.Is<string>(s => s.Contains("items/finished-goods")), It.IsAny<HttpContent>()))
				.ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new ItemDataUtil().GetResultFormatterOkString()) });
			HttpClientService
				.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
				.ReturnsAsync(messagePost);
			var serviceProvider = new Mock<IServiceProvider>();
			serviceProvider
				.Setup(x => x.GetService(typeof(IdentityService)))
				.Returns(new IdentityService() { Token = "Token", Username = "Test" });

			serviceProvider
				.Setup(x => x.GetService(typeof(IHttpClientService)))
				.Returns(HttpClientService.Object);

			return serviceProvider;
		}

		private Mock<IServiceProvider> GetServiceProvidernulldataget()
		{
			HttpResponseMessage message = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			//message.Content = new StringContent("{\"apiVersion\":\"1.0\",\"statusCode\":200,\"message\":\"Ok\",\"data\":[{ }}");
			HttpResponseMessage messagePost = new HttpResponseMessage();
			var HttpClientService = new Mock<IHttpClientService>();
			HttpClientService
				.Setup(x => x.GetAsync(It.IsAny<string>()))
				.ReturnsAsync(message);
			HttpClientService
				 .Setup(x => x.PostAsync(It.Is<string>(s => s.Contains("items/finished-goods")), It.IsAny<HttpContent>()))
				 .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(new ItemDataUtil().GetResultFormatterOkString()) });

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

		private SPKDocDataUtil dataUtil(Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade, string testName)
		{
			var pkbbjfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(ServiceProvider, _dbContext(testName));
			//var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
			//var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
			//var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

			return new SPKDocDataUtil(facade);
		}

		private SPKDocDataUtilCSV dataUtilCSV(Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade, string testName)
		{
			var pkbbjfacade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(ServiceProvider, _dbContext(testName));
			//var sPKDocDataUtil = new SPKDocDataUtil(pkbbjfacade);
			//var transferFacade = new TransferFacade(ServiceProvider, _dbContext(testName));
			//var transferDataUtil = new TransferDataUtil(transferFacade, sPKDocDataUtil);

			return new SPKDocDataUtilCSV(facade);
		}

		[Fact]
		public async Task Should_Success_Upload_Data()
		{
			Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var model = await dataUtil(facade, GetCurrentMethod()).GetTestData();
			var Response = facade.UploadData(model, USERNAME);
			Assert.NotNull(Response);
		}

		[Fact]
		public void Should_Success_Validate_UploadData()
		{
			Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var viewModel = dataUtilCSV(facade, GetCurrentMethod()).GetNewData();
			var viewModel2 = dataUtilCSV(facade, GetCurrentMethod()).GetNewData2();
			var viewModel3 = dataUtilCSV(facade, GetCurrentMethod()).GetNewData3();
			var viewModel4 = dataUtilCSV(facade, GetCurrentMethod()).GetNewData4();

			List<SPKDocsCsvViewModel> data = new List<SPKDocsCsvViewModel>();
			data.Add(viewModel);
			data.Add(viewModel2);
			data.Add(viewModel3);
			data.Add(viewModel4);

			List<KeyValuePair<string, StringValues>> Body = new List<KeyValuePair<string, StringValues>>();

			var Response = facade.UploadValidate(ref data, Body);

			Assert.True(Response.Item2.Count() > 0);

		}

		[Fact]
		public void Should_Success_Validate_UploadDataNoErrorMessage()
		{
			Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var viewModel = dataUtilCSV(facade, GetCurrentMethod()).GetNewDataValid();

			List<SPKDocsCsvViewModel> data = new List<SPKDocsCsvViewModel>();
			data.Add(viewModel);

			List<KeyValuePair<string, StringValues>> Body = new List<KeyValuePair<string, StringValues>>();

			var Response = facade.UploadValidate(ref data, Body);

			Assert.True(Response.Item2.Count() == 0);

		}

		[Fact]
		public async Task Should_Success_Map_ViewModel()
		{
			Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var viewModel = dataUtilCSV(facade, GetCurrentMethod()).GetNewData();
			viewModel.code = "code";
			viewModel.name = "name";
			viewModel.articleRealizationOrder = "Ro";
			viewModel.domesticCOGS = 0;
			viewModel.domesticSale = 0;
			viewModel.PackingList = "EFR";
			viewModel.Password = "1";
			viewModel.size = "S";
			viewModel.uom = "Pcs";
			viewModel.quantity = 1;
			viewModel._id = 0;

			List<SPKDocsCsvViewModel> data = new List<SPKDocsCsvViewModel>();
			data.Add(viewModel);

			var Response = await facade.MapToViewModel(data, 0, "codes", "names", 0, "coded", "named", DateTimeOffset.Now);

			Assert.NotNull(Response);
		}
		[Fact]
		public  async Task ShouldSuccesReadForUpload(){
			Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
			var viewModel = dataUtilCSV(facade, GetCurrentMethod()).GetNewData();
			viewModel.code = "code";
			viewModel.name = "name";
			viewModel.articleRealizationOrder = "Ro";
			viewModel.domesticCOGS = 0;
			viewModel.domesticSale = 0;
			viewModel.PackingList = "BTQ-FN";
			viewModel.Password = "1";
			viewModel.size = "S";
			viewModel.uom = "Pcs";
			viewModel.quantity = 1;
			viewModel._id = 0;

			List<SPKDocsCsvViewModel> data = new List<SPKDocsCsvViewModel>();
			data.Add(viewModel);

			var Response =  facade.ReadForUpload(1,25,"","","");

			Assert.NotNull(Response);
		}
        //[Fact]
        //public async Task Should_Success_Map_ViewModel2()
        //{
        //    Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade facade = new Com.Bateeq.Service.Warehouse.Lib.Facades.PkpbjFacade(GetServiceProvider().Object, _dbContext(GetCurrentMethod()));
        //    var viewModel = dataUtilCSV(facade, GetCurrentMethod()).GetNewData();
        //    viewModel.code = "code";
        //    viewModel.name = "name";

        //    List<SPKDocsCsvViewModel> data = new List<SPKDocsCsvViewModel>();
        //    data.Add(viewModel);

        //    var Response = await facade.MapToViewModel(data, 0, "codes", "names", 0, "coded", "named", DateTimeOffset.Now);

        //    Assert.NotNull(Response);
        //}
    }
}
