using SafriSoftv1._3.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.Data
{
    public class Expense: BaseModel
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public NominalAccountType Type { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
        public int VatAccountId { get; set; }
        public int VatOptionId { get; set; }
        public int BankAccountId { get; set; }
    }
}