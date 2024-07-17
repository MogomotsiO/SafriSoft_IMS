using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class InvoiceItem: BaseModel
    {
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int VatOptionId { get; set; }
        public int ItemAccountId { get; set; }
        public double Amount { get; set; }
        public int InvoiceId { get; set; }
    }
}