using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.Enums
{
    public enum ExpeditionPosition
    {
        INVALID = 0,
        PURCHASING_DIVISION = 1,
        SEND_TO_VERIFICATION_DIVISION = 2,
        VERIFICATION_DIVISION = 3,
        SEND_TO_CASHIER_DIVISION = 4,
        SEND_TO_ACCOUNTING_DIVISION = 5,
        SEND_TO_PURCHASING_DIVISION = 6,
        CASHIER_DIVISION = 7,
        FINANCE_DIVISION = 8
    }
}
