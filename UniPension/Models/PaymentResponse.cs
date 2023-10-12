using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniPension.Models
{

    public class PaymentResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public PaymentInfo data { get; set; }
    }

    public class PaymentInfo
    {
        public string PID { get; set; }
        public int Install_Paid_Count { get; set; }
        public float Install_Paid_Amount { get; set; }
        public string Next_Due_Date { get; set; }
    }
    
}