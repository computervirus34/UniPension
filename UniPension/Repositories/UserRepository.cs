using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniPension.Data;
using UniPension.IRepositories;
using UniPension.Models;
using System.Security.Cryptography;
using System.Text;

namespace UniPension.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        public UserRepository(UniPensionDBEntities context) : base(context)
        {
        }

        public bool ChangePassword(string userID, string newPassword)
        {
            try
            {

                var isExists = _context.AppUsers.Where(o => o.userId == userID).SingleOrDefault();
                string passHash = GetPasswordHash(userID + newPassword);
                isExists.userPass = passHash;
                isExists.firstLogin = "N";
                _context.SaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool ResetPassword(string userID,  string newPassword)
        {
            try
            {
                var isExists = _context.AppUsers.Where(o => o.userId == userID).SingleOrDefault();
                string passHash = GetPasswordHash(userID + newPassword);

                isExists.userPass = passHash;
                isExists.firstLogin = "Y";

                _context.SaveChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public string GetPasswordHash(string userIdPassword)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding(); 
            encrypt = md5.ComputeHash(encode.GetBytes(userIdPassword));
            StringBuilder encryptdata = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }

        public IEnumerable<MenuModels> GetUserMenues(List<int> roles)
        {
            try
            {
                IEnumerable<MenuModels> mnuList = _context.RoleMenuMappings
                .Where(x => roles.Contains(x.appRoleID)).Select(x => new MenuModels
                {
                    MainMenuId = x.SubMenu.mainMenuID,
                    MainMenuName = x.SubMenu.MainMenu.name,
                    SubMenuId = x.subMenuID,
                    SubMenuName = x.SubMenu.name,
                    ControllerName = x.SubMenu.controllerName,
                    ActionName = x.SubMenu.actionName,
                    MenuStatus = x.isActive
                }).Distinct().OrderBy(x => x.MainMenuId).ToList();

                return mnuList;
            }
            catch
            {
                throw;
            }
            
        }

        public LoginModels ValidateUser(LoginModels user)
        {
            try
            {
                LoginModels userDet = _context.AppUsers.Where
                                (x => x.userId.Trim().ToLower() == user.UserId.Trim().ToLower()
                                ).Select
                                (x => new LoginModels
                                {
                                    UserId = x.userId,
                                    userName = x.Email,
                                    BranchID = x.Branch.id,
                                    BranchCode = x.Branch.branch_code,
                                    BranchName = x.Branch.branch_name,
                                    UserStatus = x.isActive,
                                    UserMobile = x.CellNo,
                                    UserMail = x.Email,
                                    UserType = x.userType,
                                    FirstLogin = x.firstLogin,
                                    Password = x.userPass
                                }).FirstOrDefault();
                return userDet;
            }
            catch
            {
                return null;
            }
        }
    }
}