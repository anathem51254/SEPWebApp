namespace SEPBankingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lol : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.TransactionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TransactionHistories");
        }
    }
}
