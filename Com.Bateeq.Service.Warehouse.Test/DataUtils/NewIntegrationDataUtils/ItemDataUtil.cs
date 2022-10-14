using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.NewIntegrationDataUtils
{
    public class ItemDataUtil
    {
        public List<ItemCoreViewModel> GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var instance = new ItemCoreViewModel
            {
                _id = 1,
                dataDestination = new List<ItemViewModelRead>()
                {
                    new ItemViewModelRead
                    {
                        _id = 1,
                        code = "code",
                        name = "name",
                        Description = "",
                        Uom = "Pcs",
                        Tags = "",
                        Remark = "",
                        ArticleRealizationOrder = "Ro",
                        Size = "S",
                        ImagePath = "",
                        ImgFile = ""
                    }
                },
                color = new ItemArticleColorViewModel
                {
                        _id = 1,
                        code = "code",
                        name = "name"
                },
                process = new ItemArticleProcesViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                materials = new ItemArticleMaterialViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                materialCompositions = new ItemArticleMaterialCompositionViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                collections = new ItemArticleCollectionViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                seasons = new ItemArticleSeasonViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                counters = new ItemArticleCounterViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                subCounters = new ItemArticleSubCounterViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                categories = new ItemArticleCategoryViewModel
                {
                        _id = 1,
                        code = $"code{nowTicks}",
                        name = $"name{nowTicks}"
                },
                DomesticCOGS = 0,
                DomesticWholesale = 0,
                DomesticRetail = 0,
                DomesticSale = 0,
                InternationalCOGS = 0,
                InternationalWholesale = 0,
                InternationalRetail = 0,
                InternationalSale = 0,
                price = 0,
                ImageFile = ""

            };
            var data = new List<ItemCoreViewModel>();

            data.Add(instance);
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}
