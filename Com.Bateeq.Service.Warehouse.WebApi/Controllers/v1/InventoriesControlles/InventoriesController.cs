using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
//using Com.MM.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
//using Com.MM.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using General = Com.MM.Service.Warehouse.Lib.Helpers.General;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.InventoriesControlles
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/inventory")]
    [Authorize]
    public class InventoriesController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly InventoryFacade facade;
        private readonly IdentityService identityService;

        public InventoriesController(IMapper mapper, InventoryFacade facade, IServiceProvider serviceProvider)
        {
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetCode(string itemData, string source, int size = 25, int page = 1, string Order = "{}")
        {
            //try
            //{
            //    Tuple<List<Storage>, int, Dictionary<string, string>, List<string>> Data = service.ReadModel(Page, Size, Order, Select, Keyword, Filter);

            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
            //        .Ok<Storage, Storage>(Data.Item1, service.MapToViewModel, Page, Size, Data.Item2, Data.Item1.Count, Data.Item3, Data.Item4);

            //    return Ok(Result);
            //}

            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.GetItemPack(itemData, source, Order, page, size);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1[0],
                    info = new { count = data.Item2, order = Order, page = page, size = size, total = data.Item2, },

                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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

        [HttpGet("code")]
        public IActionResult GetName(string itemData, int source)
        {

            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.getDatabyCode(itemData, source);
                var model = mapper.Map<List<InventoryViewModel>>(data);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = model,
                    info = new { count = data.Count(), total = data.Count() },

                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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
        [HttpGet("name")]
        public IActionResult GetbyCode(string itemData, int source)
        {
           

            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.getDatabyName(itemData, source);
                var model = mapper.Map<List<InventoryViewModel>>(data);


                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = model,
                    info = new { count = data.Count(), total = data.Count() },

                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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

        [HttpGet("stock")]
        public IActionResult GetStock(int source, int itemId)
        {


            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.getStock(source, itemId);
                var model = mapper.Map<InventoryViewModel>(data);


                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = model,
                    info = new { count = 1, total = 1 },

                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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

        [HttpGet("stock-pos")]
        public IActionResult GetStockForPOS(string source, string itemId)
        {


            string accept = Request.Headers["Accept"];
            try
            {

                var data = facade.getStockPOS(source, itemId);
                var model = mapper.Map<InventoryViewModel>(data);



                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = model,
                    info = new { count = 1, total = 1 },

                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
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
    }
}
