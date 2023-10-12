using UniPension.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniPension.Data;
using UniPension.IRepositories;

namespace UniPension.Repositories
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        public BranchRepository(UniPensionDBEntities context) : base(context)
        {
        }

        public virtual int GetBranchId(string branchCode)
        {
            return _context.Branches.Where(o => o.branch_code == branchCode).Select(o => o.id).SingleOrDefault();
        }

        public string GetBranchRoutingNo(int BranchId)
        {
            return _dbSet.Where(o => o.id == BranchId).Select(o => o.routingNo).SingleOrDefault();
        }
    }
}