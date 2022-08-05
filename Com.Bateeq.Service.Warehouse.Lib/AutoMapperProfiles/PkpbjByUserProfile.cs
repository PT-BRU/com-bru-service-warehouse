using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.PkbjByUserViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.AutoMapperProfiles
{
    public class PkpbjByUserProfile : Profile
    {
        public PkpbjByUserProfile()
        {
            CreateMap<SPKDocsItem, PkbjByUserItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForMember(d => d.sendquantity, opt => opt.MapFrom(s => s.SendQuantity))
                .ForPath(d => d.item._id, opt => opt.MapFrom(s => s.ItemId))
                .ForPath(d => d.item.code, opt => opt.MapFrom(s => s.ItemCode))
                .ForPath(d => d.item.name, opt => opt.MapFrom(s => s.ItemName))
                .ForPath(d => d.item.domesticCOGS, opt => opt.MapFrom(s => s.ItemDomesticCOGS))
                .ForPath(d => d.item.domesticRetail, opt => opt.MapFrom(s => s.ItemDomesticRetail))
                .ForPath(d => d.item.domesticSale, opt => opt.MapFrom(s => s.ItemDomesticSale))
                .ForPath(d => d.item.domesticWholesale, opt => opt.MapFrom(s => s.ItemDomesticWholesale))
                .ForPath(d => d.item.size, opt => opt.MapFrom(s => s.ItemSize))
                .ForPath(d => d.item.uom, opt => opt.MapFrom(s => s.ItemUom))
                .ForPath(d => d.item.articleRealizationOrder, opt => opt.MapFrom(s => s.ItemArticleRealizationOrder))
                .ReverseMap();
            CreateMap<SPKDocs, PkbjByUserViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                /* Destination */
                .ForPath(d => d.destination._id, opt => opt.MapFrom(s => s.DestinationId))
                .ForPath(d => d.destination.code, opt => opt.MapFrom(s => s.DestinationCode))
                .ForPath(d => d.destination.name, opt => opt.MapFrom(s => s.DestinationName))
                /* Source */
                .ForPath(d => d.source._id, opt => opt.MapFrom(s => s.SourceId))
                .ForPath(d => d.source.code, opt => opt.MapFrom(s => s.SourceCode))
                .ForPath(d => d.source.name, opt => opt.MapFrom(s => s.SourceName))
                .ForMember(d => d.items, opt => opt.MapFrom(s => s.Items))
                .ReverseMap();
        }      
    }
}
