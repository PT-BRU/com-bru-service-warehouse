using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces
{
    public interface ISPKDoc
    {
        Tuple<List<SPKDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        SPKDocs ReadById(int id);
        SPKDocs ReadByReference(string reference);
        Task<int> Create(SPKDocsFromFinihsingOutsViewModel viewModel, string username, string token);
        List<SPKDocs> ReadByFinishingOutIdentity(string FinishingOutIdentity);
    }
}
