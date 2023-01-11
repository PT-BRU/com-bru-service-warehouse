using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.PkpbjControllers;
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
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.PkbjTests
{
    public class PkbjControllerTest
    {
        private PkbjByUserViewModel ViewModel
        {
            get
            {
                return new PkbjByUserViewModel
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
                    items = new List<PkbjByUserItemViewModel>
                    {
                        new PkbjByUserItemViewModel
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

        private PkpbjByUserController GetController(Mock<IPkpbjFacade> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            PkpbjByUserController controller = new PkpbjByUserController(mapper.Object,facadeM.Object, servicePMock.Object)
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
        public void Should_Success_Get_PackingList_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadPackingList(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingList();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_PackingList_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadPackingList(It.IsAny<int>(), It.IsAny<int>(), "}", null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingList();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), "{", null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Expedition_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadExpedition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetExpedition();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Expedition_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();


            mockFacade.Setup(x => x.ReadExpedition(It.IsAny<int>(), It.IsAny<int>(), "}", null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetExpedition();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PDF_PackingList()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocsViewModel>(It.IsAny<SPKDocs>()))
                .Returns(SpkViewModel);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);


            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.GetPackingListPDF(It.IsAny<int>());

            Assert.NotNull(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public void Should_Error_Get_PDF_PackingList()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            //mockMapper.Setup(x => x.Map<SPKDocsViewModel>(It.IsAny<SPKDocs>()))
            //    .Returns(SpkViewModel);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);


            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.GetPackingListPDF(It.IsAny<int>());

            Assert.Null(response.GetType().GetProperty("FileStream"));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PkbjByUserViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocs>(It.IsAny<PkbjByUserViewModel>()))
                .Returns(sPK);

            var mockFacade = new Mock<IPkpbjFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<SPKDocs>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PkbjByUserViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPkpbjFacade>();

            var controller = GetController(mockFacade, validateMock, mockMapper);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PkbjByUserViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocs>(It.IsAny<PkbjByUserViewModel>()))
                .Returns(sPK);

            var mockFacade = new Mock<IPkpbjFacade>();
            mockFacade.Setup(x => x.Create(It.IsAny<SPKDocs>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object ,GetServiceProvider().Object);

            var response = await controller.Post(this.ViewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocsViewModel>(It.IsAny<SPKDocs>()))
                .Returns(SpkViewModel);

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();

            PkpbjByUserController controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_reference()
        {
            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocsViewModel>(It.IsAny<SPKDocs>()))
                .Returns(SpkViewModel);

            PkpbjByUserController controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Getbyreference(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_refernce()
        {
            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();

            PkpbjByUserController controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Getbyreference(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_reference_TO()
        {
            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SPKDocsViewModel>(It.IsAny<SPKDocs>()))
                .Returns(SpkViewModel);

            PkpbjByUserController controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Getbyreferencetransfer(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_refernce_TO()
        {
            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();

            PkpbjByUserController controller = new PkpbjByUserController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Getbyreferencetransfer(It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PackingRTT_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadPackingRTT(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingRTT();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_PackingRTT_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadPackingRTT(It.IsAny<int>(), It.IsAny<int>(), "{", null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingRTT();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_PackingRTP_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingRTP();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_PackingRTP_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SPKDocsViewModel>())).Verifiable();

            var mockFacade = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), "{", null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<SPKDocs>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SPKDocsViewModel>>(It.IsAny<List<SPKDocs>>()))
                .Returns(new List<SPKDocsViewModel> { SpkViewModel });

            PkpbjByUserController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.GetPackingRTP();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
