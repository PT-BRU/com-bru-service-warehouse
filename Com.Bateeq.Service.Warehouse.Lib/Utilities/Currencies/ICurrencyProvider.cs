using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Bateeq.Service.Warehouse.Lib.Utilities.Currencies
{
    public interface ICurrencyProvider
    {
        Task<Currency> GetCurrencyByCurrencyCode(string currencyCode);
        Task<Currency> GetCurrencyByCurrencyCodeDate(string currencyCode, DateTimeOffset date);
        Task<List<Currency>> GetCurrencyByCurrencyCodeDateList(IEnumerable<Tuple<string, DateTimeOffset>> currencyTuples);
    }
}
