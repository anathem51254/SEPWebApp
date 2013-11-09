namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<SEPBankingApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SEPBankingApp.Models.ApplicationDbContext context)
        {
            //AddRole("GeneralUser");
            //AddRole("GeneralAdmin");

            AddUserToRole("2b7e6280-65a7-4778-a200-39c1804d0a63", "GeneralAdmin");

            //AddUserAndRole();

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
