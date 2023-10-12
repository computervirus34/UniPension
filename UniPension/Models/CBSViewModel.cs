using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniPension.Models
{
    public class AuthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string error { get; set; }
    }
    public class Error
    {
        public string error { get; set; }
    }
    public class AccountDetailsResponse
    {
        public string IsError { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public AccountModel RspnsData { get; set; }
    }
    public class AccountModel
    {
        public string AccountNo { get; set; }
        public string AcName { get; set; }
        public string BrCode { get; set; }
        public string AtypeCode { get; set; }
        public string AccountStatus { get; set; }
        public string DrawableAmt { get; set; }
    }
    public class SuccessfulTransactionResponse
    {
        public string IsError { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public TransactionResponseModel RspnsData { get; set; }
    }
    public class TransactionResponseModel
    {
        public string MsgCode { get; set; }
        public string Msg { get; set; }
        public string TraceNo { get; set; }
        public string TrnDate { get; set; }
        public string BatchNo { get; set; }
    }

    public class TransactionRequest
    {
        public string srcAccountNo { get; set; }
        public string destAccountNo { get; set; }
        public string amount { get; set; }
        public string drCr { get; set; }
        public string chequeNo { get; set; }
        public string srcAcctBrCode { get; set; }
        public string destAcctBrCode { get; set; }
        public string remarks { get; set; }
        public string trnBrCode { get; set; }
        public int trnUserCode { get; set; }
    }
}