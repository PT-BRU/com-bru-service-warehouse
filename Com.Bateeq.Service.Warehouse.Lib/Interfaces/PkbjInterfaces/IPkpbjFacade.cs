using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces
{
    public interface IPkpbjFacade
    {
        Tuple<List<SPKDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadForUpload(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
		Tuple<List<SPKDocsViewModel>, int, Dictionary<string, string>> ReadForUploadNew(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
		Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadExpedition(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        SPKDocs ReadById(int id);
        SPKDocs ReadByReference(string reference);
        Task<int> Create(SPKDocs model, string username, int clientTimeZoneOffset = 7);
        Tuple<bool, List<object>> UploadValidate(ref List<SPKDocsCsvViewModel> Data, List<KeyValuePair<string, StringValues>> Body);
        List<string> CsvHeader { get; }
        Task UploadData(SPKDocs data, string username);
        Task<SPKDocsViewModel> MapToViewModel(List<SPKDocsCsvViewModel> data, double source, string sourcec, string sourcen, double destination, string destinationc, string destinationn, DateTimeOffset date);

        Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPackingRTT(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPackingList(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");

    }
}
