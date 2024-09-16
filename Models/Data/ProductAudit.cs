using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class ProductAudit : BaseModel
    {
        public string Description { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
    }
}