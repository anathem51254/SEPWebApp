namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blockacessaddedtobankaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "BlockAccess", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccounts", "BlockAccess");
        }
    }
}
