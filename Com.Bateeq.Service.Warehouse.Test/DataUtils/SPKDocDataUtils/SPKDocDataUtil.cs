using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils
{
	public class SPKDocDataUtil
	{
		private readonly PkpbjFacade pkpbjFacade;

		public SPKDocDataUtil(PkpbjFacade facade/*, GarmentInternalPurchaseOrderDataUtil garmentPurchaseOrderDataUtil*/)
		{
			this.pkpbjFacade = facade;
			//this.garmentPurchaseOrderDataUtil = garmentPurchaseOrderDataUtil;
		}
		public SPKDocs GetNewData()
		{
			//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
			return new SPKDocs
			{
				Code = "codetest",
				Date = DateTimeOffset.Now,
				DestinationCode = "destinationcode1",
				DestinationName = "destinationname",
				DestinationId = 1,
				IsDistributed = false,
				IsDraft = false,
				IsReceived = false,
				PackingList = "packinglist",
				Password = "password",
				SourceCode = "SourceCode",
				SourceId = 1,
				SourceName = "SourceName",
				Weight = 1,
				Reference = "reference",
				Items = new List<SPKDocsItem>
				{
					new SPKDocsItem
					{
						ItemArticleRealizationOrder = "art1",
						ItemCode = "itemcode1",
						ItemDomesticCOGS = 0,
						ItemDomesticRetail = 0,
						ItemDomesticSale = 0,
						ItemDomesticWholesale = 0,
						ItemId = 1,
						ItemName = "name12",
						ItemSize = "Size12",
						ItemUom =  "Uom12",
						Quantity = 0,
						Remark = "remark",
						SendQuantity = 0
					}
				}
			};
		}
		public SPKDocs GetNewDataForUpload()
		{
			//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
			return new SPKDocs
			{
				Code = "codetest",
				Date = DateTimeOffset.Now,
				DestinationCode = "destinationcode1",
				DestinationName = "destinationname",
				DestinationId = 1,
				IsDistributed = false,
				IsDraft = false,
				IsReceived = false,
				PackingList = "BTQ-FN",
				Password = "password",
				SourceCode = "SourceCode",
				SourceId = 1,
				SourceName = "SourceName",
				Weight = 1,
				Reference = "reference",
				Items = new List<SPKDocsItem>
				{
					new SPKDocsItem
					{
						ItemArticleRealizationOrder = "art1",
						ItemCode = "itemcode1",
						ItemDomesticCOGS = 0,
						ItemDomesticRetail = 0,
						ItemDomesticSale = 0,
						ItemDomesticWholesale = 0,
						ItemId = 1,
						ItemName = "name12",
						ItemSize = "Size12",
						ItemUom =  "Uom12",
						Quantity = 0,
						Remark = "remark",
						SendQuantity = 0
					}
				}
			};
		}
		public async Task<SPKDocs> GetTestData()
		{
			var data = GetNewData();
			await pkpbjFacade.Create(data, "Unit Test");
			return data;
		}
		public async Task<SPKDocs> GetTestDataUpload()
		{
			var data = GetNewDataForUpload();
			await pkpbjFacade.Create(data, "Unit Test");
			return data;
		}
		public class SPKDocDataUtilRTT
		{
			private readonly PkpbjFacade pkpbjFacade;

			public SPKDocDataUtilRTT(PkpbjFacade facade/*, GarmentInternalPurchaseOrderDataUtil garmentPurchaseOrderDataUtil*/)
			{
				this.pkpbjFacade = facade;
				//this.garmentPurchaseOrderDataUtil = garmentPurchaseOrderDataUtil;
			}
			public SPKDocs GetNewData()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocs
				{
					Code = "codetest",
					Date = DateTimeOffset.Now,
					DestinationCode = "destinationcode1",
					DestinationName = "destinationname",
					DestinationId = 1,
					IsDistributed = false,
					IsDraft = false,
					IsReceived = false,
					PackingList = "packinglist",
					Password = "password",
					SourceCode = "SourceCode",
					SourceId = 1,
					SourceName = "SourceName",
					Weight = 1,
					Reference = "BTQ-KB/RTT",
					Items = new List<SPKDocsItem>
				{
					new SPKDocsItem
					{
						ItemArticleRealizationOrder = "art1",
						ItemCode = "itemcode1",
						ItemDomesticCOGS = 0,
						ItemDomesticRetail = 0,
						ItemDomesticSale = 0,
						ItemDomesticWholesale = 0,
						ItemId = 1,
						ItemName = "name12",
						ItemSize = "Size12",
						ItemUom =  "Uom12",
						Quantity = 0,
						Remark = "remark",
						SendQuantity = 0
					}
				}
				};
			}

			public async Task<SPKDocs> GetTestData()
			{
				var data = GetNewData();
				await pkpbjFacade.Create(data, "Unit Test");
				return data;
			}
		}

		public class SPKDocDataUtilCSV
		{
			private readonly PkpbjFacade pkpbjFacade;

			public SPKDocDataUtilCSV(PkpbjFacade facade/*, GarmentInternalPurchaseOrderDataUtil garmentPurchaseOrderDataUtil*/)
			{
				this.pkpbjFacade = facade;
				//this.garmentPurchaseOrderDataUtil = garmentPurchaseOrderDataUtil;
			}
			public SPKDocsCsvViewModel GetNewData()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocsCsvViewModel
				{
					PackingList = "",
					Password = "",
					code = "",
					name = "",
					size = "",
					domesticSale = "",
					uom = "",
					quantity = "",
					articleRealizationOrder = "",
					domesticCOGS = ""
				};
			}

			//public async Task<SPKDocsCsvViewModel> GetTestData()
			//{
			//    var data = GetNewData();
			//    await pkpbjFacade.Create(data, "Unit Test");
			//    return data;
			//}

			public SPKDocsCsvViewModel GetNewData2()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocsCsvViewModel
				{
					PackingList = "",
					Password = "",
					code = "code",
					name = "name",
					size = "",
					domesticSale = "hhh",
					uom = "",
					quantity = "lko",
					articleRealizationOrder = "",
					domesticCOGS = "25.3658"
				};
			}
			public SPKDocsCsvViewModel GetNewData3()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocsCsvViewModel
				{
					PackingList = "",
					Password = "",
					code = "code",
					name = "name",
					size = "",
					domesticSale = "-60",
					uom = "",
					quantity = "-50",
					articleRealizationOrder = "",
					domesticCOGS = "-25"
				};
			}
			public SPKDocsCsvViewModel GetNewData4()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocsCsvViewModel
				{
					PackingList = "",
					Password = "",
					code = "code",
					name = "name",
					size = "",
					domesticSale = "265.3222",
					uom = "",
					quantity = "-50",
					articleRealizationOrder = "",
					domesticCOGS = "hjkjh"
				};
			}

			public SPKDocsCsvViewModel GetNewDataValid()
			{
				//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
				return new SPKDocsCsvViewModel
				{
					PackingList = "ss",
					Password = "ss",
					code = "code",
					name = "name",
					size = "size",
					domesticSale = "60",
					uom = "ss",
					quantity = "40",
					articleRealizationOrder = "rr",
					domesticCOGS = "25"
				};

			}

		}

		public SPKDocs GetNewDataExpedition()
		{
			//var datas = await Task.Run(() => garmentPurchaseOrderDataUtil.GetTestDataByTags());
			return new SPKDocs
			{
				Code = "codetest",
				Date = DateTimeOffset.Now,
				DestinationCode = "destinationcode1",
				DestinationName = "destinationname",
				DestinationId = 1,
				IsDistributed = false,
				IsDraft = false,
				IsReceived = false,
				PackingList = "packinglist",
				Password = "password",
				SourceCode = "SourceCode",
				SourceId = 1,
				SourceName = "SourceName",
				Weight = 1,
				Reference = "reference",
				Items = new List<SPKDocsItem>
				{
					new SPKDocsItem
					{
						ItemArticleRealizationOrder = "art1",
						ItemCode = "code",
						ItemDomesticCOGS = 0,
						ItemDomesticRetail = 0,
						ItemDomesticSale = 0,
						ItemDomesticWholesale = 0,
						ItemId = 1,
						ItemName = "name",
						ItemSize = "size",
						ItemUom =  "uom",
						Quantity = 0,
						Remark = "remark",
						SendQuantity = 0
					}
				}
			};
		}

		public async Task<SPKDocs> GetTestDataExpedition()
		{
			var data = GetNewDataExpedition();
			await pkpbjFacade.Create(data, "Unit Test");
			return data;
		}
	}
}
