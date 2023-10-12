using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniPension.Data;
using UniPension.Models;

namespace UniPension.IRepositories
{
    public interface IUserRepository:IGenericRepository<AppUser>
    {
        LoginModels ValidateUser(LoginModels user);
        string GetPasswordHash(string userIdPassword);
        IEnumerable<MenuModels> GetUserMenues(List<int> roles);
        bool ChangePassword(string userID, string newPassword);
        bool ResetPassword(string userID, string newPassword);

    }
}
