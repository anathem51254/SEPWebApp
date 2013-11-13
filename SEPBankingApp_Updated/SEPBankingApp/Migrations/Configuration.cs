namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<SEPBankingApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SEPBankingApp.Models.ApplicationDbContext context)
        {
            //List<SEPBankingApp.Models.ApplicationUser> peopleDetails = new List<SEPBankingApp.Models.ApplicationUser>();

            //peopleDetails.Add(new Models.ApplicationUser() { } );

            //AddBankAccountToUser();

            //AddGeneralUser();

            //AddGeneralAdmin(1, "admin1@example.com", "admin", "admin", 12345678);

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }

        void AddGeneralUser()
        {
            string[] firstName = { "John", "Carla", "Bill", "William", "RZA", "GZA", "Iris", "Matt", "Motoko", "David", "Linh"};
            string[] lastName = { "Smith", "Jackson", "Murray", "Gibson", "WU", "WU", "Wu", "Shaw", "Kusanagi", "Mur", "Tran"};
            int[] phoneNumber = { 12345678, 12345678, 12345678, 12345678, 12345678, 12345678, 12345678, 12345678, 12345678, 12345678, 12345678 };

            for (int i = 0; i < 10; i++ )
            {
                var um = new UserManager<SEPBankingApp.Models.ApplicationUser>(new UserStore<SEPBankingApp.Models.ApplicationUser>(new SEPBankingApp.Models.ApplicationDbContext()));

                var user = new SEPBankingApp.Models.ApplicationUser()
                {
                    UserName = "user" + i.ToString(),
                    Email = "user" + i.ToString() + "@example.com",
                    FirstName = firstName[i],
                    LastName = lastName[i],
                    PhoneNumber = phoneNumber[i],
                };

                um.Create(user, "password");
                um.AddToRole(user.Id, "GeneralUser");
            }
        }

        void AddBankAccountToUser()
        {
            SEPBankingApp.Models.ApplicationDbContext db = new SEPBankingApp.Models.ApplicationDbContext();
            var Users = from tb in db.Users
                        select tb;
                        
            foreach(var user in Users)
            {
                SEPBankingApp.Models.BankAccount BankAccountSaver = new Models.BankAccount();

                BankAccountSaver.AccountType = "Saver";

                BankAccountSaver.BlockAccess = false;

                BankAccountSaver.CurrentBalance = 5000;

                BankAccountSaver.UserId = user.Id;

                db.BankAccounts.Add(BankAccountSaver);
                db.SaveChanges();

                SEPBankingApp.Models.BankAccount BankAccountAccess = new Models.BankAccount();

                BankAccountAccess.AccountType = "Access";

                BankAccountAccess.BlockAccess = false;

                BankAccountAccess.CurrentBalance = 5000;

                BankAccountAccess.UserId = user.Id;

                db.BankAccounts.Add(BankAccountAccess);
                db.SaveChanges();
            }

        }

        void AddTransactionHistory()
        {

            
        }

        void AddGeneralAdmin(int i, string email, string firstName, string lastName, int phoneNumber)
        {
            var um = new UserManager<SEPBankingApp.Models.ApplicationUser>(new UserStore<SEPBankingApp.Models.ApplicationUser>(new SEPBankingApp.Models.ApplicationDbContext()));

            var user = new SEPBankingApp.Models.ApplicationUser()
            {
                UserName = "admin" + i.ToString(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
            }; 
            
            um.Create(user, "password");
            um.AddToRole(user.Id, "GeneralAdmin");
        }

        bool AddUserAndRole()
        {
            IdentityResult ir;

            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new SEPBankingApp.Models.ApplicationDbContext()));

            ir = rm.Create(new IdentityRole("admin"));

            var um = new UserManager<SEPBankingApp.Models.ApplicationUser>(new UserStore<SEPBankingApp.Models.ApplicationUser>(new SEPBankingApp.Models.ApplicationDbContext()));

            var user = new SEPBankingApp.Models.ApplicationUser()
            {
                UserName = "admin1",
            };

            ir = um.Create(user, "password");

            if(ir.Succeeded == false)
                return ir.Succeeded;

            ir = um.AddToRole(user.Id, "admin");

            return ir.Succeeded;
        }

        void AddRole(string RoleName)
        {
            IdentityResult ir;

            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new SEPBankingApp.Models.ApplicationDbContext()));

            ir = rm.Create(new IdentityRole(RoleName));
        }

        void AddUserToRole(string id, string role)
        {
            var um = new UserManager<SEPBankingApp.Models.ApplicationUser>(new UserStore<SEPBankingApp.Models.ApplicationUser>(new SEPBankingApp.Models.ApplicationDbContext()));

            um.AddToRole(id, role);
        }

    }
}
