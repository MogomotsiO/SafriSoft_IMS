using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class InvoiceTransaction: BaseModel
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
        public int InvoiceId { get; set; }
    }
}