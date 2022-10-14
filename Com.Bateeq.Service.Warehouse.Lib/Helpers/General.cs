using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Helpers
{
    public static class General
    {
        public const string JsonMediaType = "application/json";

        public static object OK_STATUS_CODE { get; internal set; }
        public static object OK_MESSAGE { get; internal set; }
        public static object INTERNAL_ERROR_STATUS_CODE { get; internal set; }

    }
}
