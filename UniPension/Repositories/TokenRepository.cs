using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using UniPension.IRepositories;
using UniPension.Models;

namespace UniPension.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        string ClientSecret = ConfigurationManager.AppSettings.Get("ClientSecret");
        string ClientId = ConfigurationManager.AppSettings.Get("ClientId");
        private static AuthToken AuthToken = null;
        //private Error error = null;
        ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static DateTime? lastTokenGenerated = null;
        public void GetToken(string strBearTokenURL, string userID, string password, string grant_type)
        {
            try
            {
                strBearTokenURL = strBearTokenURL + "/token";
                HttpClient client = HeadersForAccessTokenGenerate(strBearTokenURL);
                string body = "username=" + userID + "&password=" + password + "&grant_type=" + grant_type;
                client.BaseAddress = new Uri(strBearTokenURL);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
                request.Content = new StringContent(body,
                                                    Encoding.UTF8,
                                                    "application/x-www-form-urlencoded");//CONTENT-TYPE header  

                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();

                postData.Add(new KeyValuePair<string, string>("username", userID));
                postData.Add(new KeyValuePair<string, string>("password", password));
                postData.Add(new KeyValuePair<string, string>("grant_type", grant_type));


                logger.Debug(request);

                request.Content = new FormUrlEncodedContent(postData);
                HttpResponseMessage tokenResponse = client.PostAsync(strBearTokenURL, new FormUrlEncodedContent(postData)).GetAwaiter().GetResult();
                if (tokenResponse.IsSuccessStatusCode)
                {
                    var responseContent = tokenResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    logger.Debug(responseContent);
                    AuthToken = JsonConvert.DeserializeObject<AuthToken>(responseContent);
                    lastTokenGenerated = System.DateTime.Now;
                }
                //return AuthToken.access_token;

                logger.Debug(tokenResponse.ReasonPhrase + tokenResponse.Content);
            }
            catch
            {
                throw;
            }
        }

        private bool IsTokenExpired()
        {
            DateTime dtDateTime = Convert.ToDateTime(lastTokenGenerated);
            dtDateTime = dtDateTime.AddSeconds(AuthToken.expires_in).ToLocalTime();
            return dtDateTime <= DateTime.Now;
        }


        public string RestClientWithAuth(string strBearTokenURL, string userID, string password, string grant_type)
        {
            try
            {
                var client = new HttpClient();
                if (AuthToken == null || IsTokenExpired())
                {
                    GetToken(strBearTokenURL, userID, password, grant_type);
                }
                return AuthToken.access_token;
            }
            catch
            {
                throw;
            }
        }


        private HttpClient HeadersForAccessTokenGenerate(string strBearTokenURL)
        {
            HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
            HttpClient client = new HttpClient(handler);
            try
            {
                client.BaseAddress = new Uri(strBearTokenURL);
                client.DefaultRequestHeaders.Accept.Clear();
                var authData = string.Format("{0}:{1}", ClientId, ClientSecret);
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeaderValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return client;
        }
    }
}