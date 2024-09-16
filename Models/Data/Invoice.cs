using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class Invoice : BaseModel
    {
        public string InvoiceDescription { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Reference { get; set; }
        public double Shipping { get; set; }
        public double VatPercentage { get; set; }
        public int VatOptionId { get; set; }
        public double Amount { get; set; }
        public int CustomerId { get; set; }
        public int DebtorsAccountId { get; set; }
        public bool Paid { get; set; }
        public bool Repeating { get; set; }
        public string ProofOfPoayment { get; set; }
        public int InvoiceAccountId { get; set; }
        public string UserId { get; set; }
    }
}