namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_discriminator_to_nullable : DbMigration
    {
        public override void Up()
        {

            AlterColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: true, maxLength: 128), null);

            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        AccountNumber = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        TransactionHistoryNumber = c.Int(nullable: false),
                        AccountType = c.String(nullable: false),
                        CurrentBalance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.AccountNumber);
            
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Int(nullable: false),
                        DestinationAccountNumber = c.Int(nullable: false),
                        TransactionDateTime = c.DateTime(nullable: false),
                        TransactionAmount = c.Double(nullable: false),
                        PreBalance = c.Double(nullable: false),
                        PostBalance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.BankAccounts", t => t.AccountNumber, cascadeDelete: true)
                .Index(t => t.AccountNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionHistories", "AccountNumber", "dbo.BankAccounts");
            DropIndex("dbo.TransactionHistories", new[] { "AccountNumber" });
            DropTable("dbo.TransactionHistories");
            DropTable("dbo.BankAccounts");
            AlterColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128), null);
        }
    }
}
