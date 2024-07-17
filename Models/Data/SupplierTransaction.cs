using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class SupplierTransaction: BaseModel
    {
        public string Description { get; set; }
        public double Amount { get; set; }
        public int SupplierInvoiceId { get; set; }
        public int SupplierId { get; set; }
    }
}