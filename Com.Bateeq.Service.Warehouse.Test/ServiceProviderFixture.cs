using Com.Bateeq.Service.Warehouse.Lib;
//using Com.MM.Service.Warehouse.Lib.Facades;
//using Com.MM.Service.Warehouse.Lib.Facades.InternalPO;
//using Com.MM.Service.Warehouse.Lib.Facades.Expedition;
//using Com.MM.Service.Warehouse.Lib.Facades.Report;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Serializers;
using Com.Bateeq.Service.Warehouse.Lib.Services;
//using Com.MM.Service.Warehouse.Lib.ViewModels.IntegrationViewModel;
//using Com.MM.Service.Warehouse.Lib.ViewModels.PurchaseOrder;
//using Com.MM.Service.Warehouse.Lib.ViewModels.UnitReceiptNote;
//using Com.MM.Service.Warehouse.Test.DataUtils.ExpeditionDataUtil;
//using Com.MM.Service.Warehouse.Test.DataUtils.PurchaseRequestDataUtils;
//using Com.MM.Service.Warehouse.Test.DataUtils.InternalPurchaseOrderDataUtils;
//using Com.MM.Service.Warehouse.Test.DataUtils.UnitReceiptNote;
using Com.Bateeq.Service.Warehouse.Test.Helpers;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using Xunit;
//using Com.MM.Service.Warehouse.Lib.Facades.ExternalPurchaseOrderFacade;
//using Com.MM.Service.Warehouse.Test.DataUtils.ExternalPurchaseOrderDataUtils;
//using Com.MM.Service.Warehouse.Test.DataUtils.DeliveryOrderDataUtils;
//using Com.MM.Service.Warehouse.Lib.Facades.BankExpenditureNoteFacades;
//using Com.MM.Service.Warehouse.Test.DataUtils.BankExpenditureNoteDataUtils;
//using Com.MM.Service.Warehouse.Lib.Facades.UnitReceiptNoteFacade;
//using Com.MM.Service.Warehouse.Test.DataUtils.UnitReceiptNoteDataUtils;
//using Com.MM.Service.Warehouse.Lib.ViewModels.Expedition;
//using Com.MM.Service.Warehouse.Lib.Facades.ExternalPurchaseOrderFacade.Reports;
//using Com.MM.Service.Warehouse.Lib.Facades.UnitPaymentCorrectionNoteFacade;
//using Com.MM.Service.Warehouse.Test.DataUtils.UnitPaymentCorrectionNoteDataUtils;
//using Com.MM.Service.Warehouse.Test.DataUtils.UnitPaymentOrderDataUtils;
//using Com.MM.Service.Warehouse.Lib.Facades.GarmentInvoiceFacades;
//using Com.MM.Service.Warehouse.Test.DataUtils.GarmentInvoiceDataUtils;
//using Com.MM.Service.Warehouse.Test.DataUtils.GarmentInternNoteDataUtils;
//using Com.MM.Service.Warehouse.Lib.Facades.GarmentInternNoteFacades;
//using Com.MM.Service.Warehouse.Lib.Facades.PurchaseRequestFacades;
//using Com.MM.Service.Warehouse.Lib.Facades.MonitoringCentralBillReceptionFacades;
//using Com.MM.Service.Warehouse.Lib.Facades.MonitoringCentralBillExpenditureFacades;
//using Com.MM.Service.Warehouse.Lib.Facades.MonitoringCorrectionNoteReceptionFacades;
//using Com.MM.Service.Warehouse.Lib.Facades.MonitoringCorrectionNoteExpenditureFacades;
using Com.Bateeq.Service.Warehouse.Lib.Utilities.Currencies;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.TransferDataUtils;
using Com.Bateeq.Service.Warehouse.Test.DataUtils.SPKDocDataUtils;

namespace Com.Bateeq.Service.Warehouse.Test
{
    public class ServiceProviderFixture
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private void RegisterEndpoints(IConfigurationRoot Configuration)
        {
            APIEndpoint.Purchasing = Configuration.GetValue<string>(Constant.PURCHASING_ENDPOINT) ?? Configuration[Constant.PURCHASING_ENDPOINT];
        }

        private void RegisterSerializationProvider()
        {
            BsonSerializer.RegisterSerializationProvider(new SerializationProvider());
        }

        //private void RegisterClassMap()
        //{
        //    ClassMap<UnitReceiptNoteViewModel>.Register();
        //    ClassMap<UnitReceiptNoteItemViewModel>.Register();
        //    ClassMap<UnitViewModel>.Register();
        //    ClassMap<DivisionViewModel>.Register();
        //    ClassMap<CategoryViewModel>.Register();
        //    ClassMap<ProductViewModel>.Register();
        //    ClassMap<UomViewModel>.Register();
        //    ClassMap<PurchaseOrderViewModel>.Register();
        //    ClassMap<SupplierViewModel>.Register();
        //    ClassMap<UnitPaymentOrderUnpaidViewModel>.Register();
        //}

        public ServiceProviderFixture()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(Constant.SECRET, "DANLIRISDEVELOPMENT"),
                    new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", "Test"),
                    //new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=(localdb)\\mssqllocaldb;Database=com-danliris-db-test;Trusted_Connection=True;MultipleActiveResultSets=true"),
                    new KeyValuePair<string, string>(Constant.DEFAULT_CONNECTION, "Server=localhost,1401;Database=com.bateeq.db.warehouse.service.test;User Id=sa;Password=Standar123.;MultipleActiveResultSets=True;"),
                    new KeyValuePair<string, string>(Constant.MONGODB_CONNECTION, "mongodb://localhost:27017/admin")
                })
                .Build();

            RegisterEndpoints(configuration);
            string connectionString = configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? configuration[Constant.DEFAULT_CONNECTION];
			APIEndpoint.ConnectionString = configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? configuration[Constant.DEFAULT_CONNECTION];

			this.ServiceProvider = new ServiceCollection()
                .AddDbContext<WarehouseDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Transient)
                .AddTransient<TransferFacade>()
                //.AddTransient<SPKDocFacade>()
                .AddTransient<PkpbjFacade>()
                .AddTransient<TransferDataUtil>()
                .AddTransient<SPKDocDataUtil>()

                //            .AddTransient<UnitPaymentOrderUnpaidReportFacade>()
                //            .AddTransient<UnitPaymentOrderNotVerifiedReportFacade>()
                //            .AddTransient<PurchasingDocumentAcceptanceDataUtil>()
                //            .AddTransient<UnitReceiptNoteBsonDataUtil>()
                //            .AddTransient<UnitPaymentOrderUnpaidReportDataUtil>()
                //            .AddTransient<UnitReceiptNoteImportFalseBsonDataUtil>()

                //            .AddTransient<PurchaseRequestFacade>()
                //            .AddTransient<PurchaseRequestDataUtil>()
                //            .AddTransient<PurchaseRequestItemDataUtil>()
                //            .AddTransient<PurchaseOrderMonitoringAllFacade>()
                //            .AddTransient<MonitoringPriceFacade>()
                //            .AddTransient<PurchaseRequestGenerateDataFacade>()
                //            .AddTransient<ExternalPurchaseOrderGenerateDataFacade>()
                //            .AddTransient<UnitReceiptNoteGenerateDataFacade>()

                //            .AddTransient<InternalPurchaseOrderFacade>()
                //            .AddTransient<InternalPurchaseOrderDataUtil>()
                //            .AddTransient<InternalPurchaseOrderItemDataUtil>()

                //            .AddTransient<ExternalPurchaseOrderFacade>()
                //            .AddTransient<ExternalPurchaseOrderDataUtil>()
                //            .AddTransient<ExternalPurchaseOrderItemDataUtil>()
                //            .AddTransient<ExternalPurchaseOrderDetailDataUtil>()

                //            .AddTransient<DeliveryOrderFacade>()
                //            .AddTransient<IDeliveryOrderFacade, DeliveryOrderFacade>()
                //            .AddTransient<DeliveryOrderDataUtil>()
                //            .AddTransient<DeliveryOrderItemDataUtil>()
                //            .AddTransient<DeliveryOrderDetailDataUtil>()

                //            .AddTransient<BankExpenditureNoteFacade>()
                //            .AddTransient<BankExpenditureNoteDataUtil>()

                //            //.AddTransient<UnitReceiptNoteFacade>()
                //            .AddTransient<UnitReceiptNoteFacade>()
                //            .AddTransient<UnitReceiptNoteDataUtil>()
                //            .AddTransient<UnitReceiptNoteItemDataUtil>()
                //.AddTransient<TotalPurchaseFacade>()
                //.AddTransient<ImportPurchasingBookReportFacade>()
                //.AddTransient<IGarmentInvoice,GarmentInvoiceFacade>()
                //.AddTransient<GarmentInvoiceDataUtil>()
                //.AddTransient<GarmentInvoiceItemDataUtil>()
                //.AddTransient<GarmentInvoiceDetailDataUtil>()

                //            .AddTransient<GarmentInternNoteFacades>()
                //            .AddTransient<GarmentInternNoteDataUtil>()

                //            .AddTransient<IUnitPaymentOrderFacade, UnitPaymentOrderFacade>()
                //            .AddTransient<UnitPaymentOrderDataUtil>()
                //            .AddTransient<IUnitPaymentPriceCorrectionNoteFacade, UnitPaymentPriceCorrectionNoteFacade>()
                //            .AddTransient<IMonitoringCentralBillReceptionFacade, MonitoringCentralBillReceptionFacade>()
                //            .AddTransient<IMonitoringCentralBillExpenditureFacade, MonitoringCentralBillExpenditureFacade>()
                //            .AddTransient<IMonitoringCorrectionNoteReceptionFacade, MonitoringCorrectionNoteReceptionFacade>()
                //            .AddTransient<IMonitoringCorrectionNoteExpenditureFacade, MonitoringCorrectionNoteExpenditureFacade>()
                //            .AddTransient<UnitPaymentPriceCorrectionNoteDataUtils>()
                //            .AddTransient<UnitPaymentCorrectionNoteDataUtil>()
                .AddSingleton<IHttpClientService, HttpClientTestService>()
				.AddSingleton<ICurrencyProvider, CurrencyProvider>()
                .AddSingleton<IdentityService>()
                .BuildServiceProvider();

            RegisterSerializationProvider();
            //RegisterClassMap();
            //MongoDbContext.connectionString = configuration.GetConnectionString(Constant.MONGODB_CONNECTION) ?? configuration[Constant.MONGODB_CONNECTION];

            WarehouseDbContext dbContext = ServiceProvider.GetService<WarehouseDbContext>();
            if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                dbContext.Database.Migrate();
            }
        }     
    }

    [CollectionDefinition("ServiceProviderFixture Collection")]
    public class ServiceProviderFixtureCollection : ICollectionFixture<ServiceProviderFixture>
    {
    }
}
