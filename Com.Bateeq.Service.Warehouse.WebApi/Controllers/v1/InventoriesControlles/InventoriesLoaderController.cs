using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.InventoryLoaderInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.InventoriesControlles
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/inventory-loader")]
    [Authorize]

    public class InventoriesLoaderController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IdentityService identityService;
        private readonly IInventoryLoader iInventoryLoader;

        public InventoriesLoaderController(IMapper mapper, IdentityService identityService, IInventoryLoader iInventoryLoader)
        {
            this.mapper = mapper;
            this.identityService = identityService;
            this.iInventoryLoader = iInventoryLoader;
        }

        [HttpGet("itemCode")]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {
                var Data = iInventoryLoader.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<InventoryViewModel>>(Data.Item1);

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
    }
}
