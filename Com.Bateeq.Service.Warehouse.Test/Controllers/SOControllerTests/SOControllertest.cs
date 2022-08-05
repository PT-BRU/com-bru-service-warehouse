using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.SOControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Bateeq.Service.Warehouse.Test.Controllers.SOControllerTests
{
    public class SOControllertest
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
        private SOController GetController(Mock<ISODoc> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper)
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

            SOController controller = new SOController(mapper.Object, facadeM.Object, servicePMock.Object)
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

        private SODocsViewModel vm
        {
            get
            {
                return new SODocsViewModel
                {
                    code = "Code",
                    
                    items = new List<SODocsItemViewModel>
                    {
                        new SODocsItemViewModel
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

        private SODocs model
        {
            get
            {
                return new SODocs
                {
                    Code = "Code",

                    Items = new List<SODocsItem>
                    {
                        new SODocsItem
                        {
                            ItemCode = "code"

                        }
                    }
                };
            }
        }

        [Fact]
        public void Should_Success_Get()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new Tuple<List<SODocs>, int, Dictionary<string, string>>(It.IsAny<List<SODocs>>(), It.IsAny<int>(), It.IsAny<Dictionary<string,string>>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SODocsViewModel>>(It.IsAny<List<SODocs>>()))
                .Returns(new List<SODocsViewModel> { vm });

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
                .Returns(new Tuple<List<SODocs>, int, Dictionary<string, string>>(It.IsAny<List<SODocs>>(), It.IsAny<int>(), It.IsAny<Dictionary<string, string>>()));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<SODocsViewModel>>(It.IsAny<List<SODocs>>()))
                .Returns(new List<SODocsViewModel> { It.IsAny< SODocsViewModel>() });

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get();
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }


        [Fact]
        public void Should_Success_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SODocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SODocsViewModel>(It.IsAny<SODocs>()))
                .Returns(vm);

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Get_Data_By_Id()
        {
            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SODocs());

            var mockMapper = new Mock<IMapper>();

            SOController controller = new SOController(mockMapper.Object, mockFacade.Object, GetServiceProvider().Object);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_exception_Get_Data_By_Id()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
                .Returns(new SODocs());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SODocsViewModel>(It.IsAny<SODocs>()))
                .Returns(It.IsAny<SODocsViewModel>);

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Get(It.IsAny<int>());
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Delete(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Delete_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<string>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Delete(It.IsAny<int>()).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Should_Success_Process_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Process(It.IsAny<SODocs>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(1);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SODocs>(It.IsAny<SODocsViewModel>()))
                .Returns(model);

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Process(vm).Result;
            Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
        }

        [Fact]
        public void Should_Error_Process_Data()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<SODocsViewModel>())).Verifiable();

            var mockFacade = new Mock<ISODoc>();

            mockFacade.Setup(x => x.Process(It.IsAny<SODocs>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<SODocs>(It.IsAny<SODocsViewModel>()))
                .Returns(model);

            SOController controller = GetController(mockFacade, validateMock, mockMapper);
            var response = controller.Process(vm).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        

    }
}
