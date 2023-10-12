using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UniPension.Models
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Please enter the User Name")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Please enter the Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int BranchID { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        [Display(Name = "User Status")]
        public string UserStatus { get; set; }
        [Display(Name = "User Mobile")]
        public string UserMobile { get; set; }

        public string UserType { get; set; }
        public string UserMail { get; set; }
        public string FirstLogin { get; set; }

        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string userIPAddress { get; set; }

        public string agentID { get; set; }
        public string agentSequence { get; set; }

        public string token { get; set; }
        public string RoutingNo { get; set; }
    }
}