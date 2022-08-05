using AutoMapper;
using Com.Bateeq.Service.Warehouse.Lib;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Serializers;
using Com.Bateeq.Service.Warehouse.Lib.Services;
//using Com.DanLiris.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
//using Com.DanLiris.Service.Purchasing.Lib.ViewModels.PurchaseOrder;
//using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
//using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Serialization;
using System.Text;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacade.Reports;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderReturFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCentralBillReceptionFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCentralBillExpenditureFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteReceptionFacades;
using FluentScheduler;
using Com.Bateeq.Service.Warehouse.WebApi.SchedulerJobs;
using Com.Bateeq.Service.Warehouse.Lib.Utilities.CacheManager;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteExpenditureFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.Bateeq.Service.Warehouse.Lib.Utilities.Currencies;
//using Com.MM.Service.Warehouse.Lib.Facades.GarmentDailyPurchasingReportFacade;
//using Com.MM.Service.Warehouse.Lib.AutoMapperProfiles;
using Com.Bateeq.Service.Warehouse.Lib.Utilities;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using Com.Bateeq.Service.Warehouse.Lib.Facades;
using System;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.TransferInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SPKInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Facades.Stores;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Facades.AdjustmentFacade;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.AdjustmentInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.SOInterfaces;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.InventoryLoaderInterfaces;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.PRMasterValidationReportFacade;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades.Reports;
//using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentSupplierBalanceDebtFacades;

namespace Com.Bateeq.Service.Warehouse.WebApi
{
    public class Startup
    {
        /* Hard Code */
        private string[] EXPOSED_HEADERS = new string[] { "Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time" };
        private string PURCHASING_POLICITY = "PurchasingPolicy";

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Register

        private void RegisterEndpoints()
        {
            APIEndpoint.Purchasing = Configuration.GetValue<string>(Constant.PURCHASING_ENDPOINT) ?? Configuration[Constant.PURCHASING_ENDPOINT];
            APIEndpoint.Core = Configuration.GetValue<string>(Constant.CORE_ENDPOINT) ?? Configuration[Constant.CORE_ENDPOINT];
            APIEndpoint.Inventory = Configuration.GetValue<string>(Constant.INVENTORY_ENDPOINT) ?? Configuration[Constant.INVENTORY_ENDPOINT];
            APIEndpoint.Finance = Configuration.GetValue<string>(Constant.FINANCE_ENDPOINT) ?? Configuration[Constant.FINANCE_ENDPOINT];
            APIEndpoint.CustomsReport = Configuration.GetValue<string>(Constant.CUSTOMSREPORT_ENDPOINT) ?? Configuration[Constant.FINANCE_ENDPOINT];
            APIEndpoint.Sales = Configuration.GetValue<string>(Constant.SALES_ENDPOINT) ?? Configuration[Constant.SALES_ENDPOINT];
            APIEndpoint.Auth = Configuration.GetValue<string>(Constant.AUTH_ENDPOINT) ?? Configuration[Constant.AUTH_ENDPOINT];
            APIEndpoint.GarmentProduction = Configuration.GetValue<string>(Constant.GARMENT_PRODUCTION_ENDPOINT) ?? Configuration[Constant.GARMENT_PRODUCTION_ENDPOINT];
            APIEndpoint.POS = Configuration.GetValue<string>(Constant.POS_ENDPOINT) ?? Configuration[Constant.POS_ENDPOINT];

            AuthCredential.Username = Configuration.GetValue<string>(Constant.USERNAME) ?? Configuration[Constant.USERNAME];
            AuthCredential.Password = Configuration.GetValue<string>(Constant.PASSWORD) ?? Configuration[Constant.PASSWORD];
        }

        private void RegisterFacades(IServiceCollection services)
        {
            services
                .AddTransient<ICoreData, CoreData>()
                .AddTransient<ICoreHttpClientService, CoreHttpClientService>()
                .AddTransient<IMemoryCacheManager, MemoryCacheManager>()
                .AddTransient<IPkpbjFacade, PkpbjFacade>()
                .AddTransient<InventoryFacade>()
                .AddTransient<ExpeditionFacade>()
                .AddTransient<ITransferInDoc, TransferFacade>()
                .AddTransient<ITransferOutDoc, TransferOutFacade>()
                .AddTransient<TransferInStoreFacade>()
                .AddTransient<ReturnToCenterFacade>()
                .AddTransient<TransferStockFacade>()
                .AddTransient<IAdjustmentDoc, AdjustmentFacade>()
                .AddTransient<ISODoc, SOFacade>()
                .AddTransient<SPKDocsFacade>()
                .AddTransient<ISPKDoc, SPKDocsControllerFacade>()
                .AddTransient<IInventoryLoader, InventoryLoaderFacade>();
        }

        private void AddTransient<T>()
        {
            throw new NotImplementedException();
        }

        private void RegisterServices(IServiceCollection services, bool isTest)
        {
            services
                .AddScoped<IdentityService>()
                .AddScoped<ValidateService>()
                .AddScoped<IValidateService, ValidateService>();

            if (isTest == false)
            {
                services.AddScoped<IHttpClientService, HttpClientService>();
            }
        }

        private void RegisterSerializationProvider()
        {
            BsonSerializer.RegisterSerializationProvider(new SerializationProvider());
        }

        private void RegisterClassMap()
        {
            //ClassMap<UnitReceiptNoteViewModel>.Register();
            //ClassMap<UnitReceiptNoteItemViewModel>.Register();
            //ClassMap<UnitViewModel>.Register();
            //ClassMap<DivisionViewModel>.Register();
            //ClassMap<CategoryViewModel>.Register();
            //ClassMap<ProductViewModel>.Register();
            //ClassMap<UomViewModel>.Register();
            //ClassMap<PurchaseOrderViewModel>.Register();
            //ClassMap<SupplierViewModel>.Register();
            //ClassMap<UnitPaymentCorrectionNoteViewModel>.Register();
        }

        #endregion Register

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? Configuration[Constant.DEFAULT_CONNECTION];
            string env = Configuration.GetValue<string>(Constant.ASPNETCORE_ENVIRONMENT);
            string connectionStringLocalCashFlow = Configuration.GetConnectionString("LocalDbCashFlowConnection") ?? Configuration["LocalDbCashFlowConnection"];
            APIEndpoint.ConnectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];

            /* Register */
            //services.AddDbContext<PurchasingDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<WarehouseDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(1000 * 60 * 20)));
            services.AddTransient<ILocalDbCashFlowDbContext>(s => new LocalDbCashFlowDbContext(connectionStringLocalCashFlow));
            RegisterEndpoints();
            RegisterFacades(services);
            RegisterServices(services, env.Equals("Test"));
            services.AddAutoMapper(typeof(BaseAutoMapperProfile));
            services.AddMemoryCache();

            RegisterSerializationProvider();
            RegisterClassMap();
            //MongoDbContext.connectionString = Configuration.GetConnectionString(Constant.MONGODB_CONNECTION) ?? Configuration[Constant.MONGODB_CONNECTION];


            /* Versioning */
            services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1, 0); });

            /* Authentication */
            string Secret = Configuration.GetValue<string>(Constant.SECRET) ?? Configuration[Constant.SECRET];
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });

            /* CORS */
            services.AddCors(options => options.AddPolicy(PURCHASING_POLICITY, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders(EXPOSED_HEADERS);
            }));

            /* API */
            services
               .AddMvcCore()
               .AddApiExplorer()
               .AddAuthorization()
               .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
               .AddJsonFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BateeqWareHouse API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() },
                });
                c.OperationFilter<ResponseHeaderFilter>();

                c.CustomSchemaIds(i => i.FullName);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* Update Database */
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                WarehouseDbContext context = serviceScope.ServiceProvider.GetService<WarehouseDbContext>();
                context.Database.SetCommandTimeout(10 * 60 * 1000);
                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    context.Database.Migrate();
                }
            }

            app.UseAuthentication();
            app.UseCors(PURCHASING_POLICITY);
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            JobManager.Initialize(new MasterRegistry(app.ApplicationServices));
        }
    }
}
