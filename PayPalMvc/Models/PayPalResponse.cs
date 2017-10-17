using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPalMvc.Models
{
    public class PayPalResponse
    {
        public string item_number { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string custom { get; set; }
        public string item_name { get; set; }
        public string tx { get; set; }
    }
}