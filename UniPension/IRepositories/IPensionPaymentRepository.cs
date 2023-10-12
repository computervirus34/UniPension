using UniPension.Data;
using UniPension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPension.IRepositories
{
    public interface IPensionPaymentRepository : IGenericRepository<PensionPaymentRequest>
    {
        string RestClientWithAuth(out string statusCode, out string message);
        string GetBKBTransactionID(string BranchCode);
        PensionPaymentRequest GetByTransactionID(CallBackModel callBackModel);
        bool UpdateCBSInfo(PensionPaymentRequest paymentRequest);
    }
}
