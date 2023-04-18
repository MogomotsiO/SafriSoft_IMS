namespace SafriSoftv1._3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class safrisoftIms : DbMigration
    {
        public override void Up()
        {
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
        }
        
        public override void Down()
        {
        }
    }
}
