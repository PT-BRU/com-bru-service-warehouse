using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.ReturnToCenterInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Stores.ReturnToCenterController
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/stores/btq-kb-rtp")]
    [Authorize]
    public class ReturnToCenterController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ReturnToCenterFacade facade;
        private readonly IdentityService identityService;
        public readonly IServiceProvider serviceProvider;

        public ReturnToCenterController(IServiceProvider serviceProvider, IMapper mapper, ReturnToCenterFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetRetur(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadForRetur(page, size, order, keyword, filter);

                //var viewModel = mapper.Map<List<TransferOutDocViewModel>>(Data.Item1);

                //List<object> listData = new List<object>();
                //listData.AddRange(
                //    viewModel.AsQueryable().Select(s => s).ToList()
                //);

                var info = new Dictionary<string, object>
                    {
                        { "count", Data.Item1.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(Data.Item1, info);
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
        public async Task<IActionResult> Post([FromBody]TransferOutDocViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<TransferOutDoc>(ViewModel);

                await facade.Create(ViewModel, model, identityService.Username);

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
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var model = facade.ReadById(id);
                var viewModel = mapper.Map<TransferOutDocViewModel>(model);
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
        [HttpGet("{id}/exportall")]
        public IActionResult GetXls(int id)
        {
            //Console.WriteLine("sampai sini");
            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                string accept = Request.Headers["Accept"];

                var xls = facade.GenerateExcel(id);
                string filename = String.Format("Report RTP - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
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

    }
}
