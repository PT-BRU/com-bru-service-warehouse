using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Transfer;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.TransferTests
{
    public class TransferOutDocControllerTest
    {
        private TransferOutDocViewModel viewModel
        {
            get
            {
                return new TransferOutDocViewModel
                {
                    code = "code",
                    reference = "reference",
                    destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                    {
                        code = "code",
                        name = "name"
                    },
                    source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                    {
                        code = "code",
                        name = "name"
                    },
                    expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                    {
                        code = "code",
                        name = "name",
                        _id = 1
                    },
                    items = new List<TransferOutDocItemViewModel>
                    {
                        new TransferOutDocItemViewModel
                        {
                            articleRealizationOrder = "ro",
                            item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                            {
                                code = "code",
                                name = "name",
                                domesticSale = 0
                            },
                            quantity = 0
                        }
                    }
                };
            }
        }

        private TransferOutDoc Model
        {
            get
            {
                return new TransferOutDoc
                {
                    Code = "code",
                    DestinationCode = "destination",
                    Items = new List<TransferOutDocItem>
                    {
                        new TransferOutDocItem{
                            ArticleRealizationOrder = "RO"
                        }
                    }
                };
            }
        }

        private PkbjByUserViewModel viewModel1
        {
            get
            {
                return new PkbjByUserViewModel
                {
                    code = "code",
                    destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                    {
                        code = "code",
                        name = "name"
                    },
                    source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                    {
                        code = "code",
                        name = "name"
                    },
                    reference = "code",
                    items = new List<PkbjByUserItemViewModel>
                    {
                        new PkbjByUserItemViewModel
                        {
                            item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel
                            {
                                articleRealizationOrder = "RO",
                                code = "itemcode",
                                domesticSale = 0
                            },
                            quantity = 0
                            
                        }
                    }
                };
            }
        }
        
        private ServiceValidationExeption GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(viewModel, serviceProvider.Object, null);
            return new ServiceValidationExeption(validationContext, validationResults);
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private TransferOutDocController GetController(Mock<ITransferOutDoc> facadeM, Mock<IPkpbjFacade> facadePkbj, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            TransferOutDocController controller = new TransferOutDocController(servicePMock.Object, mapper.Object, facadeM.Object, facadePkbj.Object )
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
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferOutDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferOutDocViewModel>>(It.IsAny<List<TransferOutDoc>>()))
                .Returns(new List<TransferOutDocViewModel> { viewModel });

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferOutDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferOutDocViewModel>>(It.IsAny<List<TransferOutDoc>>()))
                .Returns(new List<TransferOutDocViewModel> { viewModel });

            TransferOutDocController controller = GetController(mockFacade, mockFacadePkbj, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_All_Data_Retur_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();


            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferOutDoc>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferOutDocViewModel>>(It.IsAny<List<TransferOutDoc>>()))
                .Returns(new List<TransferOutDocViewModel> { viewModel });

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            var response = controller.GetRetur();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_All_Data_Retur_By_User()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadForRetur(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(Tuple.Create(new List<TransferOutReadViewModel>(), 0, new Dictionary<string, string>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<TransferOutDocViewModel>>(It.IsAny<List<TransferOutDoc>>()))
                .Returns(new List<TransferOutDocViewModel> { viewModel });

            TransferOutDocController controller = GetController(mockFacade, mockFacadePkbj, validateMock, mockMapper);
            var response = controller.GetRetur();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferOutDoc>(It.IsAny<TransferOutDocViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Create(It.IsAny<TransferOutDocViewModel>(), It.IsAny<TransferOutDoc>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = GetController(mockFacade, mockFacadePkbj, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Validate_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Throws(GetServiceValidationExeption());

            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            var controller = GetController(mockFacade, mockFacadePkbj, validateMock, mockMapper);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Error_Create_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<TransferOutDocViewModel>())).Verifiable();

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferOutDoc>(It.IsAny<TransferOutDocViewModel>()))
                .Returns(Model);

            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.Create(It.IsAny<TransferOutDocViewModel>(), It.IsAny<TransferOutDoc>(), "unittestusername", 7))
               .ReturnsAsync(1);

            var controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);

            var response = await controller.Post(this.viewModel);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>(); 

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferOutDoc());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferOutDocViewModel>(It.IsAny<TransferOutDoc>()))
                .Returns(viewModel);

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferOutDoc());

            var mockMapper = new Mock<IMapper>();

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Sucess_Get_Excel()
        {
            var mockFacade = new Mock<ITransferOutDoc>();
            mockFacade.Setup(x => x.GenerateExcel(It.IsAny<int>()))
                .Returns(new MemoryStream());
            var mockMapper = new Mock<IMapper>();
            var mockFacadePkbj = new Mock<IPkpbjFacade>();
            //var INVFacade = new Mock<IGarmentInvoice>();
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";
            var response = controller.GetXls(It.IsAny<int>());
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.GetType().GetProperty("ContentType").GetValue(response, null));
        }

        [Fact]
        public void Should_Error_Get_Excel()
        {
            var mockFacade = new Mock<ITransferOutDoc>();
            var mockMapper = new Mock<IMapper>();
            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);
            var response = controller.GetXls(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Get_ExpeditionPDF()
        {
            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferOutDoc());

            mockFacadePkbj.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<TransferOutDocViewModel>(It.IsAny<TransferOutDoc>()))
                .Returns(viewModel);
            mockMapper.Setup(x => x.Map<PkbjByUserViewModel>(It.IsAny<SPKDocs>()))
                .Returns(viewModel1);

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
{
                new Claim("username", "unittestusername")
};
            user.Setup(u => u.Claims).Returns(claims);

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.GetExpeditionPDF(It.IsAny<int>());

            Assert.NotNull(response.GetType().GetProperty("FileStream"));

        }

        [Fact]
        public void Should_Error_Get_ExpeditionPDF()
        {
            var mockFacade = new Mock<ITransferOutDoc>();

            var mockFacadePkbj = new Mock<IPkpbjFacade>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new TransferOutDoc());

            mockFacadePkbj.Setup(x => x.ReadByReference(It.IsAny<string>()))
                .Returns(new SPKDocs());

            var mockMapper = new Mock<IMapper>();

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
{
                new Claim("username", "unittestusername")
};
            user.Setup(u => u.Claims).Returns(claims);

            TransferOutDocController controller = new TransferOutDocController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, mockFacadePkbj.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

            var response = controller.GetExpeditionPDF(It.IsAny<int>());

            Assert.Null(response.GetType().GetProperty("FileStream"));
        }
    }
}
