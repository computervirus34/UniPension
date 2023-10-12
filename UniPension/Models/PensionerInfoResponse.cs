using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniPension.Models
{

    public class PensionerInfoResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public PensionerBasicInfo data { get; set; }
    }

    public class PensionerBasicInfo
    {
        public string NID { get; set; }
        public string PID { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Pension_holder_Name { get; set; }
        public float Install_Amount { get; set; }
        public string Pay_Interval_Type { get; set; }
        public float Payable_Per_Installment { get; set; }
        public string Scheme_name { get; set; }
        public int Total_Due_Install_Count { get; set; }
        public float Total_Due_Install_Amount { get; set; }
        public float Total_Fine_Amount { get; set; }
        public float Grand_Total_Due_Amount { get; set; }
        public Due_Installments[] Due_Installments { get; set; }
        public Creditaccdetails CreditAccDetails { get; set; }
        public Nullable<int> PayingInstallCount { get; set; }
        public Nullable<decimal> PayingAmount { get; set; }
        public Nullable<decimal> CommissionAmount { get; set; }
        public Nullable<decimal> VATAmount { get; set; }
        public Nullable<int> InstallPaidCount { get; set; }
        public Nullable<decimal> InstallPaidAmount { get; set; }
    }

    public class Creditaccdetails
    {
        public string AccNo { get; set; }
        public string Payment_Ref_No { get; set; }
    }

    public class Due_Installments
    {
        public int Install_No { get; set; }
        public float Due_Amount { get; set; }
        public float Fine_Amount { get; set; }
        public float Total_Due { get; set; }
        public string Payment_Currency { get; set; }
        public string Due_Date { get; set; }
    }
    
}