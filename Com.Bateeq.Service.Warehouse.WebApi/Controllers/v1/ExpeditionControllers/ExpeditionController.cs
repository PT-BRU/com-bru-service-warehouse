using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.PDFTemplates;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.ExpeditionViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.ExpeditionControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/btq-kb-exp")]
    [Authorize]
    public class ExpeditionController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ExpeditionFacade facade;
        private readonly IdentityService identityService;
        private readonly IServiceProvider serviceProvider;
        public ExpeditionController(IMapper mapper, ExpeditionFacade facade, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<ExpeditionViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s=>new {
                        s._id,
                        s.code,
                        s.expeditionService.name,
                        s.weight,
                        s.date,
                        destination = s.items.Select(i=>i.spkDocsViewModel.destination.name),
                        s.CreatedBy

                    })
                );

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
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
        [HttpGet("byStore")]
        public IActionResult GetExpeditionByStore(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
                var Data = facade.ReadExpeditionByStore(identityService.Token, page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<ExpeditionViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new {
                        s._id,
                        s.code,
                        s.expeditionService.name,
                        s.weight,
                        s.date,
                        destination = s.items.Select(i => i.spkDocsViewModel.destination.name),
                        s.CreatedBy

                    })
                );

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
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
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var model = facade.ReadById(id);
                //model.Password = "";
                var viewModel = mapper.Map<ExpeditionViewModel>(model);
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ExpeditionViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<Expedition>(ViewModel);

                await facade.Create(model, identityService.Username);

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
        public IActionResult GetExpeditionPDF(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                Expedition model = facade.ReadById(id);
                ExpeditionViewModel viewModel = mapper.Map<ExpeditionViewModel>(model);
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

                    #region comment
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
                    #endregion

                    ExpeditionPdfTemplate PdfTemplateLocal = new ExpeditionPdfTemplate();
                    MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel, serviceProvider, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.code}.pdf"
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
    }
}
