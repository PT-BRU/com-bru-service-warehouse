using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Models.SOModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.SODataUtils
{
    public class SODataUtil
    {
        private readonly SOFacade facade;
        private readonly InventoryDataUtil inventoryDataUtils;

        public SODataUtil(SOFacade facade, InventoryDataUtil inventoryDataUtils)
        {
            this.facade = facade;
            this.inventoryDataUtils = inventoryDataUtils;
        }
        public async Task<SODocs> GetNewData()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
            return new SODocs
            {
                Code = "code",
                StorageId=datas.StorageId,
                StorageName="name",
                Items = new List<SODocsItem>
                {
                    new SODocsItem
                    {
                        ItemCode = datas.ItemCode,
                        ItemId = datas.ItemId,
                        ItemName = "name",
                        Remark = "Remark",

                    }
                }
            };

        }

        //public async Task<TransferOutDoc> GetNewDataForTransfer()
        //{
        //    var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
        //    var data2 = await Task.Run(() => inventoryDataUtils.GetTestDataForTransfer());
        //    //List<SPKDocsItem> Item = new List<SPKDocsItem>(datas.Items);
        //    return new TransferOutDoc
        //    {
        //        Code = "code",
        //        Date = DateTimeOffset.Now,
        //        DestinationCode = "destCode",
        //        DestinationId = 1,
        //        DestinationName = "name",
        //        Reference = "reference",
        //        Remark = "remark",
        //        SourceCode = "sourceCode",
        //        SourceId = datas.StorageId,
        //        SourceName = "Name",
        //        Items = new List<TransferOutDocItem>
        //        {
        //            new TransferOutDocItem
        //            {
        //                ArticleRealizationOrder = datas.ItemArticleRealizationOrder,
        //                DomesticCOGS = datas.ItemDomesticCOGS,
        //                DomesticRetail = datas.ItemDomesticRetail,
        //                DomesticSale = datas.ItemDomesticSale,
        //                DomesticWholeSale = datas.ItemDomesticWholeSale,
        //                ItemCode = datas.ItemCode,
        //                ItemId = datas.ItemId,
        //                ItemName = datas.ItemName,
        //                Quantity = datas.Quantity,
        //                Remark = "Remark",
        //                Size = datas.ItemSize,

        //            }
        //        }
        //    };

        //}

        public async Task<SODocs> GetTestData()
        {
            var data = await GetNewData();
            await facade.UploadData(data, "Unit Test");
            return data;
        }
        public async Task<SODocs> GetTestDataIsAdjusted()
        {
            var data = await GetNewData();
            data.Items.First().IsAdjusted = true;
            data.Items.First().SODocsId = data.Id;
            data.Items.First().QtyBeforeSO =3;
            data.Items.First().QtySO = 5;

            await facade.UploadData(data, "Unit Test");
            return data;
        }

        public async Task<SODocs> GetTestDataIsAdjustedQty()
        {
            var data = await GetNewData();
            data.Items.First().IsAdjusted = true;
            data.Items.First().SODocsId = data.Id;

            await facade.UploadData(data, "Unit Test");
            return data;
        }
    }
}
