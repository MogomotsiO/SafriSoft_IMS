namespace SafriSoftv1._3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SafriSoftDbContext_IMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CustomerEmail = c.String(),
                        CustomerCell = c.String(),
                        CustomerAddress = c.String(),
                        DateCustomerCreated = c.String(),
                        NumberOfOrders = c.Int(nullable: false),
                        Status = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InboxMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Body = c.String(),
                        From = c.String(),
                        To = c.String(),
                        Status = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InboxReplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Body = c.String(),
                        From = c.String(),
                        To = c.String(),
                        Status = c.String(),
                        MessageId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderAudit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Changed = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UserId = c.String(),
                        OrderId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.String(),
                        ProductName = c.String(),
                        NumberOfItems = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        CustomerName = c.String(),
                        OrderStatus = c.String(),
                        OrderProgress = c.Int(nullable: false),
                        DateOrderCreated = c.String(),
                        ExpectedDeliveryDate = c.String(),
                        Status = c.String(),
                        OrderWorth = c.Decimal(precision: 18, scale: 2),
                        ShippingCost = c.Decimal(precision: 18, scale: 2),
                        UserId = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        ProductReference = c.String(),
                        ProductCode = c.String(),
                        ProductCategory = c.String(),
                        ProductImage = c.String(),
                        SellingPrice = c.Double(nullable: false),
                        ItemsSold = c.Int(),
                        ItemsAvailable = c.Int(nullable: false),
                        Status = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Product");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderAudit");
            DropTable("dbo.InboxReplies");
            DropTable("dbo.InboxMessages");
            DropTable("dbo.Customer");
        }
    }
}
