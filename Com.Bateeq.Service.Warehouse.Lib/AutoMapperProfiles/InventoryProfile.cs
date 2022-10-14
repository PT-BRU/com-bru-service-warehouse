using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib.Models.InventoryModel;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.InventoryViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.AutoMapperProfiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile() {
            CreateMap<Inventory, InventoryViewModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.itemInternationalCOGS, opt => opt.MapFrom(s => s.ItemInternationalCOGS))
                .ForMember(d => d.itemInternationalRetail, opt => opt.MapFrom(s => s.ItemInternationalRetail))
                .ForMember(d => d.itemInternationalSale, opt => opt.MapFrom(s => s.ItemInternationalSale))
                .ForMember(d => d.itemInternationalWholeSale, opt => opt.MapFrom(s => s.ItemInternationalWholeSale))
                .ForMember(d => d.quantity, opt => opt.MapFrom(s => s.Quantity))
                .ForPath(d => d.item._id, opt => opt.MapFrom(s => s.ItemId))
                .ForPath(d => d.item.code, opt => opt.MapFrom(s => s.ItemCode))
                .ForPath(d => d.item.name, opt => opt.MapFrom(s => s.ItemName))
                .ForPath(d => d.item.domesticCOGS, opt => opt.MapFrom(s => s.ItemDomesticCOGS))
                .ForPath(d => d.item.domesticRetail, opt => opt.MapFrom(s => s.ItemDomesticRetail))
                .ForPath(d => d.item.domesticSale, opt => opt.MapFrom(s => s.ItemDomesticSale))
                .ForPath(d => d.item.size, opt => opt.MapFrom(s => s.ItemSize))
                .ForPath(d => d.item.uom, opt => opt.MapFrom(s => s.ItemUom))
                .ForPath(d => d.item.articleRealizationOrder, opt => opt.MapFrom(s => s.ItemArticleRealizationOrder))
                .ForPath(d => d.storage._id, opt => opt.MapFrom(s => s.StorageId))
                .ForPath(d => d.storage.code, opt => opt.MapFrom(s => s.StorageCode))
                .ForPath(d => d.storage.name, opt => opt.MapFrom(s => s.StorageName))
                .ForPath(d => d.storage.isCentral, opt => opt.MapFrom(s => s.StorageIsCentral))
                
                .ReverseMap();
        }
    }
}
