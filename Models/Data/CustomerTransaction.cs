using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class CustomerTransaction : BaseModel
    {
        public string Description { get; set; }
        public double Amount { get; set; }
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
    }
}