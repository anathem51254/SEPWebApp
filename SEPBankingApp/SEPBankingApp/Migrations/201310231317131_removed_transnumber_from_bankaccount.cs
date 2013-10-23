namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removed_transnumber_from_bankaccount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BankAccounts", "TransactionHistoryNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BankAccounts", "TransactionHistoryNumber", c => c.Int(nullable: false));
        }
    }
}
