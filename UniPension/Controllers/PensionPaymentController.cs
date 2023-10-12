using UniPension.Data;
using UniPension.IConfiguration;
using UniPension.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;

namespace UniPension.Controllers
{
    public class PensionPaymentController : Controller
    {
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string baseUrl = ConfigurationManager.AppSettings.Get("baseURL");
        string userID = ConfigurationManager.AppSettings.Get("userIDUnipension");
        string cbsURL = ConfigurationManager.AppSettings["cbsURL"];
        string cbsUser = ConfigurationManager.AppSettings["cbsUsername"];
        string cbsPass = ConfigurationManager.AppSettings["cbsPassword"];
        string grant_type = ConfigurationManager.AppSettings["grant_type"];
        string cbsAppUser = ConfigurationManager.AppSettings["cbsAppUser"];
        string cbsAppPass = ConfigurationManager.AppSettings["cbsAppPass"];
        string PostingUser = ConfigurationManager.AppSettings["PostingUser"];
        //string message = ConfigurationManager.AppSettings.Get("messageTemplate");

        private readonly IUnitOfWork _unitOfWork;

        public PensionPaymentController()
        {
            _unitOfWork = new UnitOfWork();
        }

        public PensionPaymentController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        // GET: Bill

        public ActionResult Index()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index(string searchString)
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];
            try
            {
                string statusCode, message;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/getpensionerdues");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", _unitOfWork.PensionPayment.RestClientWithAuth(out statusCode, out message));
                //Error in token genration
                if (statusCode != null)
                {
                    TempData["errMsg"] = $"{statusCode}-{message}";
                    return RedirectToAction("Index");
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        nid_pid = searchString,
                        UserId = userID
                    });
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    logger.Debug(json);
                }


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    logger.Debug(result);
                    var pensionerInfo = JsonConvert.DeserializeObject<PensionerInfoResponse>(result);

                    if (pensionerInfo.code != "200")
                    {
                        TempData["errMsg"] = $"{pensionerInfo.code}-{pensionerInfo.message}";
                        return RedirectToAction("Index");
                    }

                    var pensionerBasicInfo = pensionerInfo.data;
                    
                    //if (bvm.paymentStatus == "PAID")
                    //{
                    //    TempData["errMsg"] = $"Bill no {bvm.billNo} has already paid!";
                    //    return RedirectToAction("Index");
                    //}
                    TempData["PensionerBasicInfo"] = pensionerBasicInfo;
                }
                return RedirectToAction("PensionDetailsInfo");
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = ex.Message;
                logger.Error(ex);
                return RedirectToAction("Index");
            }

        }

        public ActionResult PensionDetailsInfo()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                PensionerBasicInfo pensionerBasicInfo = (PensionerBasicInfo)TempData["PensionerBasicInfo"];
                //long cTicks = DateTime.Now.Ticks;
                //bvm.TransactionId = _unitOfWork.PensionPayment.GetBKBTransactionID(login.BranchCode);
                return View(pensionerBasicInfo);
            }
            catch(Exception ex)
            {
                TempData["errMsg"] = $"Error: {ex.InnerException.Message}!";
                return RedirectToAction("Index", "Bill");
            }
        }

        [HttpPost]
        public ActionResult PensionDetailsInfo(PensionerBasicInfo pensionerBasicInfo)
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];
            string statusCode, message;
            bool isSuccess = false;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl + "/makeotcpayment");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                httpWebRequest.Headers.Add("Authorization", _unitOfWork.PensionPayment.RestClientWithAuth(out statusCode, out message));

                if (statusCode != null)
                {
                    TempData["errMsg"] = $"{statusCode}-{message}";
                    return RedirectToAction("Index");
                }

                var paymentRequest = MapResponseToDB(pensionerBasicInfo, login);

                if (ModelState.IsValid)
                {
                    isSuccess = _unitOfWork.PensionPayment.Insert(paymentRequest);
                    _unitOfWork.Save();
                }
                else
                {
                    TempData["errMsg"] = "DB save error! Please check all the fileds correctly.";
                    logger.Error("DB save error! Please check all the fileds correctly.");
                    return RedirectToAction("Index");
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        Payment_Ref_No = paymentRequest.PaymentRefNo,
                        PID = paymentRequest.PensionerID,
                        UserId = userID,
                        TransactionId = paymentRequest.TransactionId,
                        Paying_Install_Count = paymentRequest.PayingInstallCount,
                        Paying_Amount = paymentRequest.PayingAmount,
                        Commission_Amount = paymentRequest.CommissionAmount==null?0: paymentRequest.CommissionAmount,
                        VAT_Amount = paymentRequest.VATAmount==null ? 0: paymentRequest.VATAmount,
                        Pay_Mode = "A02",
                        CreditAccount = paymentRequest.CreditAccount
                    });
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    logger.Debug(json);
                }
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    logger.Debug(result);
                    var obj = JsonConvert.DeserializeObject<PaymentResponse>(result);
                    
                    if (obj.code != "200")
                    {
                        paymentRequest.IsSuccess = "N";
                        TempData["errMsg"] = $"{obj.code}-{obj.message}";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        paymentRequest.IsSuccess = "Y";
                        paymentRequest.TransactionDateTime = DateTime.Now;
                        paymentRequest.NextDueDate = obj.data.Next_Due_Date;
                        paymentRequest.InstallPaidCount = obj.data.Install_Paid_Count;
                        paymentRequest.InstallPaidAmount = Convert.ToDecimal(obj.data.Install_Paid_Amount);
                        
                        isSuccess = _unitOfWork.PensionPayment.Update(paymentRequest);
                        _unitOfWork.Save();

                        var cbsStatus = DoInvoiceTransaction(paymentRequest, login, out message);

                        paymentRequest.CBSBatchNo = cbsStatus.RspnsData.BatchNo;
                        paymentRequest.CBSTraceNo = cbsStatus.RspnsData.TraceNo;
                        paymentRequest.CBSDepositDate = Convert.ToDateTime(cbsStatus.RspnsData.TrnDate);
                        
                        //New update CBS
                        isSuccess = _unitOfWork.PensionPayment.UpdateCBSInfo(paymentRequest);
                        _unitOfWork.Save();
                        
                        TempData["PaymentResponse"] = obj;
                        return RedirectToAction("Success");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errMsg"] = ex.Message;
                logger.Error(ex);
                return RedirectToAction("Index");
            }
        }
        public ActionResult Success()
        {
            var login = (UniPension.Models.LoginModels)Session["LoginCredentials"];

            if (login == null)
            {
                return RedirectToAction("Index", "Login");
            }
            PaymentResponse paymentResponse = (PaymentResponse)TempData["PaymentResponse"];
            return View(paymentResponse);
        }

        private PensionPaymentRequest MapResponseToDB(PensionerBasicInfo pensionerBasicInfo, LoginModels login)
        {
            PensionPaymentRequest paymentRequest = new PensionPaymentRequest();

            paymentRequest.BranchCode = login.BranchCode;
            paymentRequest.PensionerID = pensionerBasicInfo.PID;
            paymentRequest.PensionerNid = pensionerBasicInfo.NID;
            paymentRequest.PensionerName = pensionerBasicInfo.Pension_holder_Name;
            paymentRequest.PhoneNo = pensionerBasicInfo.PhoneNo;
            paymentRequest.Email = pensionerBasicInfo.Email;
            paymentRequest.PayIntervalType = pensionerBasicInfo.Pay_Interval_Type;
            paymentRequest.TransactionId = _unitOfWork.PensionPayment.GetBKBTransactionID(login.BranchCode);
            paymentRequest.PayingInstallCount = pensionerBasicInfo.PayingInstallCount;
            paymentRequest.PayingAmount = Convert.ToDecimal(pensionerBasicInfo.PayingAmount);
            paymentRequest.CreditAccount = pensionerBasicInfo.CreditAccDetails.AccNo;
            paymentRequest.CreatedOn = DateTime.Now;
            paymentRequest.CreatedBy = login.BranchCode;
            paymentRequest.PaymentRefNo = pensionerBasicInfo.CreditAccDetails.Payment_Ref_No;
            //paymentRequest.CreditAccount = pensionerBasicInfo.CreditAccDetails.AccNo;

            return paymentRequest;
        }
        public SuccessfulTransactionResponse DoInvoiceTransaction(PensionPaymentRequest paymentRequest, LoginModels login, out string message)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(cbsURL + "/api/Transaction/RemittanceTransaction");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + _unitOfWork.CBSToken.RestClientWithAuth(cbsURL, cbsUser, cbsPass, grant_type));
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        srcAccountNo = "",
                        destAccountNo = paymentRequest.CreditAccount,
                        amount = paymentRequest.PayingAmount,
                        drCr = "DR",
                        chequeNo = "",
                        srcAcctBrCode = login.BranchCode,
                        destAcctBrCode = paymentRequest.CreditAccount.Substring(0,4),
                        remarks = "Ref:"+paymentRequest.PaymentRefNo+" Txn ID:"+paymentRequest.TransactionId,
                        trnBrCode = "0101",
                        trnUserCode = PostingUser
                    });

                    //add request log
                    logger.Debug(json);
                    var log = new CBSTransactionsLog
                    {
                        PaymentRefNo = paymentRequest.PaymentRefNo,
                        RequestJson = json,
                        TxnType = "Request"
                    };

                    if (AddCBSRequestResponseLog(log) != 0)
                    {
                        message = "Failed to add CBS log";
                        return null;
                    }

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                }


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    logger.Debug(result);
                    var txnResponse = JsonConvert.DeserializeObject<SuccessfulTransactionResponse>(result);

                    //add respose log 
                    var log = new CBSTransactionsLog
                    {
                        PaymentRefNo = paymentRequest.PaymentRefNo,
                        RequestJson = result,
                        TxnType = "R-Response"
                    };

                    AddCBSRequestResponseLog(log);

                    //process CBS transaction response
                    if (txnResponse.RspnsData.TraceNo != "0")
                    {
                        //AccountDepositRemittance accRemit = db.AccountDepositRemittances.Find(avm.id);
                        message = "Success";
                        return txnResponse;
                    }
                    else
                    {
                        message = txnResponse.RspnsData.Msg;
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                message = ex.Message;
                return null;
            }
        }

        public int AddCBSRequestResponseLog(CBSTransactionsLog txn)
        {

            try
            {
                CBSTransactionsLog cbs = new CBSTransactionsLog();
                cbs.PaymentRefNo = txn.PaymentRefNo;
                cbs.TxnType = txn.TxnType;
                cbs.RequestJson = txn.RequestJson;
                cbs.CreatedOn = DateTime.Now;

                _unitOfWork.TransactionRequestLog.Insert(cbs);
                _unitOfWork.Save();
                return 0;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        logger.Error("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}