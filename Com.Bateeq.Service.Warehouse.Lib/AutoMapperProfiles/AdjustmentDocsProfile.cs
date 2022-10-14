using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Models.AdjustmentDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.AdjustmentDocsViewModel;

namespace Com.Bateeq.Service.Warehouse.Lib.AutoMapperProfiles
{
    public class AdjustmentDocsProfile : Profile
    {
        public AdjustmentDocsProfile()
        {
            CreateMap<AdjustmentDocsItem, AdjustmentDocsItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForPath(d => d.item._id, opt => opt.MapFrom(s => s.ItemId))
                .ForPath(d => d.item.code, opt => opt.MapFrom(s => s.ItemCode))
                .ForPath(d => d.item.name, opt => opt.MapFrom(s => s.ItemName))
                .ForPath(d => d.item.articleRealizationOrder, opt => opt.MapFrom(s => s.ItemArticleRealizationOrder))
                .ForPath(d => d.item.domesticCOGS, opt => opt.MapFrom(s => s.ItemDomesticCOGS))
                .ForPath(d => d.item.domesticRetail, opt => opt.MapFrom(s => s.ItemDomesticRetail))
                .ForPath(d => d.item.domesticSale, opt => opt.MapFrom(s => s.ItemDomesticSale))
                .ForPath(d => d.item.domesticWholesale, opt => opt.MapFrom(s => s.ItemDomesticWholeSale))
                .ForPath(d => d.item.size, opt => opt.MapFrom(s => s.ItemSize))
                .ForPath(d => d.item.uom, opt => opt.MapFrom(s => s.ItemUom))
                .ForMember(d => d.qtyBeforeAdjustment, opt => opt.MapFrom(s => s.QtyBeforeAdjustment))
                .ForMember(d => d.qtyAdjustment, opt => opt.MapFrom(s => s.QtyAdjustment))
                .ForMember(d => d.remark, opt => opt.MapFrom(s => s.Remark))
                .ForMember(d => d.type, opt => opt.MapFrom(s => s.Type))
                .ReverseMap();

            CreateMap<AdjustmentDocs, AdjustmentDocsViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.code, opt => opt.MapFrom(s => s.Code))
                .ForPath(d => d.storage.code, opt => opt.MapFrom(s => s.StorageCode))
                .ForPath(d => d.storage.name, opt => opt.MapFrom(s => s.StorageName))
                .ForPath(d => d.storage._id, opt => opt.MapFrom(s => s.StorageId))
                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                .ReverseMap();
        }
    }
}
