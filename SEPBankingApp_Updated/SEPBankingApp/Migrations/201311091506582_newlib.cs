namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newlib : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                   "dbo.AspNetUsers",
                   c => new
                   {
                       Id = c.String(nullable: false, maxLength: 128),
                       UserName = c.String(),
                       PasswordHash = c.String(),
                       SecurityStamp = c.String(),
                       Discriminator = c.String(nullable: false, maxLength: 128),
                   })
                   .PrimaryKey(t => t.Id);
            DropTable("dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        Email = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUserClaims", "UserId", c => c.String());
            AddColumn("dbo.AspNetUserClaims", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetUserLogins", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetUserRoles", "Id", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUserRoles", "IdentityRole_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUserRoles", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AspNetUserRoles", "IdentityRole_Id");
            CreateIndex("dbo.AspNetUserClaims", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUserLogins", "ApplicationUser_Id");
            CreateIndex("dbo.AspNetUserRoles", "ApplicationUser_Id");
            AddForeignKey("dbo.AspNetUserRoles", "IdentityRole_Id", "dbo.AspNetRoles", "Id");
            AddForeignKey("dbo.AspNetUserClaims", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.AspNetUserClaims", "User_Id");
           
        }
        
        public override void Down()
        {
            DropTable("dbo.AspNetUsers");
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUserClaims", "User_Id", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.AspNetUserRoles", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "IdentityRole_Id", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "IdentityRole_Id" });
            DropColumn("dbo.AspNetUserRoles", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUserRoles", "IdentityRole_Id");
            DropColumn("dbo.AspNetUserRoles", "Id");
            DropColumn("dbo.AspNetUserLogins", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUserClaims", "ApplicationUser_Id");
            DropColumn("dbo.AspNetUserClaims", "UserId");
           
            CreateIndex("dbo.AspNetUserClaims", "User_Id");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            AddForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
