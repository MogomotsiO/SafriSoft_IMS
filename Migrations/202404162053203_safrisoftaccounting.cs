namespace SafriSoftv1._3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class safrisoftaccounting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeneralLedger",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(),
                        AccountName = c.String(),
                        Amount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialBalanceAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(),
                        AccountName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialBalanceGeneralLedgerMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrialBalanceAccountId = c.Int(nullable: false),
                        GeneralLedgerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TrialBalanceGeneralLedgerMapping");
            DropTable("dbo.TrialBalanceAccount");
            DropTable("dbo.GeneralLedger");
        }
    }
}
