using UniPension.Data;
using UniPension.IConfiguration;
using UniPension.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace UniPension.Controllers
{
    public class LoginController : Controller
    {
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUnitOfWork _unitOfWork;

        public LoginController()
        {
            _unitOfWork = new UnitOfWork();
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModels _login)
        {
            if (ModelState.IsValid) //validating the user inputs
            {

                try
                {
                    string ip = GetIp();
                    logger.Debug("Login Attempt for user ID:" + _login.UserId + " from IP:" + ip);
                    string passwordHash = _unitOfWork.Users.GetPasswordHash(_login.UserId.Trim() + _login.Password).ToString();
                    LoginModels _loginCredentials = _unitOfWork.Users.ValidateUser(_login);
                    if (_loginCredentials != null)
                    {
                        logger.Debug("Login successful! for user ID:" + _login.UserId + " from IP:" + ip);
                        if (_loginCredentials.UserStatus == "I")
                        {
                            TempData["retMsg"] = "Please Active Your User ID";
                            return View();
                        }
                        if(_loginCredentials.Password != passwordHash)
                        {
                            TempData["retMsg"] = "Invalid password!";
                            return View();
                        }
                        var roles = _loginCredentials.UserType.Split(',').Select(int.Parse).ToList();
                        IEnumerable<MenuModels> _menus = _unitOfWork.Users.GetUserMenues(roles);
                        FormsAuthentication.SetAuthCookie(_loginCredentials.UserId, false); // set the formauthentication cookie
                        Session["LoginCredentials"] = _loginCredentials; // Bind the _logincredentials details to "LoginCredentials" session

                        if (_loginCredentials.FirstLogin == "Y")
                        {
                            return RedirectToAction("FirstLogin", "Account");
                        }

                        Session["MenuMaster"] = _menus; //Bind the _menus list to MenuMaster session
                        Session["UserID"] = _loginCredentials.UserId;
                        Session["BranchCode"] = _loginCredentials.BranchCode;
                        Session["RoutingNo"] = _loginCredentials.RoutingNo;
                        Session["BranchName"] = _loginCredentials.BranchName;
                        Session["userRole"] = _loginCredentials.UserRoleId;

                        return RedirectToAction("Welcome", "Login");
                    }
                    else
                    {
                        TempData["retMsg"] = "User ID not found!";
                        logger.Debug("Login failed! for user ID:" + _login.UserId + " from IP:" + ip);
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    TempData["retMsg"] = "Error!"+ex.Message;
                    logger.Error(ex);
                    return View();
                }
            }
            return View();
        }

        public ActionResult Welcome()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];
            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Index", "Login");
        }
    }
}