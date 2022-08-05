using Com.Bateeq.Service.Warehouse.Lib.ViewModels.NewIntegrationViewModel;
using Com.Bateeq.Service.Warehouse.WebApi.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Test.DataUtils.NewIntegrationDataUtils
{
    public class StorageDataUtil
    {
        public StorageViewModel2 GetNewData()
        {
            long nowTicks = DateTimeOffset.Now.Ticks;

            var data = new StorageViewModel2
            {
                Id = 1,
                Code = "storagecode",
                Name = "storagename",
                IsCentral = false
            };
            
            return data;
        }

        public Dictionary<string, object> GetResultFormatterOk()
        {
            var data = GetNewData();

            Dictionary<string, object> result =
                new ResultFormatter("1.0", General.OK_STATUS_CODE, General.OK_MESSAGE)
                .Ok(data);

            return result;
        }

        public string GetResultFormatterOkString()
        {
            var result = GetResultFormatterOk();

            return JsonConvert.SerializeObject(result);
        }
    }
}
