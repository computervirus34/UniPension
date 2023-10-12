using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniPension.Models
{
    public class CallBackModel
    {
        public string TransactionId { get; set; }
        public string Payment_Ref_No { get; set; }
        public decimal Paying_Amount { get; set; }
    }
    public class CallBackResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public string TransactionId { get; set; }
        public string Payment_Ref_No { get; set; }
        public string PaymentBatchNo { get; set; }
    }
}