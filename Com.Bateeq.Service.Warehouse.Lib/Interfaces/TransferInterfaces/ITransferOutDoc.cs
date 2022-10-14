using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces
{
    public interface ITransferOutDoc
    {
        Tuple<List<TransferOutDoc>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        Task<int> Create(TransferOutDocViewModel model, TransferOutDoc model2, string username, int clientTimeZoneOffset = 7);
        Tuple<List<TransferOutReadViewModel>, int, Dictionary<string, string>> ReadForRetur(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        TransferOutDoc ReadById(int id);
        MemoryStream GenerateExcel(int id);
        Task<int> CreateForPOS(TransferOutDoc model2, string username, int clientTimeZoneOffset = 7);
    }
}
