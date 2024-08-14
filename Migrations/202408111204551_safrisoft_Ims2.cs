namespace SafriSoftv1._3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class safrisoft_Ims2 : DbMigration
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
                        Street = c.String(),
                        Surburb = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        PostalCode = c.Int(nullable: false),
                        DateCustomerCreated = c.String(),
                        NumberOfOrders = c.Int(nullable: false),
                        Status = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        Description = c.String(),
                        Amount = c.Double(nullable: false),
                        InvoiceId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Expense",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(),
                        CategoryId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        AccountId = c.Int(nullable: false),
                        VatAccountId = c.Int(nullable: false),
                        VatOptionId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GeneralLedger",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(),
                        AccountReference = c.String(),
                        AccountName = c.String(),
                        AccountDescription = c.String(),
                        Amount = c.Double(nullable: false),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        AccountDate = c.DateTime(nullable: false),
                        Month = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
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
                "dbo.InvoiceItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Description = c.String(),
                        Qty = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Discount = c.Double(nullable: false),
                        VatOptionId = c.Int(nullable: false),
                        ItemAccountId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        InvoiceId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceDescription = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        InvoiceDueDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(),
                        Reference = c.String(),
                        Shipping = c.Double(nullable: false),
                        VatPercentage = c.Double(nullable: false),
                        VatOptionId = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        DebtorsAccountId = c.Int(nullable: false),
                        Paid = c.Boolean(nullable: false),
                        Repeating = c.Boolean(nullable: false),
                        ProofOfPoayment = c.String(),
                        InvoiceAccountId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                        AccountId = c.Int(nullable: false),
                        InvoiceId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JournalEntry",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Narration = c.String(),
                        Debit = c.Double(nullable: false),
                        Credit = c.Double(nullable: false),
                        VatOptionId = c.Int(nullable: false),
                        JournalId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Journal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Number = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        ActivatedDate = c.DateTime(),
                        ActivatedBy = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
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
                        Cost = c.Double(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        ItemsSold = c.Int(),
                        ItemsAvailable = c.Int(nullable: false),
                        Status = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportBalanceSheetAccountLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BsAccountId = c.Int(nullable: false),
                        TbAccountId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportBalanceSheetAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsSubtotal = c.Boolean(nullable: false),
                        SubtotalAccountId = c.Int(nullable: false),
                        IsHeading = c.Boolean(nullable: false),
                        IsEmptySpace = c.Boolean(nullable: false),
                        HeadingAccountId = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        IsGlobal = c.Boolean(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportIncomeStatementAccountLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAccountId = c.Int(nullable: false),
                        TbAccountId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportIncomeStatementAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsSubtotal = c.Boolean(nullable: false),
                        SubtotalAccountId = c.Int(nullable: false),
                        IsHeading = c.Boolean(nullable: false),
                        HeadingAccountId = c.Int(nullable: false),
                        IsEmptySpace = c.Boolean(nullable: false),
                        Index = c.Int(nullable: false),
                        IsGlobal = c.Boolean(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Value = c.String(),
                        SettingType = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupplierInvoice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceDate = c.DateTime(),
                        Number = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        FileContentType = c.String(),
                        Qty = c.Int(nullable: false),
                        Amount = c.Double(nullable: false),
                        VatAmount = c.Double(nullable: false),
                        VatAccountId = c.Int(nullable: false),
                        InvoiceAccountId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Supplier",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        TradingAs = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Street = c.String(),
                        Surburb = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        PostalCode = c.Int(nullable: false),
                        ContactPersonName = c.String(),
                        ContactPersonPosition = c.String(),
                        ContactPersonPhone = c.String(),
                        ContactPersonEmail = c.String(),
                        ProductId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupplierTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        Description = c.String(),
                        Amount = c.Double(nullable: false),
                        SupplierInvoiceId = c.Int(nullable: false),
                        SupplierId = c.Int(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialBalanceAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(),
                        AccountName = c.String(),
                        Index = c.Int(nullable: false),
                        IsGlobal = c.Boolean(),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TrialBalanceGeneralLedgerMapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrialBalanceAccountId = c.Int(nullable: false),
                        GeneralLedgerId = c.Int(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VatOption",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Percentage = c.Double(nullable: false),
                        TaxAccountId = c.Int(nullable: false),
                        IsGloabl = c.Boolean(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VatTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        TypeId = c.Int(nullable: false),
                        Account = c.String(),
                        Reference = c.String(),
                        Description = c.String(),
                        Exclusive = c.Double(nullable: false),
                        Inclusive = c.Double(nullable: false),
                        TaxAmount = c.Double(nullable: false),
                        Inserted = c.DateTime(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VatTransaction");
            DropTable("dbo.VatOption");
            DropTable("dbo.TrialBalanceGeneralLedgerMapping");
            DropTable("dbo.TrialBalanceAccount");
            DropTable("dbo.SupplierTransaction");
            DropTable("dbo.Supplier");
            DropTable("dbo.SupplierInvoice");
            DropTable("dbo.Setting");
            DropTable("dbo.ReportIncomeStatementAccount");
            DropTable("dbo.ReportIncomeStatementAccountLink");
            DropTable("dbo.ReportBalanceSheetAccount");
            DropTable("dbo.ReportBalanceSheetAccountLink");
            DropTable("dbo.Product");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderAudit");
            DropTable("dbo.Journal");
            DropTable("dbo.JournalEntry");
            DropTable("dbo.InvoiceTransaction");
            DropTable("dbo.Invoice");
            DropTable("dbo.InvoiceItem");
            DropTable("dbo.InboxReplies");
            DropTable("dbo.InboxMessages");
            DropTable("dbo.GeneralLedger");
            DropTable("dbo.Expense");
            DropTable("dbo.CustomerTransaction");
            DropTable("dbo.Customer");
        }
    }
}
