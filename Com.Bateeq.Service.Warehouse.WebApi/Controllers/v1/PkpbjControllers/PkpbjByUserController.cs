using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
//using Com.MM.Service.Warehouse.Lib.PDFTemplates;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.PDFTemplates;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
//using Com.DanLiris.Service.Purchasing.Lib.PDFTemplates;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.PkpbjControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/pkpbj/by-user")]
    [Authorize]

    public class PkpbjByUserController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IPkpbjFacade facade;
        private readonly IdentityService identityService;
        private readonly IServiceProvider serviceProvider;

        public PkpbjByUserController(IMapper mapper, IPkpbjFacade facade, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                var Data = facade.Read(page, size, order, keyword, filter);

                var newData = mapper.Map<List<SPKDocsViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.packingList,
                        s.date,
                        s.password,
                        s.reference,
                        SourceCode = s.source.code ,
                        SourceName = s.source.name ,
                        DestinationCode = s.destination.code,
                        DestinationName = s.destination.name,
                        s.isReceived,
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("packingList")]
        public IActionResult GetPackingList(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                var Data = facade.ReadPackingList(page, size, order, keyword, filter);

                var newData = mapper.Map<List<SPKDocsViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.packingList,
                        s.date,
                        s.password,
                        s.reference,
                        SourceCode = s.source.code,
                        SourceName = s.source.name,
                        DestinationCode = s.destination.code,
                        DestinationName = s.destination.name,
                        s.isReceived,
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("packingRTT")]
        public IActionResult GetPackingRTT(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                //string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                //if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                //{
                //    filter = string.Concat("{", filterUser, "}");
                //}
                //else
                //{
                //    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                //}

                var Data = facade.ReadPackingRTT(page, size, order, keyword, filter);

                var newData = mapper.Map<List<SPKDocsViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.packingList,
                        s.date,
                        s.password,
                        s.reference,
                        SourceCode = s.source.code ,
                        SourceName = s.source.name ,
                        DestinationCode = s.destination.code,
                        DestinationName = s.destination.name,
                        s.isReceived,
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("expedition")]
        public IActionResult GetExpedition(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                var Data = facade.ReadExpedition(page, size, order, keyword, filter);

                var newData = mapper.Map<List<SPKDocsViewModel>>(Data.Item1);

                //List<object> listData = new List<object>();
                //listData.AddRange(
                //    newData.AsQueryable().Select(s => new
                //    {
                //        s._id,
                //        s.packingList,
                //        s.date,
                //        s.password,
                //        SourceCode = s.source.code,
                //        SourceName = s.source.name,
                //        DestinationCode = s.destination.code,
                //        DestinationName = s.destination.name,
                //        s.isReceived,
                //    }).ToList()
                //);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = newData,
                    info = new Dictionary<string, object>
                    {
                        { "count", newData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                SPKDocs model = facade.ReadById(id);
                SPKDocsViewModel viewModel = mapper.Map<SPKDocsViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                //if (indexAcceptPdf < 0)
                //{
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
                    });
               // }
                //else
                //{
                //    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                //    PurchaseRequestPDFTemplate PdfTemplate = new PurchaseRequestPDFTemplate();
                //    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

                //    return new FileStreamResult(stream, "application/pdf")
                //    {
                //        FileDownloadName = $"{viewModel.Code}.pdf"
                //    };
                //}
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PkbjByUserViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<SPKDocs>(ViewModel);

                await facade.Create(model, identityService.Username);

                // await facade.Crea

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("pdf/{id}")]
        public IActionResult GetPackingListPDF(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                SPKDocs model = facade.ReadById(id);
                SPKDocsViewModel viewModel = mapper.Map<SPKDocsViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                if (indexAcceptPdf < 0)
                {
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
                    });
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

                    //foreach (var item in viewModel.items)
                    //{
                    //    var garmentInvoice = invoiceFacade.ReadById((int)item.garmentInvoice.Id);
                    //    var garmentInvoiceViewModel = mapper.Map<GarmentInvoiceViewModel>(garmentInvoice);
                    //    item.garmentInvoice = garmentInvoiceViewModel;

                    //    foreach (var detail in item.details)
                    //    {
                    //        var deliveryOrder = deliveryOrderFacade.ReadById((int)detail.deliveryOrder.Id);
                    //        var deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
                    //        detail.deliveryOrder = deliveryOrderViewModel;
                    //    }
                    //}

                    PackingListPdfTemplate PdfTemplateLocal = new PackingListPdfTemplate();
                    MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel, serviceProvider, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.packingList}.pdf"
                    };

                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("byreference")]
        public IActionResult Getbyreference(string reference)
        {
            try
            {
                var model = facade.ReadByReference(reference);
                //password minta ditampilkan tanggal 19 Juli 2022
				//model.Password = "";
                var viewModel = mapper.Map<SPKDocsViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(viewModel);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("transfer-stock/byreference")]
        public IActionResult Getbyreferencetransfer(string reference)
        {
            try
            {
                var model = facade.ReadByReference(reference);
                var viewModel = mapper.Map<SPKDocsViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(viewModel);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

    }
}
