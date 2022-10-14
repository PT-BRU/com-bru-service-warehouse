using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferOutDataUtils
{
    public class TransferOutDataUtil
    {
        private readonly TransferOutFacade facade;
        private readonly InventoryDataUtil inventoryDataUtils;

        public TransferOutDataUtil(TransferOutFacade facade, InventoryDataUtil inventoryDataUtils)
        {
            this.facade = facade;
            this.inventoryDataUtils = inventoryDataUtils;
        }
        public async Task<TransferOutDoc> GetNewData()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
            //List<SPKDocsItem> Item = new List<SPKDocsItem>(datas.Items);
            return new TransferOutDoc
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                DestinationCode = "destCode",
                DestinationId = 1,
                DestinationName = "name",
                Reference = "reference",
                Remark = "remark",
                SourceCode = "sourceCode",
                SourceId = datas.StorageId,
                SourceName = "Name",
                Items = new List<TransferOutDocItem>
                {
                    new TransferOutDocItem
                    {
                        ArticleRealizationOrder = datas.ItemArticleRealizationOrder,
                        DomesticCOGS = datas.ItemDomesticCOGS,
                        DomesticRetail = datas.ItemDomesticRetail,
                        DomesticSale = datas.ItemDomesticSale,
                        DomesticWholeSale = datas.ItemDomesticWholeSale,
                        ItemCode = datas.ItemCode,
                        ItemId = datas.ItemId,
                        ItemName = datas.ItemName,
                        Quantity = datas.Quantity,
                        Remark = "Remark",
                        Size = datas.ItemSize,

                    }
                }
            };
 
        }
        public async Task<TransferOutDoc> GetNewDataForTransfer()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
            var data2 = await Task.Run(() => inventoryDataUtils.GetTestDataForTransfer());
            //List<SPKDocsItem> Item = new List<SPKDocsItem>(datas.Items);
            return new TransferOutDoc
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                DestinationCode = "destCode",
                DestinationId = 1,
                DestinationName = "name",
                Reference = "reference",
                Remark = "remark",
                SourceCode = "sourceCode",
                SourceId = datas.StorageId,
                SourceName = "Name",
                Items = new List<TransferOutDocItem>
                {
                    new TransferOutDocItem
                    {
                        ArticleRealizationOrder = datas.ItemArticleRealizationOrder,
                        DomesticCOGS = datas.ItemDomesticCOGS,
                        DomesticRetail = datas.ItemDomesticRetail,
                        DomesticSale = datas.ItemDomesticSale,
                        DomesticWholeSale = datas.ItemDomesticWholeSale,
                        ItemCode = datas.ItemCode,
                        ItemId = datas.ItemId,
                        ItemName = datas.ItemName,
                        Quantity = datas.Quantity,
                        Remark = "Remark",
                        Size = datas.ItemSize,

                    }
                }
            };

        }
        public TransferOutDocViewModel MapToViewModel (TransferOutDoc transferOutDoc)
        {
            List<TransferOutDocItem> transferOutDocItems = new List<TransferOutDocItem>(transferOutDoc.Items);
            return new TransferOutDocViewModel
            {
                code = transferOutDoc.Code,
                date = transferOutDoc.Date,
                expeditionService = new Lib.ViewModels.NewIntegrationViewModel.ExpeditionServiceViewModel
                {
                    code = "expCode",
                    name = "expName",
                    _id = 0
                },
                destination = new Lib.ViewModels.NewIntegrationViewModel.DestinationViewModel
                {
                    code = transferOutDoc.DestinationCode,
                    name = transferOutDoc.DestinationName,
                    _id = transferOutDoc.DestinationId
                },
                remark = transferOutDoc.Remark,
                reference = transferOutDoc.Reference,
                source = new Lib.ViewModels.NewIntegrationViewModel.SourceViewModel
                {
                    code = transferOutDoc.SourceCode,
                    name = transferOutDoc.SourceName,
                    _id = transferOutDoc.SourceId
                },
                items = new List<TransferOutDocItemViewModel>
                {
                    new TransferOutDocItemViewModel
                    {
                        articleRealizationOrder = transferOutDocItems[0].ArticleRealizationOrder,
                        quantity = transferOutDocItems[0].Quantity,
                        remark = transferOutDocItems[0].Remark,
                        item = new Lib.ViewModels.NewIntegrationViewModel.ItemViewModel{
                            articleRealizationOrder = transferOutDocItems[0].ArticleRealizationOrder,
                            code = transferOutDocItems[0].ItemCode,
                            domesticCOGS = transferOutDocItems[0].DomesticCOGS,
                            domesticRetail = transferOutDocItems[0].DomesticRetail,
                            domesticSale = transferOutDocItems[0].DomesticSale,
                            domesticWholesale = transferOutDocItems[0].DomesticWholeSale,
                            name = transferOutDocItems[0].ItemName,
                            size = transferOutDocItems[0].Size,
                            uom = transferOutDocItems[0].Uom,
                            _id = transferOutDocItems[0].ItemId,
                        },

                    }
                }
            };
        }
        public async Task<TransferOutDoc> GetTestData()
        {
            var data = await GetNewData();
            var viewmodel = MapToViewModel(data);
            await facade.Create(viewmodel,data, "Unit Test");
            return data;
        }
        public async Task<TransferOutDoc> GetTestDataForTransfer()
        {
            var data = await GetNewDataForTransfer();
            var viewmodel = MapToViewModel(data);
            await facade.Create(viewmodel, data, "Unit Test");
            return data;
        }
    }
}
