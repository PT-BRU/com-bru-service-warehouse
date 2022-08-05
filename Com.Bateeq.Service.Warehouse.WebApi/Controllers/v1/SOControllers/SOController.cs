using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Moonlay.NetCore.Lib.Service;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.SOControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/stock-opname/by-user")]
    [Authorize]

    public class SOController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ISODoc facade;
        private readonly IdentityService identityService;
        private readonly IServiceProvider serviceProvider;

        public SOController(IMapper mapper, ISODoc facade, IServiceProvider serviceProvider)
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

                var newData = mapper.Map<List<SODocsViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.code,
                        s.storage,
                        s.CreatedBy,
                        s.isProcessed
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");
                SODocs model = facade.ReadById(id);
                SODocsViewModel viewModel = mapper.Map<SODocsViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }

        ////////        //if (indexAcceptPdf < 0)
        ////////        //{
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
                    });
        ////////       // }
        ////////        //else
        ////////        //{
        ////////        //    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

        ////////        //    PurchaseRequestPDFTemplate PdfTemplate = new PurchaseRequestPDFTemplate();
        ////////        //    MemoryStream stream = PdfTemplate.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

        ////////        //    return new FileStreamResult(stream, "application/pdf")
        ////////        //    {
        ////////        //        FileDownloadName = $"{viewModel.Code}.pdf"
        ////////        //    };
        ////////        //}
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        //[HttpGet("download")]
        //public IActionResult DownloadTemplate()
        //{
        //    try
        //    {
        //        byte[] csvInBytes;
        //        var csv = facade.DownloadTemplate();

        //        string fileName = "Stock Opname Template.csv";

        //        csvInBytes = csv.ToArray();

        //        var file = File(csvInBytes, "text/csv", fileName);
        //        return file;
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //          new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //          .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}

        [HttpGet("download")]
        public IActionResult DownloadTemplate(string code)
        {
            try
            {
                byte[] csvInBytes;
                var csv = facade.DownloadTemplate();

                string fileName = "Stock Opname Template.csv";

                csvInBytes = csv.ToArray();

                var file = File(csvInBytes, "text/csv", fileName);
                return file;
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                  new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                  .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Process([FromBody]SODocsViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var Model = mapper.Map<SODocs>(ViewModel);

                await facade.Process(Model, identityService.Username);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                await facade.Delete(id, identityService.Username);

                return NoContent();
            }

            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }

        }

        //[HttpGet("pdf/{id}")]
        //public IActionResult GetPackingListPDF(int id)
        //{
        //    try
        //    {
        //        var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

        //        SPKDocs model = facade.ReadById(id);
        //        SPKDocsViewModel viewModel = mapper.Map<SPKDocsViewModel>(model);
        //        if (viewModel == null)
        //        {
        //            throw new Exception("Invalid Id");
        //        }
        //        if (indexAcceptPdf < 0)
        //        {
        //            return Ok(new
        //            {
        //                apiVersion = ApiVersion,
        //                statusCode = General.OK_STATUS_CODE,
        //                message = General.OK_MESSAGE,
        //                data = viewModel,
        //            });
        //        }
        //        else
        //        {
        //            int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());

        //            //foreach (var item in viewModel.items)
        //            //{
        //            //    var garmentInvoice = invoiceFacade.ReadById((int)item.garmentInvoice.Id);
        //            //    var garmentInvoiceViewModel = mapper.Map<GarmentInvoiceViewModel>(garmentInvoice);
        //            //    item.garmentInvoice = garmentInvoiceViewModel;

        //            //    foreach (var detail in item.details)
        //            //    {
        //            //        var deliveryOrder = deliveryOrderFacade.ReadById((int)detail.deliveryOrder.Id);
        //            //        var deliveryOrderViewModel = mapper.Map<GarmentDeliveryOrderViewModel>(deliveryOrder);
        //            //        detail.deliveryOrder = deliveryOrderViewModel;
        //            //    }
        //            //}

        //            PackingListPdfTemplate PdfTemplateLocal = new PackingListPdfTemplate();
        //            MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel, serviceProvider, clientTimeZoneOffset);

        //            return new FileStreamResult(stream, "application/pdf")
        //            {
        //                FileDownloadName = $"{viewModel.packingList}.pdf"
        //            };

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}

        //[HttpGet("byreference")]
        //public IActionResult Getbyreference(string reference)
        //{
        //    try
        //    {
        //        var model = facade.ReadByReference(reference);
        //        model.Password = "";
        //        var viewModel = mapper.Map<SPKDocsViewModel>(model);
        //        if (viewModel == null)
        //        {
        //            throw new Exception("Invalid Id");
        //        }

        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
        //            .Ok(viewModel);
        //        return Ok(Result);
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}
        //[HttpGet("transfer-stock/byreference")]
        //public IActionResult Getbyreferencetransfer(string reference)
        //{
        //    try
        //    {
        //        var model = facade.ReadByReference(reference);
        //        var viewModel = mapper.Map<SPKDocsViewModel>(model);
        //        if (viewModel == null)
        //        {
        //            throw new Exception("Invalid Id");
        //        }

        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
        //            .Ok(viewModel);
        //        return Ok(Result);
        //    }
        //    catch (Exception e)
        //    {
        //        Dictionary<string, object> Result =
        //            new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
        //            .Fail();
        //        return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
        //    }
        //}

    }
}
