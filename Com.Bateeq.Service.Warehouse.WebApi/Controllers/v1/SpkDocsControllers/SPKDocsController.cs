using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.SpkDocsControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/spkdocs")]
    [Authorize]
    
    public class SPKDocsController: Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IdentityService identityService;
        private readonly ISPKDoc iSPKDocs;

        public SPKDocsController(IdentityService identityService, ISPKDoc iSPKDocs)
        {
            this.identityService = identityService;
            this.iSPKDocs = iSPKDocs;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SPKDocsFromFinihsingOutsViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                await iSPKDocs.Create(ViewModel, identityService.Username, identityService.Token);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("FinishingOutIdentity/{FinishingOutIdentity}")]
        public IActionResult GetByFinishingIdentity([FromRoute] string FinishingOutIdentity)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

                var data = iSPKDocs.ReadByFinishingOutIdentity(FinishingOutIdentity);

                var info = new Dictionary<string, object>
                    {
                        { "count", data.Count }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(data, info);
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