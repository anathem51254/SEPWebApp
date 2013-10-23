using Microsoft.AspNet.Identity.EntityFramework;

namespace SEPBankingApp.Models
{
    // You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : User
    {  
    }

    public class ApplicationDbContext : IdentityDbContextWithCustomUser<ApplicationUser>
    {
        public System.Data.Entity.DbSet<SEPBankingApp.Models.BankAccount> BankAccounts { get; set; }
        public System.Data.Entity.DbSet<SEPBankingApp.Models.TransactionHistory> TransactionHistorys { get; set; }
    }
}