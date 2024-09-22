using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class Journal : BaseModel
    {
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public string ActivatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}