using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class ProductRequirement : BaseModel
    {
        public string ProductName { get; set; }
        public int RequiredProductId { get; set; }
        public int QtyRequired { get; set; }
        public int ProductId { get; set; }
    }
}