using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.AdjustmentInterfaces
{
    public interface IAdjustmentDoc
    {
        Tuple<List<AdjustmentDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        AdjustmentDocs ReadById(int id);
        List<AdjustmentDocs> ReadByStorage(int id);
        Task<int> Create(AdjustmentDocs model, string username, int clientTimeZoneOffset = 7);
    }
}
