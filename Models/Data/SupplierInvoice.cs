using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class SupplierInvoice : BaseModel
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public int Qty { get; set; }
        public double Amount { get; set; }
        public double VatAmount { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
    }
}