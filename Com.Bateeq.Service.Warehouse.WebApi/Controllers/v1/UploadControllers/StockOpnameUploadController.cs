using Microsoft.AspNetCore.Mvc;
using Com.Bateeq.Service.Warehouse.Lib.Services;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;

using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AutoMapper;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SOViewModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;

namespace Com.MM.Service.Core.WebApi.Controllers.v1.UploadControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/warehouse/upload-so")]
    [Authorize]
    public class StockOpnameUploadController : Controller
    //: BasicUploadController<PkpbjFacade, SPKDocs, SPKDocsViewModel, PkpbjFacade.PkbjMap, WarehouseDbContext>
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper mapper;
        private readonly ISODoc facade;
        private readonly IdentityService identityService;
        private readonly string ContentType = "application/vnd.openxmlformats";
        private readonly string FileName = string.Concat("Error Log - ", typeof(SODocs).Name, " ", DateTime.Now.ToString("dd MMM yyyy"), ".csv");
        public StockOpnameUploadController(IMapper mapper, ISODoc facade, IdentityService identityService) //: base(facade, ApiVersion)
        {
            this.mapper = mapper;
            this.facade = facade;
            this.identityService = identityService;
        }

        //private Action<COAModel> Transfrom => (coaModel) =>
        //{
        //    var codeArray = coaModel.Code.Split('.');
        //    coaModel.Code1 = codeArray[0];
        //    coaModel.Code2 = codeArray[1];
        //    coaModel.Code3 = codeArray[2];
        //    coaModel.Code4 = codeArray[3];
        //    coaModel.Header = coaModel.Code.Substring(0, 1);
        //    coaModel.Subheader = coaModel.Code.Substring(0, 2);

        //};
        [HttpPost("upload")]
        public async Task<IActionResult> PostCSVFileAsync(string source)
        // public async Task<IActionResult> PostCSVFileAsync(double source, double destination,  DateTime date)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
                identityService.Token = Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");
                identityService.TimezoneOffset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                if (Request.Form.Files.Count > 0)
                {
                    //VerifyUser();
                    var UploadedFile = Request.Form.Files[0];
                    StreamReader Reader = new StreamReader(UploadedFile.OpenReadStream());
                    List<string> FileHeader = new List<string>(Reader.ReadLine().Split(";"));
                    var ValidHeader = facade.CsvHeader.SequenceEqual(FileHeader, StringComparer.OrdinalIgnoreCase);

                    if (ValidHeader)
                    {
                        Reader.DiscardBufferedData();
                        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        Reader.BaseStream.Position = 0;
                        CsvReader Csv = new CsvReader(Reader);
                        Csv.Configuration.IgnoreQuotes = true;
                        Csv.Configuration.Delimiter = ";";
                        Csv.Configuration.RegisterClassMap<Bateeq.Service.Warehouse.Lib.Facades.SOFacade.SOMap>();
                        Csv.Configuration.HeaderValidated = null;

                        List<SODocsCsvViewModel> Data = Csv.GetRecords<SODocsCsvViewModel>().ToList();

                        Tuple<bool, List<object>> Validated = facade.UploadValidate(ref Data, Request.Form.ToList(), source);

                        Reader.Close();

                        if (Validated.Item1) /* If Data Valid */
                        {
                            SODocsViewModel Data1 = await facade.MapToViewModel(Data, source);
                            SODocs data = mapper.Map<SODocs>(Data1);

                            await facade.UploadData(data, identityService.Username);

                            Dictionary<string, object> Result =
                                new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                                .Ok();
                            return Created(HttpContext.Request.Path, Result);
                        }
                        else
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                                {
                                    var configuration = new CsvHelper.Configuration.Configuration();
                                    configuration.Delimiter = ";";

                                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, configuration))
                                    {
                                        csvWriter.WriteRecords(Validated.Item2);
                                    }
                                    return File(memoryStream.ToArray(), ContentType, FileName);
                                }
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, object> Result =
                           new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, General.CSV_ERROR_MESSAGE)
                           .Fail();

                        return NotFound(Result);
                    }
                }
                else
                {
                    Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.NO_FILE_ERROR_MESSAGE)
                            .Fail();
                    return BadRequest(Result);
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
