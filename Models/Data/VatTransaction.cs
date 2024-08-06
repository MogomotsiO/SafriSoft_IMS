using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class VatTransaction: BaseModel
    {
        public DateTime Date { get; set; }
        public int TypeId { get; set; }
        public string Account { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public double Exclusive { get; set; }
        public double Inclusive { get; set; }
        public double TaxAmount { get; set; }
    }
}