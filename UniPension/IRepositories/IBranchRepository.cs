using UniPension.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPension.IRepositories
{
    public interface IBranchRepository : IGenericRepository<Branch>
    {
        int GetBranchId(string branchCode);
        string GetBranchRoutingNo(int BranchId);
    }
}
