using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.AdjustmentInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.AdjustmentControllers
{

    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/adjustment/report")]
    [Authorize]

    public class AdjustmentReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly IAdjustmentDoc facade;
        private readonly IdentityService identityService;
        public readonly IServiceProvider serviceProvider;

        public AdjustmentReportController (IServiceProvider serviceProvider, IMapper mapper, IAdjustmentDoc facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet("byStorage")]
        public IActionResult Get(int id)
        {
            try
            {
                var model = facade.ReadByStorage(id);
                var viewModel = mapper.Map<List<AdjustmentDocsViewModel>>(model);
                if(viewModel == null)
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
