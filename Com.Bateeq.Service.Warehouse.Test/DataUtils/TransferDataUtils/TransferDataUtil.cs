using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferDataUtils
{
    public class TransferDataUtil
    {
        private readonly TransferFacade facade;
        private readonly SPKDocDataUtil sPKDocDataUtils;

        public TransferDataUtil(TransferFacade facade, SPKDocDataUtil sPKDocDataUtils)
        {
            this.facade = facade;
            this.sPKDocDataUtils = sPKDocDataUtils;
        }

        public async Task<TransferInDoc> GetNewData()
        {
            var datas = await Task.Run(() => sPKDocDataUtils.GetTestData());
            List<SPKDocsItem> Item = new List<SPKDocsItem>(datas.Items);
            return new TransferInDoc
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                DestinationCode = "destinationCode1",
                DestinationId = 1,
                DestinationName = "destName1",
                Reference = datas.PackingList,
                Remark = "remark",
                SourceCode = "SorceCode",
                SourceId = 1,
                SourceName = "SorceName",
                Items = new List<TransferInDocItem>
                {
                    new TransferInDocItem
                    {
                        ArticleRealizationOrder = Item[0].ItemArticleRealizationOrder,
                        DomesticCOGS = Item[0].ItemDomesticCOGS,
                        DomesticRetail = Item[0].ItemDomesticRetail,
                        DomesticSale = Item[0].ItemDomesticSale,
                        DomesticWholeSale = Item[0].ItemDomesticWholesale,
                        ItemCode = Item[0].ItemCode,
                        ItemId =Item[0].ItemId,
                        ItemName = Item[0].ItemName,
                        Quantity = Item[0].Quantity,
                        Remark = Item[0].Remark,
                        Size = Item[0].ItemSize,
                        Uom = Item[0].ItemUom
                    }
                }
            };
        }

        
        public async Task<TransferInDoc> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }
        
        public async Task<TransferInDoc> GetNewData2()
        {
            //var datas = await Task.Run(() => sPKDocDataUtils.GetTestData());
            //List<SPKDocsItem> Item = new List<SPKDocsItem>(datas.Items);
            return new TransferInDoc
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                DestinationCode = "destinationCode1",
                DestinationId = 1,
                DestinationName = "destName1",
                Reference = "0095/EFR-FN/08/21",
                Remark = "remark",
                SourceCode = "SorceCode",
                SourceId = 1,
                SourceName = "SorceName",
                Items = new List<TransferInDocItem>
                {
                    new TransferInDocItem
                    {
                        ArticleRealizationOrder = "2110003",
                        DomesticCOGS = 40000,
                        DomesticRetail = 0,
                        DomesticSale = 40010,
                        DomesticWholeSale = 0,
                        ItemCode = "111",
                        ItemId =60482,
                        ItemName = "BOYS VEST",
                        Quantity = 1,
                        Remark = "",
                        Size = "S",
                        Uom = "PCS"
                    }
                }
            };
        }

    }
}
