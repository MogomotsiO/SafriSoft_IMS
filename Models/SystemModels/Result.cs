using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class Result
    {
        public int Number { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object obj { get; set; }
    }
}