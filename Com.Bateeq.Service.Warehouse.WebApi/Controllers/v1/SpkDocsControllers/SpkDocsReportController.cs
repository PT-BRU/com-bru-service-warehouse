using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.WebApi.Controllers.v1.SpkDocsControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/spkdocs/monitoring")]
    [Authorize]
    public class SPKDocstReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly SPKDocsFacade facade;
        private readonly IdentityService identityService;

        public SPKDocstReportController(IMapper mapper, SPKDocsFacade facade, IdentityService identityService)
        {
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = identityService;
        }
        #region By User
        [HttpGet("by-user")]
        public IActionResult GetReport(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList, int page = 1, int size = 25, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            string accept = Request.Headers["Accept"];

            try
            {

                var data = facade.GetReport(dateFrom, dateTo, destinationCode, status, transaction, packingList, page, size, Order, offset, identityService.Username);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
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

        [HttpGet("by-user/download")]
        public IActionResult GetXls(DateTime? dateFrom, DateTime? dateTo, string destinationCode, bool status, int transaction, string packingList)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = facade.GenerateExcel(dateFrom, dateTo, destinationCode, status, transaction, packingList, offset, identityService.Username);

                string filename = String.Format("Packing List - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

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
        #endregion
    }
}