using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEPBankingApp.Models
{
    public class UserDetailsViewModels
    {
        public ApplicationUser AppUser;
        public IQueryable<BankAccount> BankAccount;
        public IList<string> Roles;
    }
}