using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.DanLiris.Service.Warehouse.Lib.ViewModels.TransferViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Interfaces.Stores.TransferStocksInterfaces
{
	public interface ITransferStock
	{
		Tuple<List<TransferOutDoc>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
		Tuple<List<TransferStockViewModel>, int, Dictionary<string, string>> ReadModel(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}");
		TransferStockViewModel ReadById(int id);
		Task<int> Create(TransferOutDocViewModel model, TransferOutDoc model2, string username, int clientTimeZoneOffset = 7);
		Tuple<List<TransferStockReportViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string status, string code, int page, int size, string Order, int offset);
		 

	}
}
