using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Bateeq.Service.Warehouse.Lib.ViewModels.TransferViewModels
{
    public class TransferStockViewModel
    {
        public int id { get; set; }
        public string code { get; set; }
        public string referensi { get; set; }
        public string sourcename { get; set; }
        public string sourcecode { get; set; }
        public string destinationname { get; set; }
        public string destinationcode { get; set; }
        public string transfername { get; set; }
        public string transfercode { get; set; }
        public string password { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public List<TransferOutDocItemViewModel> items { get; set; }
    }
}
