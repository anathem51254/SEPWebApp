using Microsoft.AspNet.Identity.EntityFramework;

namespace SEPBankingApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public System.Data.Entity.DbSet<SEPBankingApp.Models.BankAccount> BankAccounts { get; set; }
        public System.Data.Entity.DbSet<SEPBankingApp.Models.TransactionHistory> TransactionHistorys { get; set; }
    }
}