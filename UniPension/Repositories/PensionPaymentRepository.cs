using UniPension.Data;
using UniPension.IRepositories;
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
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace UniPension.Repositories
{
    public class PensionePaymentRepository : GenericRepository<PensionPaymentRequest>, IPensionPaymentRepository
    {
        string baseUrl = ConfigurationManager.AppSettings.Get("baseURL");
        string userID = ConfigurationManager.AppSettings.Get("userIDUnipension");
        string userPass = ConfigurationManager.AppSettings.Get("userPassUniPension");
        private static TokenViewModel Instance = new TokenViewModel();
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private DescoBill db = new DescoBill();
        public PensionePaymentRepository(UniPensionDBEntities context) : base(context)
        {
        }

        public override bool Update(PensionPaymentRequest entityToUpdate)
        {

            try
            {
                var isExists = _dbSet.Where(o => o.Id == entityToUpdate.Id)
                                     .SingleOrDefault();
                if (isExists == null)
                    return false;

                isExists.IsSuccess = entityToUpdate.IsSuccess;
                isExists.TransactionDateTime = entityToUpdate.TransactionDateTime;
                isExists.NextDueDate = entityToUpdate.NextDueDate;
                isExists.InstallPaidCount = entityToUpdate.InstallPaidCount;
                isExists.InstallPaidAmount = entityToUpdate.InstallPaidAmount;

                return true;
            }
            catch
            {
                //_logger.LogError(ex, "{Repo} Update method error.", typeof(UserRepository));

                throw;
            }
        }

        public bool UpdateCBSInfo(PensionPaymentRequest entityToUpdate)
        {

            try
            {
                var isExists = _dbSet.Where(o => o.Id == entityToUpdate.Id)
                                     .SingleOrDefault();
                if (isExists == null)
                    return false;

                isExists.IsCbsDeposited = "Y";
                isExists.CBSBatchNo = entityToUpdate.CBSBatchNo;
                isExists.CBSTraceNo = entityToUpdate.CBSTraceNo;
                isExists.CBSDepositDate = entityToUpdate.CBSDepositDate;

                return true;
            }
            catch
            {
                //_logger.LogError(ex, "{Repo} Update method error.", typeof(UserRepository));

                throw;
            }
        }
        public string RestClientWithAuth(out string statusCode, out string message)
        {
            try
            {
                if (Instance == null)
                {
                    GetToken();
                }
                else
                {
                    if (IsTokenExpired())
                    {
                        GetToken();
                    }
                }

                statusCode = Instance.statusCode;
                message = Instance.message;
                return $"Bearer {Instance.access_token}";
            }
            catch
            {
                throw;
            }
        }
        private void GetToken()
        {
            try
            {
                baseUrl = baseUrl + "/token";

                using (HttpClient client = new HttpClient())
                {
                    // Define your parameters as key-value pairs
                    var parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "username", userID },
                { "Password", userPass },
                { "grant_type", "password"}
            };

                    // Encode the parameters in x-www-form-urlencoded format
                    var content = new FormUrlEncodedContent(parameters);

                    // Send a POST request to the API with the parameters in the body
                    HttpResponseMessage response = client.PostAsync(baseUrl, content).GetAwaiter().GetResult();

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and process the response content
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Instance = JsonConvert.DeserializeObject<TokenViewModel>(responseBody);
                        Instance.generateTime = System.DateTime.Now;
                        Console.WriteLine("Response: " + responseBody);
                    }
                    else
                    {
                        Console.WriteLine("Response: " + response.StatusCode);
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        private bool IsTokenExpired()
        {
            return Instance.generateTime.AddSeconds(Instance.expires_in) <= DateTime.Now;
        }

        public bool MapDBWithView(PensionPaymentRequest dvm)
        {
            try
            {
                return true;
            }
            catch
            {
                throw;
            }
        }

        public string GetBKBTransactionID(string BranchCode)
        {
            return BranchCode + DateTime.Now.ToString("yyyyMMddhmmssfff");
        }

        public PensionPaymentRequest GetByTransactionID(CallBackModel callBackModel)
        {
            try
            {
                return _context.PensionPaymentRequests
                    .Where(o => o.TransactionId.Equals(callBackModel.TransactionId)
                         && o.PaymentRefNo.Equals(callBackModel.Payment_Ref_No)
                         && o.PayingAmount==callBackModel.Paying_Amount)
                    .SingleOrDefault();
            }
            catch
            {
                throw;
            }
            
        }
    }
}