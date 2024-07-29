using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class JournalEntry: BaseModel
    {
        public int AccountId { get; set; }
        public string Narration { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public int VatOptionId { get; set; }
        public int JournalId { get; set; }
    }
}