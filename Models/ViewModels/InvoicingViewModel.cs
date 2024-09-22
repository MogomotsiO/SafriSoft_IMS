using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class InvoicingViewModel
    {
        public Organisations Organisation { get; set; } 
        public int CustomerId { get; set; }
        public List<Customer> Customers { get; set; }
        public Product Product { get; set; }
        public Invoice InvoiceDetails { get; set; } = new Invoice();
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public List<VatOption> VatOptions { get; set; } = new List<VatOption>();
        public List<TrialBalanceAccount> TrialBalanceAccounts { get; set; } = new List<TrialBalanceAccount>();
        public List<InvoiceTransaction> InvoiceTransactions { get; set; } = new List<InvoiceTransaction>();
        public List<Product> Products { get; set; } = new List<Product>();
        public bool CreateOrder { get; set; }
    }

    public class InvoiceDetalsVm
    {
        public int Id { get; set; }
        public string DateIssuedStr { get; set; }
        public string DateDueStr { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceName { get; set; }
        public string Customer { get; set; }
        public double Amount { get; set; }
        public int OrganisationId { get; set; }
        public bool Paid { get; set; }
        public bool OverDueInvoice { get; set; }
        public double AmountPaid { get; set; }
        public string Pop { get; set; }
    }

    public class InvoiceDetails
    {
        public string InvoiceDescription { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reference { get; set; }
        public double Shipping { get; set; }
        public double VatPercentage { get; set; }
        public double Amount { get; set; }
        public int CustomerId { get; set; }
    }

    public class PayInvoiceDetailsVm
    {
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
    }
}