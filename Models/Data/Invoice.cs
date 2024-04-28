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
        public int InvoiceNumber { get; set; }
        public string Reference { get; set; }
        public int CustomerId { get; set; }
    }
}