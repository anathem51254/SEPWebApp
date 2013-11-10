namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedtransdescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionHistories", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionHistories", "Description");
        }
    }
}
