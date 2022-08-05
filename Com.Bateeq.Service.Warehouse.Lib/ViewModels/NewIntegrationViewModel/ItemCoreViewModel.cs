using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel
{
    public class ItemCoreViewModel
    {
        public int _id { get; set; }
        public List<ItemViewModelRead> dataDestination { get; set; }

        public ItemArticleColorViewModel color { get; set; }


        public ItemArticleProcesViewModel process { get; set; }


        public ItemArticleMaterialViewModel materials { get; set; }


        public ItemArticleMaterialCompositionViewModel materialCompositions { get; set; }


        public ItemArticleCollectionViewModel collections { get; set; }


        public ItemArticleSeasonViewModel seasons { get; set; }

        public ItemArticleCounterViewModel counters { get; set; }


        public ItemArticleSubCounterViewModel subCounters { get; set; }


        public ItemArticleCategoryViewModel categories { get; set; }

        public double? DomesticCOGS { get; set; }

        public double? DomesticWholesale { get; set; }

        public double? DomesticRetail { get; set; }

        public double? DomesticSale { get; set; }

        public double? InternationalCOGS { get; set; }

        public double? InternationalWholesale { get; set; }

        public double? InternationalRetail { get; set; }

        public double? InternationalSale { get; set; }
        public double? price { get; set; }
        public string ImageFile { get; set; }
    }
    public class ItemArticleColorViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleProcesViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleMaterialViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleMaterialCompositionViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleCollectionViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleSeasonViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleCounterViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleSubCounterViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }

    public class ItemArticleCategoryViewModel
    {
        public int _id { get; set; }

        public string name { get; set; }

        public string code { get; set; }
    }
    public class ItemViewModelRead
    {
        public int _id { get; set; }
        public string code { get; set; }

        public string name { get; set; }

        public string Description { get; set; }

        public string Uom { get; set; }

        public string Tags { get; set; }

        public string Remark { get; set; }

        public string ArticleRealizationOrder { get; set; }

        public string Size { get; set; }

        public string ImagePath { get; set; }

        public string ImgFile { get; set; }
    }

    public class ItemCustomViewModel
    {
        public int _id { get; set; }
        public string code { get; set; }

        public string name { get; set; }

        public string Description { get; set; }

        public string Uom { get; set; }

        public string Tags { get; set; }

        public string Remark { get; set; }

        public string ArticleRealizationOrder { get; set; }

        public string Size { get; set; }

        public string ImagePath { get; set; }

        public string ImgFile { get; set; }

        public double? DomesticCOGS { get; set; }

        public double? DomesticWholesale { get; set; }

        public double? DomesticRetail { get; set; }

        public double? DomesticSale { get; set; }

        public double? InternationalCOGS { get; set; }

        public double? InternationalWholesale { get; set; }

        public double? InternationalRetail { get; set; }

        public double? InternationalSale { get; set; }
    }
}
