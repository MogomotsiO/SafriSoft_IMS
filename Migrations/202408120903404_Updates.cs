namespace SafriSoftv1._3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customer", "PostalCode", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customer", "PostalCode", c => c.Int(nullable: false));
        }
    }
}
