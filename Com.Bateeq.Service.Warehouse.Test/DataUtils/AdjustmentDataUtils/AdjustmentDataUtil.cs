using Com.Bateeq.Service.Warehouse.Lib.Facades.AdjustmentFacade;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.AdjustmentDataUtils
{
    public class AdjustmentDataUtil
    {
        private readonly AdjustmentFacade adjustmentFacade;
        private readonly InventoryDataUtil inventoryDataUtils;

        public AdjustmentDataUtil(AdjustmentFacade facade, InventoryDataUtil inventory)
        {
            this.adjustmentFacade = facade;
            this.inventoryDataUtils = inventory;
        }

        public async Task<AdjustmentDocs> GetNewDataAsync()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestDataForAdjusment());

            return new AdjustmentDocs
            {
                Code = "codetest",
                StorageCode = datas.StorageCode,
                StorageId = datas.StorageId,
                StorageName = datas.StorageName,
                Items = new List<AdjustmentDocsItem>
                {
                    new AdjustmentDocsItem
                    {
                        ItemArticleRealizationOrder = "art1",
                        ItemCode = "itemcode1",
                        ItemName = "itemname1",
                        ItemId = 1,
                        ItemSize = "size1",
                        ItemUom = "uom1",
                        ItemDomesticCOGS = 0,
                        ItemDomesticRetail = 0,
                        ItemDomesticSale = 0,
                        ItemDomesticWholeSale = 0,
                        ItemInternationalCOGS = 0,
                        ItemInternationalRetail = 0,
                        ItemInternationalSale = 0,
                        ItemInternationalWholeSale = 0,
                        QtyAdjustment = 1,
                        QtyBeforeAdjustment = 1,
                        Type = "IN",
                        Remark = "test"

                    }
                }
            };
        }

        public async Task<AdjustmentDocs> GetNewDataAsync2()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestDataForAdjusment());

            return new AdjustmentDocs
            {
                Code = "codetest",
                StorageCode = datas.StorageCode,
                StorageId = datas.StorageId,
                StorageName = datas.StorageName,
                Items = new List<AdjustmentDocsItem>
                {
                    new AdjustmentDocsItem
                    {
                        ItemArticleRealizationOrder = "art1",
                        ItemCode = "itemcode1",
                        ItemName = "itemname1",
                        ItemId = 1,
                        ItemSize = "size1",
                        ItemUom = "uom1",
                        ItemDomesticCOGS = 0,
                        ItemDomesticRetail = 0,
                        ItemDomesticSale = 0,
                        ItemDomesticWholeSale = 0,
                        ItemInternationalCOGS = 0,
                        ItemInternationalRetail = 0,
                        ItemInternationalSale = 0,
                        ItemInternationalWholeSale = 0,
                        QtyAdjustment = 1,
                        QtyBeforeAdjustment = 1,
                        Type = "OUT",
                        Remark = "test"

                    }
                }
            };
        }

        public async Task<AdjustmentDocs> GetTestData()
        {
            var data = await GetNewDataAsync();
            await adjustmentFacade.Create(data, "Unit Test");
            return data;
        }
    }
}
