using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Lib.Models.Expeditions;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.InventoryDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.ExpeditionDataUtils
{
    public class ExpeditionDataUtil
    {
        private readonly ExpeditionFacade facade;
        private readonly InventoryDataUtil inventoryDataUtils;
        private readonly SPKDocDataUtil sPKDocDataUtils;

        public ExpeditionDataUtil(ExpeditionFacade facade, InventoryDataUtil inventoryDataUtils, SPKDocDataUtil sPKDocDataUtils)
        {
            this.facade = facade;
            this.inventoryDataUtils = inventoryDataUtils;
            this.sPKDocDataUtils = sPKDocDataUtils;
            this.facade = facade;
        }

        public async Task<Expedition> GetNewData()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
            var dataSPK = await Task.Run(() => sPKDocDataUtils.GetTestData());
            List<SPKDocsItem> Item = new List<SPKDocsItem>(dataSPK.Items);
            return new Expedition
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                ExpeditionServiceCode = "codeex",
                ExpeditionServiceName = "exname",
                ExpeditionServiceId = 1,
                Remark = "remark",
                Weight = 2,
                Items = new List<ExpeditionItem>
                {
                    new ExpeditionItem
                    {
                        Code = dataSPK.Code,
                        Date = dataSPK.Date,
                        DestinationCode = dataSPK.DestinationCode,
                        DestinationName = dataSPK.DestinationName,
                        DestinationId = (int)dataSPK.DestinationId,
                        IsDistributed = true,
                        IsReceived = false,
                        PackingList = dataSPK.PackingList,
                        Password = dataSPK.Password,
                        SourceCode = dataSPK.SourceCode,
                        Reference = dataSPK.Reference,
                        SourceId =(int)dataSPK.SourceId,
						SourceName = dataSPK.SourceName,
                        SPKDocsId = (int)dataSPK.Id,
                        Weight = dataSPK.Weight,
                        Details = new List<ExpeditionDetail>
                        {
                            new ExpeditionDetail
                            {
                                ArticleRealizationOrder = Item[0].ItemArticleRealizationOrder,
                                DomesticCOGS = Item[0].ItemDomesticCOGS,
                                DomesticRetail = Item[0].ItemDomesticRetail,
                                DomesticSale = Item[0].ItemDomesticSale,
                                DomesticWholesale = Item[0].ItemDomesticWholesale,
                                ItemCode = Item[0].ItemCode,
                                ItemName = Item[0].ItemName,
                                ItemId = Item[0].ItemId,
                                Quantity = Item[0].Quantity,
                                Remark = Item[0].Remark,
                                SendQuantity =Item[0].SendQuantity,
                                Size = Item[0].ItemSize,
                                Uom = Item[0].ItemUom,
                                SPKDocsId = (int)dataSPK.Id,
								
                                
                            }
                        }

                    }
                }
            };

        }

        public async Task<Expedition> GetTestData()
        {
            var data = await GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }

        public async Task<Expedition> GetNewData2()
        {
            var datas = await Task.Run(() => inventoryDataUtils.GetTestData());
            var dataSPK = await Task.Run(() => sPKDocDataUtils.GetTestDataExpedition());
            List<SPKDocsItem> Item = new List<SPKDocsItem>(dataSPK.Items);
            return new Expedition
            {
                Code = "code",
                Date = DateTimeOffset.Now,
                ExpeditionServiceCode = "codeex",
                ExpeditionServiceName = "exname",
                ExpeditionServiceId = 1,
                Remark = "remark",
                Weight = 2,
                Items = new List<ExpeditionItem>
                {
                    new ExpeditionItem
                    {
                        Code = dataSPK.Code,
                        Date = dataSPK.Date,
                        DestinationCode = dataSPK.DestinationCode,
                        DestinationName = dataSPK.DestinationName,
                        DestinationId = (int)dataSPK.DestinationId,
                        IsDistributed = true,
                        IsReceived = false,
                        PackingList = dataSPK.PackingList,
                        Password = dataSPK.Password,
                        SourceCode = dataSPK.SourceCode,
                        Reference = dataSPK.Reference,
                        SourceId =(int)dataSPK.SourceId,
                        SourceName = dataSPK.SourceName,
                        SPKDocsId = (int)dataSPK.Id,
                        Weight = dataSPK.Weight,
                        Details = new List<ExpeditionDetail>
                        {
                            new ExpeditionDetail
                            {
                                ArticleRealizationOrder = Item[0].ItemArticleRealizationOrder,
                                DomesticCOGS = Item[0].ItemDomesticCOGS,
                                DomesticRetail = Item[0].ItemDomesticRetail,
                                DomesticSale = Item[0].ItemDomesticSale,
                                DomesticWholesale = Item[0].ItemDomesticWholesale,
                                ItemCode = Item[0].ItemCode,
                                ItemName = Item[0].ItemName,
                                ItemId = Item[0].ItemId,
                                Quantity = Item[0].Quantity,
                                Remark = Item[0].Remark,
                                SendQuantity =Item[0].SendQuantity,
                                Size = Item[0].ItemSize,
                                Uom = Item[0].ItemUom,
                                SPKDocsId = (int)dataSPK.Id,


                            }
                        }

                    }
                }
            };

        }

        public async Task<Expedition> GetTestData2()
        {
            var data = await GetNewData2();
            await facade.Create(data, "Unit Test");
            return data;
        }
    }
}
