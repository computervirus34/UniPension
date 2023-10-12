using UniPension.IConfiguration;
using UniPension.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniPension.IRepositories;
using UniPension.Models;

namespace UniPension.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private UniPensionDBEntities _context;

        public IBranchRepository Branches {get; private set;}
        public IPensionPaymentRepository PensionPayment { get; private set; }
        public IUserRepository Users { get; private set; }
        public ITransactionRequestLogRepository TransactionRequestLog { get; private set; }
        public ITokenRepository CBSToken { get; private set; }

        public UnitOfWork(
            )
        {
            _context = new UniPensionDBEntities();
            //_logger = loggerFactory.CreateLogger("logs");
            Branches = new BranchRepository(_context);
            PensionPayment = new PensionePaymentRepository(_context);
            Users = new UserRepository(_context);
            TransactionRequestLog = new TransactionRequestLogRepository(_context);
            CBSToken = new TokenRepository();
        }
        
        public IEnumerable<DropDownModel> GetDropDownValue(string DropDownName)
        {
            if (DropDownName.ToLower() == "branch")
            {
                return (from q in Branches.GetAll()
                        select new DropDownModel
                        {
                            value = q.id,
                            description = q.branch_name + "-" + q.region
                        }).AsEnumerable();
            }else
            {
                return null;
            }

        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}