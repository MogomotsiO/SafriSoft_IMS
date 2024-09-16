using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class CustomerDocument : BaseModel
    {
        public string FileName { get; set; }
        public int CustomerId { get; set; }
    }
}