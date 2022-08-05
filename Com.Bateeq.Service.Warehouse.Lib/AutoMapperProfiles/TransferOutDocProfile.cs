using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.AutoMapperProfiles
{
    public class TransferOutDocProfile : Profile
    {
        public TransferOutDocProfile()
        {
            CreateMap<TransferOutDocItem, TransferOutDocItemViewModel>()
                .ForMember(d => d._id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForMember(d => d.remark, opt => opt.MapFrom(s => s.Remark))
                .ForPath(d => d.item._id, opt => opt.MapFrom(s => s.ItemId))
                .ForPath(d => d.item.code, opt => opt.MapFrom(s => s.ItemCode))
                .ForPath(d => d.item.name, opt => opt.MapFrom(s => s.ItemName))

                .ForPath(d => d.item.domesticCOGS, opt => opt.MapFrom(s => s.DomesticCOGS))
                .ForPath(d => d.item.domesticRetail, opt => opt.MapFrom(s => s.DomesticRetail))
                .ForPath(d => d.item.domesticSale, opt => opt.MapFrom(s => s.DomesticSale))
                .ForPath(d => d.item.domesticWholesale, opt => opt.MapFrom(s => s.DomesticWholeSale))
                .ForPath(d => d.item.size, opt => opt.MapFrom(s => s.Size))
                .ForPath(d => d.item.uom, opt => opt.MapFrom(s => s.Uom))
                .ForPath(d => d.item.articleRealizationOrder, opt => opt.MapFrom(s => s.ArticleRealizationOrder))
                .ReverseMap();

            CreateMap<TransferOutDoc, TransferOutDocViewModel>()
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
