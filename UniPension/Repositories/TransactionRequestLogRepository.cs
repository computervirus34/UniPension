using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using UniPension.Data;
using UniPension.IRepositories;

namespace UniPension.Repositories
{
    public class TransactionRequestLogRepository : GenericRepository<CBSTransactionsLog>, ITransactionRequestLogRepository
    {
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TransactionRequestLogRepository(UniPensionDBEntities context)
            : base(context)
        {

        }

        public CBSTransactionsLog GetObject()
        {
            return new CBSTransactionsLog();
        }

        public dynamic DoTransaction(PensionPaymentRequest txnReq, string token, string url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "/api/Transaction/TransferTransaction");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        srcAccountNo = "",
                        destAccountNo = txnReq.CreditAccount,
                        amount = txnReq.PayingAmount,
                        drCr = "CR",
                        chequeNo = "",
                        srcAcctBrCode = txnReq.BranchCode,
                        destAcctBrCode = "4001",
                        remarks = txnReq.PaymentRefNo,
                        trnBrCode = "0101",
                        trnUserCode = "108"
                    });
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    logger.Debug(json);
                }


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var result = streamReader.ReadToEnd();
                        logger.Debug(result);
                        dynamic txnResponse = JsonConvert.DeserializeObject<dynamic>(result);
                        return txnResponse;
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public PensionPaymentRequest GetTransactionData(string srcAccount, string destAccount, decimal amount, string srcBranch, string destBranch, string remarks)
        {
            throw new NotImplementedException();
        }
    }
}