using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.MM.Service.Core.WebApi.Controllers.v1.UploadControllers;
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

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.UploadControllerTests
{
	public class PkpbjUploadControllerTest
	{
		private SPKDocsViewModel ViewModel
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

		private SPKDocs sPK
		{
			get
			{
				return new SPKDocs
				{
					Id = 1,
					Items = new List<SPKDocsItem>
					{
					new SPKDocsItem
					{
						Id = 1,
                        //Details = new List<GarmentInvoiceDetail>
                        //{
                        //    new GarmentInvoiceDetail
                        //    {
                        //        Id = 1,
                        //        DODetailId = 1,
                        //    }
                        //}
                    }
				}
				};
			}
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

		private PkpbjUploadController GetController(Mock<IPkpbjFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

			PkpbjUploadController controller = new PkpbjUploadController(mapper.Object, facadeM.Object, servicePMock.Object)
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
		//[Fact]
		//public void Should_Success_Get_()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

		//	var mockFacade = new Mock<IPkpbjFacade>();

		//	mockFacade.Setup(x => x.ReadForUpload(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
		//		.Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
		//		.Returns(new List<SPKDocsViewModel> { SpkViewModel });

		//	PkpbjUploadController controller = GetController(mockFacade, validateMock, mockMapper);
		//	var response = controller.Get();
		//	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		//}
		[Fact]
		public void Should_Error_Get_()
		{
			var validateMock = new Mock<IValidateService>();
			validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

			var mockFacade = new Mock<IPkpbjFacade>();

			mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
				.Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
				.Returns(new List<SPKDocsViewModel> { SpkViewModel });

			PkpbjUploadController controller = GetController(mockFacade, validateMock, mockMapper);
			var response = controller.Get();
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		}
		[Fact]
		public void Should_Error_Upload()
		{
			var validateMock = new Mock<IValidateService>();
			validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

			var mockFacade = new Mock<IPkpbjFacade>();

			mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
				.Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

			var mockMapper = new Mock<IMapper>();
			mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
				.Returns(new List<SPKDocsViewModel> { SpkViewModel });

			PkpbjUploadController controller = GetController(mockFacade, validateMock, mockMapper);
			var response = controller.PostCSVFileAsync(1,"","",2,"","",DateTimeOffset.Now);
			Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response.Result));
		}
	}
}
