using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces;
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

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.Transfer
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/transfer-in")]
    [Authorize]
    public class TransferController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ITransferInDoc facade;
        private readonly IdentityService identityService;
        public readonly IServiceProvider serviceProvider;

        public TransferController(IServiceProvider serviceProvider, IMapper mapper, ITransferInDoc facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<TransferInDocViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => s).ToList()
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
        [HttpGet("retur")]
        public IActionResult GetRetur(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                //var viewModel = mapper.Map<List<TransferInDocViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    Data.Item1.AsQueryable().Select(s => s).ToList()
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
                var viewModel = mapper.Map<TransferInDocViewModel>(model);
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
        public async Task<IActionResult> Post([FromBody]TransferInDocViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<TransferInDoc>(ViewModel);

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
        [HttpPost("for-pos")]
        public async Task<IActionResult> CreateForPos([FromBody]TransferInDocViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                // validateService.Validate(ViewModel);

                var model = mapper.Map<TransferInDoc>(ViewModel);

                await facade.CreateForPos(model, identityService.Username);

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
    }
}
