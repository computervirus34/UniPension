using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniPension.IRepositories
{
    public interface ITokenRepository
    {
        string RestClientWithAuth(string strBearTokenURL, string userID, string password, string grant_type);
    }
}
