using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniPension.Models
{
    public class TokenViewModel
    {
        public string username { get; set; }
        public string status { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public DateTime generateTime { get; set; }
        public string statusCode { get; set; }
        public string message { get; set; }
    }
}