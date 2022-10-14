using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.ExpeditionDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferDataUtils
{
    public class TransferInStoreDataUtil
    {
        private readonly TransferFacade facade;
        private readonly ExpeditionDataUtil expeditionDataUtils;

        public TransferInStoreDataUtil(TransferFacade facade, ExpeditionDataUtil expeditionDataUtils)
        {
            this.facade = facade;
            this.expeditionDataUtils = expeditionDataUtils;
        }

        public async Task<TransferInDoc> GetNewData()
        {
            var datae = await Task.Run(() => expeditionDataUtils.GetTestData2());

            List<ExpeditionItem> Item = new List<ExpeditionItem>(datae.Items);
            List<ExpeditionDetail> Detail = new List<ExpeditionDetail>(Item[0].Details);
            
            return new TransferInDoc
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                DestinationCode = "destinationCode1",
                DestinationId = 1,
                DestinationName = "destName1",
                Reference = Item[0].PackingList,
                Remark = "remark",
                SourceCode = "SorceCode",
                SourceId = 1,
                SourceName = "SorceName",
                Items = new List<TransferInDocItem>
                {
                    new TransferInDocItem
                    {
                        ArticleRealizationOrder = Detail[0].ArticleRealizationOrder,
                        DomesticCOGS = Detail[0].DomesticCOGS,
                        DomesticRetail = Detail[0].DomesticRetail,
                        DomesticSale = Detail[0].DomesticSale,
                        DomesticWholeSale = Detail[0].DomesticWholesale,
                        ItemCode = Detail[0].ItemCode,
                        ItemId = Detail[0].ItemId,
                        ItemName = Detail[0].ItemName,
                        Quantity = Detail[0].Quantity,
                        Remark = Detail[0].Remark,
                        Size = Detail[0].Size,
                        Uom = Detail[0].Uom
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
    }
}
