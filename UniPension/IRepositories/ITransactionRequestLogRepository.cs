using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniPension.Data;

namespace UniPension.IRepositories
{
    public interface ITransactionRequestLogRepository : IGenericRepository<CBSTransactionsLog>
    {
        CBSTransactionsLog GetObject();
        dynamic DoTransaction(PensionPaymentRequest paymentRequest, string token, string url);
        PensionPaymentRequest GetTransactionData(string srcAccount, string destAccount, decimal amount, string srcBranch, string destBranch, string remarks);
    }
}