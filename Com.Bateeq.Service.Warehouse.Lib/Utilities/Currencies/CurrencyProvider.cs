using Com.Bateeq.Service.Warehouse.Lib.Helpers;
using Com.Bateeq.Service.Warehouse.Lib.Interfaces;
using Com.Bateeq.Service.Warehouse.Lib.Utilities.CacheManager.CacheData;
using Com.Bateeq.Service.Warehouse.Lib.Utilities.Currencies;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Utilities.Currencies
{
    public class CurrencyProvider : ICurrencyProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CurrencyProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Currency> GetCurrencyByCurrencyCodeDate(string currencyCode, DateTimeOffset date)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));

            var currencyUri = APIEndpoint.Core + $"master/garment-currencies/single-by-code-date?code={currencyCode}&stringDate={date.DateTime.ToString("yyyy-Bateeq-dd")}";
            var currencyResponse = await httpClient.GetAsync(currencyUri);

            var currencyResult = new BaseResponse<Currency>()
            {
                data = new Currency()
            };

            if (currencyResponse.IsSuccessStatusCode)
            {
                currencyResult = JsonConvert.DeserializeObject<BaseResponse<Currency>>(currencyResponse.Content.ReadAsStringAsync().Result, jsonSerializerSettings);
            }

            return currencyResult.data;
        }

        public async Task<Currency> GetCurrencyByCurrencyCode(string currencyCode)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var httpClient = (IHttpClientService)_serviceProvider.GetService(typeof(IHttpClientService));

            var currencyUri = APIEndpoint.Core + $"master/garment-currencies/single-by-code/{currencyCode}";
            var currencyResponse = await httpClient.GetAsync(currencyUri);

            var currencyResult = new BaseResponse<Currency>()
            {
                data = new Currency()
            };

            if (currencyResponse.IsSuccessStatusCode)
            {
                currencyResult = JsonConvert.DeserializeObject<BaseResponse<Currency>>(currencyResponse.Content.ReadAsStringAsync().Result, jsonSerializerSettings);
            }

            return currencyResult.data;
        }

        public async Task<List<Currency>> GetCurrencyByCurrencyCodeDateList(IEnumerable<Tuple<string, DateTimeOffset>> currencyTuples)
        {
            var tasks = currencyTuples.Select(s => GetCurrencyByCurrencyCodeDate(s.Item1, s.Item2));

            var result = await Task.WhenAll(tasks);

            return result.ToList();
        }
    }
}
