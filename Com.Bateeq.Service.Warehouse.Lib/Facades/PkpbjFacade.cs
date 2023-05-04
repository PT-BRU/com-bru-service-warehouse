using System;
using System.Collections.Generic;
using System.Linq;
using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Models.SPKDocsModel;
using Com.Moonlay.NetCore.Lib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.SpkDocsViewModel;
using Com.Moonlay.Models;
using MongoDB.Bson;
using HashidsNet;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using System.Net.Http;
using System.Text;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Dynamic;
using Com.Bateeq.Service.Warehouse.Lib.Models.TransferModel;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces.PkbjInterfaces;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Data;

namespace Com.Bateeq.Service.Warehouse.Lib.Facades
{
    public class PkpbjFacade : IPkpbjFacade
    {
        private string USER_AGENT = "Facade";

        private readonly WarehouseDbContext dbContext;
        public readonly IServiceProvider serviceProvider;
        private readonly DbSet<SPKDocs> dbSet;
        private readonly DbSet<TransferOutDoc> dbSetTransferOut;

        public object Request { get; private set; }
        public object ApiVersion { get; private set; }

        public PkpbjFacade(IServiceProvider serviceProvider, WarehouseDbContext dbContext)
        {
            this.serviceProvider = serviceProvider;
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<SPKDocs>();
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> Read(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {

            //IQueryable<SPKDocs> Query = this.dbSet.Include(x=>x.Items).Where(x => !x.PackingList.Contains("BTQ-FN"));
            
            IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items);

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName"
            };
            
            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPackingList(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(x => !x.PackingList.Contains("BTQ-FN"));
            //IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(i => i.DestinationCode.Contains("GDG"));
            //IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items);

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName"
            };

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPackingRTT(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            //IQueryable<SPKDocs> Query = this.dbSet.Include(x=>x.Items).Where(x => !x.PackingList.Contains("BTQ-FN"));
            IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(x => x.Reference.Contains("BTQ-KB/RTT"));

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName", "Reference"
            };

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadPackingRTP(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(x => x.IsReceived == false && (x.PackingList.Contains("BTQ-FN") || x.Reference.Contains("BTQ-KB/RTP") || (x.Reference == null && x.DestinationName.Contains("GUDANG") && x.SourceName.Contains("GUDANG") && x.IsDistributed == true)));

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName", "Reference"
            };

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadForUpload(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(x => x.PackingList.Contains("BTQ-FN"));

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName"
            };

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

		public Tuple<List<SPKDocsViewModel>, int, Dictionary<string, string>> ReadForUploadNew(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
		{
			IQueryable<SPKDocs> Query = this.dbSet.Include(x => x.Items).Where(x => x.PackingList.Contains("BTQ-FN"));
			IQueryable<SPKDocsViewModel> QueryV = from a in Query
												  select new SPKDocsViewModel
												  {
													  _id = a.Id,
													  packingList = a.PackingList,
													  date = a.Date,
													  password = a.Password,
													  reference = a.Reference,
													  source = new SourceViewModel
													  {
														  code = a.SourceCode,
														  name = a.SourceName
													  },
													  destination = new DestinationViewModel
													  {
														  code = a.DestinationCode,
														  name = a.DestinationName
													  },
													  isReceived = a.IsReceived,
													  code = a.SourceCode,
													  sourceName = a.SourceName,
													  destinationName = a.DestinationName,
													  CreatedBy = a.CreatedBy,
													  LastModifiedUtc = a.LastModifiedUtc,
													  status = a.IsReceived == true ? "Sudah Diterima" : "Belum Diterima"
												  }

												  ;
			List<string> searchAttributes = new List<string>()
			{
				"packingList", "sourceName", "destinationName","status"
			};

			QueryV = QueryHelper<SPKDocsViewModel>.ConfigureSearch(QueryV, searchAttributes, Keyword);

			Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
			QueryV = QueryHelper<SPKDocsViewModel>.ConfigureFilter(QueryV, FilterDictionary);

			Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
			QueryV = QueryHelper<SPKDocsViewModel>.ConfigureOrder(QueryV, OrderDictionary);

			Pageable<SPKDocsViewModel> pageable = new Pageable<SPKDocsViewModel>(QueryV, Page - 1, Size);
			List<SPKDocsViewModel> Data = pageable.Data.ToList<SPKDocsViewModel>();
			int TotalData = pageable.TotalCount;

			return Tuple.Create(Data, TotalData, OrderDictionary);
		}

		public Tuple<List<SPKDocs>, int, Dictionary<string, string>> ReadExpedition(int Page = 1, int Size = 25, string Order = "{}", string Keyword = null, string Filter = "{}")
        {
            IQueryable<SPKDocs> Query = this.dbSet.Include(x=>x.Items).Where(x => x.IsDistributed == false);

            List<string> searchAttributes = new List<string>()
            {
                "PackingList", "SourceName", "DestinationName"
            };

            Query = QueryHelper<SPKDocs>.ConfigureSearch(Query, searchAttributes, Keyword);

            Dictionary<string, string> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            Query = QueryHelper<SPKDocs>.ConfigureFilter(Query, FilterDictionary);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(Order);
            Query = QueryHelper<SPKDocs>.ConfigureOrder(Query, OrderDictionary);

            Pageable<SPKDocs> pageable = new Pageable<SPKDocs>(Query, Page - 1, Size);
            List<SPKDocs> Data = pageable.Data.ToList<SPKDocs>();
            int TotalData = pageable.TotalCount;

            return Tuple.Create(Data, TotalData, OrderDictionary);
        }

        public SPKDocs ReadById(int id)
        {
            var a = this.dbSet.Where(p => p.Id == id)
                .Include(p => p.Items)
                .FirstOrDefault();
            return a;
        }

        public SPKDocs ReadByReference(string reference)
        {
            var model = dbSet.Where(m => m.Reference == reference && m.DestinationCode != "GDG.05")
                 .Include(m => m.Items)
                 .FirstOrDefault();
            return model;
        }

        public string GenerateCode(string ModuleId)
        {
            var uid = ObjectId.GenerateNewId().ToString();
            var hashids = new Hashids(uid, 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
            var now = DateTime.Now;
            var begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var diff = (now - begin).Milliseconds;
            string code = String.Format("{0}/{1}/{2}", hashids.Encode(diff), ModuleId, DateTime.Now.ToString("MM/yyyy"));
            return code;
        }

        public async Task<int> Create(SPKDocs model, string username, int clientTimeZoneOffset = 7)
        {
            int Created = 0;

            using (var transaction = this.dbContext.Database.BeginTransaction())
            {
                try
                {
                    EntityExtension.FlagForCreate(model, username, USER_AGENT);

                    string packingList = GenerateCode("BTQ-KB/PLB");
                    string code = GenerateCode("BTQ-PK/PBJ");
                    string password = String.Join("",GenerateCode(DateTime.Now.ToString("dd")).Split("/"));
                    //(generateCode(("0" + date.getDate()).slice(-2))).split('/').join('')

                    foreach (var i in model.Items)
                    {
                        EntityExtension.FlagForCreate(i, username, USER_AGENT);
                        //var inven = GetItems(i.ItemCode, i.ItemName, i.ItemArticleRealizationOrder);
                        //if (inven == null)
                        //{
                        //    ItemCoreViewModel item = new ItemCoreViewModel
                        //    {
                        //        dataDestination = new List<ItemViewModelRead> {
                        //            new ItemViewModelRead{
                        //                ArticleRealizationOrder = i.ItemArticleRealizationOrder,
                        //                code = i.ItemCode,
                        //                name = i.ItemName,
                        //                Remark = i.Remark,
                        //                Size = i.ItemSize,
                        //                Uom = i.ItemUom,

                        //            }

                        //        },
                        //        DomesticCOGS = i.ItemDomesticCOGS,
                        //        DomesticRetail = i.ItemDomesticRetail,
                        //        DomesticSale = i.ItemDomesticSale,
                        //        DomesticWholesale = i.ItemDomesticWholesale
                        //    };

                        //    string itemsUri = "items/finished-goods";
                        //    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                        //    var response = await httpClient.PostAsync($"{APIEndpoint.Core}{itemsUri}", new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, General.JsonMediaType));

                        //    response.EnsureSuccessStatusCode();

                        //}


                    }
                    model.Code = code;
                    model.Date = new DateTimeOffset(model.CreatedUtc);
                    model.PackingList = packingList;
                    model.Password = password;
                    model.IsReceived = false;
                    model.IsDraft = false;
                    model.IsDistributed = false;

                    dbSet.Add(model);
                    Created = await dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }

            return Created;
        }

        private ItemCoreViewModel GetItems(string code, string name, string ro)
        {
            string itemUri = "items/finished-goods/readinven";
            string queryUri = "?code=" + code + "&ro=" + ro + "&name=" + name;
            string uri = itemUri + queryUri;
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
            var response = httpClient.GetAsync($"{APIEndpoint.Core}{uri}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                ItemCoreViewModel viewModel = JsonConvert.DeserializeObject<ItemCoreViewModel>(result.GetValueOrDefault("data").ToString());
                return viewModel;//.Where(x => x.dataDestination[0].name == name && x.dataDestination[0].code == code).FirstOrDefault();
                //throw new Exception(string.Format("{0}, {1}, {2}", response.StatusCode, response.Content, APIEndpoint.Purchasing));
            }
            else
            {
                return null;
            }
        }

        public List<string> CsvHeader { get; } = new List<string>()
        {
            "PackList", "Password", "Barcode", "Name", "Size", "Price", "UOM", "QTY", "RO", "HPP"
        };

        public sealed class PkbjMap : CsvHelper.Configuration.ClassMap<SPKDocsCsvViewModel>
        {
            public PkbjMap()
            {
                Map(p => p.PackingList).Index(0);
                Map(p => p.Password).Index(1);
                Map(p => p.code).Index(2);
                Map(p => p.name).Index(3);
                Map(p => p.size).Index(4);
                Map(p => p.domesticSale).Index(5).TypeConverter<StringConverter>();
                Map(p => p.uom).Index(6);
                Map(p => p.quantity).Index(7).TypeConverter<StringConverter>();
                Map(p => p.articleRealizationOrder).Index(8);
                Map(p => p.domesticCOGS).Index(9).TypeConverter<StringConverter>();
            }
        }

        public Tuple<bool, List<object>> UploadValidate(ref List<SPKDocsCsvViewModel> Data, List<KeyValuePair<string, StringValues>> Body)
        {
            List<object> ErrorList = new List<object>();
            string ErrorMessage;
            bool Valid = true;
            // Currency currency = null;
            //Uom uom = null;

            foreach (SPKDocsCsvViewModel productVM in Data)
            {
                ErrorMessage = "";
				if (!(productVM.PackingList.Contains("BTQ")))
				{
					ErrorMessage = string.Concat(ErrorMessage, "Format Packing List harus 'xxxx/BTQ-FN/xx/xx, ");
				}
				if (string.IsNullOrWhiteSpace(productVM.PackingList))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "PackingList tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.Password))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Password tidak boleh kosong, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.code))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Barcode tidak boleh kosong, ");
                }
                else if (Data.Any(d => d != productVM && d.code.Equals(productVM.code)))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Barcode tidak boleh duplikat, ");
                }

                if (string.IsNullOrWhiteSpace(productVM.name))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh kosong, ");
                }
                //else if (Data.Any(d => d != productVM && d.name.Equals(productVM.name)))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Nama tidak boleh duplikat, ");
                //}

                if (string.IsNullOrWhiteSpace(productVM.size))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Size tidak boleh kosong, ");
                }

                //if (string.IsNullOrWhiteSpace(productVM.Currency.Code))
                //{
                //    ErrorMessage = string.Concat(ErrorMessage, "Mata Uang tidak boleh kosong, ");
                //}
                decimal domesticSale = 0;
                if (string.IsNullOrWhiteSpace(productVM.domesticSale))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.domesticSale, out domesticSale))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus numerik, ");
                }
                else if (domesticSale < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Harga harus lebih besar dari 0, ");
                }
                else
                {
                    string[] domesticSaleSplit = productVM.domesticSale.Split('.');
                    if (domesticSaleSplit.Count().Equals(2) && domesticSaleSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "Harga maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }


                if (string.IsNullOrWhiteSpace(productVM.uom))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Satuan tidak boleh kosong, ");
                }

                decimal quantity = 0;
                if (string.IsNullOrWhiteSpace(productVM.quantity))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.quantity, out quantity))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity harus numerik, ");
                }
                else if (quantity < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "Quantity harus lebih besar dari 0, ");
                }
                //else
                //{
                //    string[] domesticSaleSplit = productVM.domesticSale.Split('.');
                //    if (domesticSaleSplit.Count().Equals(2) && domesticSaleSplit[1].Length > 2)
                //    {
                //        ErrorMessage = string.Concat(ErrorMessage, "Harga maksimal memiliki 2 digit dibelakang koma, ");
                //    }
                //}

                if (string.IsNullOrWhiteSpace(productVM.articleRealizationOrder))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "RO tidak boleh kosong, ");
                }

                decimal domesticCOGS = 0;
                if (string.IsNullOrWhiteSpace(productVM.domesticCOGS))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "HPP tidak boleh kosong, ");
                }
                else if (!decimal.TryParse(productVM.domesticCOGS, out domesticCOGS))
                {
                    ErrorMessage = string.Concat(ErrorMessage, "HPP harus numerik, ");
                }
                else if (domesticCOGS < 0)
                {
                    ErrorMessage = string.Concat(ErrorMessage, "HPP harus lebih besar dari 0, ");
                }
                else
                {
                    string[] domesticCOGSSplit = productVM.domesticCOGS.Split('.');
                    if (domesticCOGSSplit.Count().Equals(2) && domesticCOGSSplit[1].Length > 2)
                    {
                        ErrorMessage = string.Concat(ErrorMessage, "HPP maksimal memiliki 2 digit dibelakang koma, ");
                    }
                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {

                }

                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    productVM.quantity = quantity;
                    productVM.domesticCOGS = domesticCOGS;
                    productVM.domesticSale = domesticSale;

                }
                else
                {
                    ErrorMessage = ErrorMessage.Remove(ErrorMessage.Length - 2);
                    var Error = new ExpandoObject() as IDictionary<string, object>;

                    Error.Add("PackingList", productVM.PackingList);
                    Error.Add("Password", productVM.Password);
                    Error.Add("Barcode", productVM.code);
                    Error.Add("Name", productVM.name);
                    Error.Add("Size", productVM.size);
                    Error.Add("Price", productVM.domesticSale);
                    Error.Add("UOM", productVM.uom);
                    Error.Add("Qty", productVM.quantity);
                    Error.Add("RO", productVM.articleRealizationOrder);
                    Error.Add("HPP", productVM.domesticCOGS);
                    Error.Add("Error", ErrorMessage);

                    ErrorList.Add(Error);
                }
            }

            if (ErrorList.Count > 0)
            {
                Valid = false;
            }

            return Tuple.Create(Valid, ErrorList);
        }

        public async Task UploadData(SPKDocs data, string username)
        {
            foreach (var i in data.Items)
            {
                EntityExtension.FlagForCreate(i, username, USER_AGENT);
            }
            EntityExtension.FlagForCreate(data, username, USER_AGENT);
            dbSet.Add(data);
            var result = await dbContext.SaveChangesAsync();
            //await BulkInsert(data, username);
        }

        public async Task<SPKDocsViewModel> MapToViewModel(List<SPKDocsCsvViewModel> csv, double source, string sourcec, string sourcen, double destination, string destinationc, string destinationn, DateTimeOffset date)
        {
            List<SPKDocsItemViewModel> sPKDocsItems = new List<SPKDocsItemViewModel>();
            //var itemx = GetItem();

            foreach (var i in csv)
            {
                var itemx = GetItem(i.code);
                if (itemx.Count() == 0 || itemx == null)
                {
                    //item = new ViewModels.NewIntegrationViewModel.ItemViewModel
                    //{
                        //articleRealizationOrder = i.articleRealizationOrder,
                        //_id = itemx.Id,
                        //code = i.code,
                        //domesticCOGS = Convert.ToDouble(i.domesticCOGS),
                        //domesticSale = Convert.ToDouble(i.domesticSale),
                        //name = i.name,
                        //size = i.size,
                        //uom = i.uom

                    //},
                    //quantity = Convert.ToDouble(i.quantity),
                    //remark = ""
                //});
                ItemCoreViewModel item = new ItemCoreViewModel
                {
                    dataDestination = new List<ItemViewModelRead>
                        {
                           new ItemViewModelRead
                           {
                               ArticleRealizationOrder = i.articleRealizationOrder,
                               code = i.code,
                               name = i.name,
                               Size = i.size,
                               Uom = i.uom,
                           }
                        },
                    DomesticCOGS = Convert.ToDouble(i.domesticCOGS),
                    DomesticRetail = 0,
                    DomesticSale = Convert.ToDouble(i.domesticSale),
                    DomesticWholesale = 0,
                };
                    string itemsUri = "items/finished-goods";
                    var httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));
                    var response = await httpClient.PostAsync($"{APIEndpoint.Core}{itemsUri}", new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, General.JsonMediaType));

                    response.EnsureSuccessStatusCode();

                    var item2 = GetItem(i.code);

                    sPKDocsItems.Add(new SPKDocsItemViewModel
                    {
                        item = new ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            articleRealizationOrder = i.articleRealizationOrder,
                            _id = item2.Single()._id,
                            code = i.code,
                            domesticCOGS = Convert.ToDouble(i.domesticCOGS),
                            domesticSale = Convert.ToDouble(i.domesticSale),
                            name = i.name,
                            size = i.size,
                            uom = i.uom

                        },
                        quantity = Convert.ToDouble(i.quantity),
                        sendquantity = Convert.ToDouble(i.quantity),
                        remark = ""
                    });
                }
                else
                {
                    sPKDocsItems.Add(new SPKDocsItemViewModel
                    {
                        item = new ViewModels.NewIntegrationViewModel.ItemViewModel
                        {
                            articleRealizationOrder = i.articleRealizationOrder,
                            _id = itemx.Single()._id,
                            code = i.code,
                            domesticCOGS = Convert.ToDouble(i.domesticCOGS),
                            domesticSale = Convert.ToDouble(i.domesticSale),
                            name = i.name,
                            size = i.size,
                            uom = i.uom

                        },
                        quantity = Convert.ToDouble(i.quantity),
                        sendquantity = Convert.ToDouble(i.quantity),
                        remark = ""
                    });
                }
            }

            SPKDocsViewModel sPKDocsViews = new SPKDocsViewModel
            {
                code = GenerateCode("BTQ-PK/PBJ"),
                date = date,
                packingList = csv.FirstOrDefault().PackingList,
                password = csv.FirstOrDefault().Password,
                reference = csv.FirstOrDefault().PackingList,
                isDistributed = false,
                isReceived = false,
                Weight = 0,
                source = new SourceViewModel
                {
                    _id = (long)source,
                    code = sourcec,
                    name = sourcen
                },
                destination = new DestinationViewModel
                {
                    _id = (long)destination,
                    code = destinationc,
                    name = destinationn
                },

                items = sPKDocsItems
            };

            return sPKDocsViews;
        }

        private List<ItemCoreViewModel> GetItem(string itemCode)
        {
            string itemUri = "items/finished-goods/Code";
            IHttpClientService httpClient = (IHttpClientService)serviceProvider.GetService(typeof(IHttpClientService));

            var response = httpClient.GetAsync($"{APIEndpoint.Core}{itemUri}/{itemCode}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                List<ItemCoreViewModel> viewModel = JsonConvert.DeserializeObject<List<ItemCoreViewModel>>(result.GetValueOrDefault("data").ToString());
                //return viewModel.OrderByDescending(s => s.Date).FirstOrDefault(s => s.Date < doDate.AddDays(1)); ;
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        public MemoryStream GenerateExcel(int id)
        {
            var Query = from a in dbContext.SPKDocs
                        join b in dbContext.SPKDocsItems on a.Id equals b.SPKDocsId
                        where a.Id == id
                        select new
                        {
                            a.Code,
                            a.SourceCode,
                            a.SourceName,
                            a.DestinationCode,
                            a.DestinationName,
                            b.ItemCode,
                            b.ItemName,
                            b.Quantity,
                            b.Remark
                        };
            DataTable result = new DataTable();

            //result.Columns.Add(new DataColumn());
            result.Columns.Add(new DataColumn() { ColumnName = "No Referensi", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Dari", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Ke", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Barcode", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Nama", DataType = typeof(String) });
            result.Columns.Add(new DataColumn() { ColumnName = "Kuantitas Penerimaan", DataType = typeof(Double) });
            result.Columns.Add(new DataColumn() { ColumnName = "Catatan", DataType = typeof(String) });

            if (Query.Count() == 0)
                result.Rows.Add("", "", "", "", "", 0,  "");
            else
            {
                foreach (var item in Query)
                {

                    result.Rows.Add(item.Code, item.SourceCode + "-" + item.SourceName, item.DestinationCode + "-" + item.DestinationName, item.ItemCode, item.ItemName, item.Quantity, item.Remark);
                }
            }

            return Excel.CreateExcel(new List<KeyValuePair<DataTable, string>> { (new KeyValuePair<DataTable, string>(result, "Pemasukan Gudang Pusat")) }, true);

        }
    }
}
