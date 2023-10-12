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

namespace UniPension.Controllers
{
    public class AccountController : Controller
    {
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUnitOfWork _unitOfWork;

        public AccountController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Account
        public ActionResult Index()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(_unitOfWork.Users.GetAll());
        }
        public ActionResult BranchList()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(_unitOfWork.Branches.GetAll());
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel pcm)
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //string userID = _login.UserId.ToString();
            try
            {
                if (pcm.NewPassword != pcm.ConfirmPassword)
                {
                    TempData["retMsg"] = "New password and Confirm Password is not correct.";
                    return View();
                }


                var isExist = _unitOfWork.Users.ChangePassword(login.UserId, pcm.NewPassword);
                if (isExist)
                {
                    TempData["retMsg"] = "Password has been changed successfully.";
                    return View();
                }
                else
                {
                    TempData["retMsg"] = "Old password is incorrect.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Error." + ex.Message;
                return RedirectToAction("ChangePassword", "Account");
            }
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PasswordResetModel prm = new PasswordResetModel();

            var queryUsr = from q in _unitOfWork.Users.GetAll()
                           select new
                           {
                               value = q.userId,
                               description = q.userId + "--" + q.name
                           };
            prm.userList = new SelectList(queryUsr.ToList(), "value", "description");
            return View(prm);
        }

        [HttpPost]
        public ActionResult ResetPassword(PasswordResetModel prm)
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //string userID = _login.UserId.ToString();
            try
            {
                var isExist = _unitOfWork.Users.ResetPassword(prm.userID, "123456");
                TempData["retMsg"] = "Password has been reset successfully.";
                return RedirectToAction("ResetPassword","Account");

            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Error." + ex.Message;
                return RedirectToAction("ResetPassword", "Account");
            }
        }

        [HttpGet]
        public ActionResult FirstLogin()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult FirstLogin(PasswordChangeModel pcm)
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (pcm.NewPassword != pcm.ConfirmPassword)
            {
                TempData["retMsg"] = "New password and confirm password are not same.";
                return View();
            }
            try
            {
                var isSuccess = _unitOfWork.Users.ChangePassword(login.UserId, pcm.NewPassword);
                TempData["retMsg"] = "Password changed successfully. Please try login.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                TempData["retMsg"] = "Error " + ex.Message;
                return View("Index", "Login");
            }
        }
    }
}