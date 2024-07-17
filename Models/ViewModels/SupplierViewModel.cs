using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class SupplierViewModel
    {
        public List<Supplier> Suppliers { get; set; }
        public List<SupplierInvoice> SuppliersInvoices { get; set; }
        public Supplier Supplier { get; set; }
        public SupplierInvoice SupplierInvoice { get; set; }
        public List<TrialBalanceAccount> TrialBalanceAccounts { get; set; }
    }

    public class SupplierDetailsViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string TradingAs { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string Surburb { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int PostalCode { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonPhone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ProductName { get; set; }
        public double Balance { get;set; }
        public List<Product> Products { get; set; }
        public List<TrialBalanceAccount> TrialBalanceAccounts { get; set; }
    }

    public class SupplierInvoiceDetailViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public int Qty { get; set; }
        public double VatAmount { get; set; }
        public int VatAccountId { get; set; }
        public double Amount { get; set; }
        public int AmountAccountId { get;}
    }

    public class DocumentDetails
    {
        public int Qty { get; set; }
        public double VatAmount { get; set; }
        public double Amount { get; set; }
    }

    public class PaySupplierViewModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set;}
        public int SupplierId { get; set;}
        public int SupplierInvoiceId { get; set;}
        public int CreditorsAccountId { get; set; }
    }
}