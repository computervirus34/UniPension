using UniPension.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPension.IConfiguration
{
    public interface IUnitOfWork
    {
        IBranchRepository Branches { get; }
        IPensionPaymentRepository PensionPayment { get; }
        IUserRepository Users { get; }
        ITransactionRequestLogRepository TransactionRequestLog { get; }
        ITokenRepository CBSToken { get; }
        //ISMSRepository SMS { get; }
        void Save();
        void Dispose();
    }
}
