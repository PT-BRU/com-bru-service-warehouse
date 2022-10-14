using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces
{
    public interface ITransferInDoc
    {
        Tuple<List<TransferInDoc>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
        TransferInDoc ReadById(int id);
        Task<int> Create(TransferInDoc model, string username, int clientTimeZoneOffset = 7);
        Task<int> CreateForPos(TransferInDoc model, string username, int clientTimeZoneOffset = 7);
    }
}
