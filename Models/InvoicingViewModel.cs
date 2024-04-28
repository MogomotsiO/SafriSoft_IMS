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
        public Invoice InvoiceDetails { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}