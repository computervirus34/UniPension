//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniPension.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppUser
    {
        public int id { get; set; }
        public string userId { get; set; }
        public string userPass { get; set; }
        public string Email { get; set; }
        public string CellNo { get; set; }
        public int branchId { get; set; }
        public string isActive { get; set; }
        public string firstLogin { get; set; }
        public string userType { get; set; }
        public string name { get; set; }
        public Nullable<System.DateTime> createdby { get; set; }
        public System.DateTime createdon { get; set; }
        public Nullable<System.DateTime> updatedby { get; set; }
        public System.DateTime updatedon { get; set; }
    
        public virtual Branch Branch { get; set; }
    }
}