using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SEPBankingApp.Models
{

    public class RoleManagement
    {
        //public string Id { get; set; }

        [Required]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

    }

    public class UserManagement
    {
    }

    public class BankAccountManagement
    {
    }
}