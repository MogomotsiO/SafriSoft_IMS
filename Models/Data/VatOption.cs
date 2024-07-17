using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class VatOption : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Percentage { get; set; }
        public int TaxAccountId { get; set; }
    }
}