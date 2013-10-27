namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stuff : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransactionHistories", "AccountNumber", "dbo.BankAccounts");
            DropIndex("dbo.TransactionHistories", new[] { "AccountNumber" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.TransactionHistories", "AccountNumber");
            AddForeignKey("dbo.TransactionHistories", "AccountNumber", "dbo.BankAccounts", "AccountNumber", cascadeDelete: true);
        }
    }
}
