using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class SupplierInvoice : BaseModel
    {
        public DateTime? InvoiceDate { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public int Qty { get; set; }
        public double Amount { get; set; }
        public double VatAmount { get; set; }
        public int VatAccountId { get; set; }
        public int InvoiceAccountId { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime? StockLoaded {get;set;}
        public string StockLoadedUser { get; set; }
    }
}