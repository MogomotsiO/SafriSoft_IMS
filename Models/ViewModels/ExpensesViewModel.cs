using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.ViewModels
{
    public class ExpensesViewModel
    {
        public List<Setting> categories { get; set; }
        public List<TrialBalanceAccount> Accounts { get; set; }
        public List<VatOption> VatOptions { get; set; }
    }

    public class ExpensesViewModelItems
    {
        public string Date { get; set; }
        public string Name { get; set; }        
        public string Category { get; set; }
        public double Amount { get; set; }
        public string AccountName { get; set; }
        public double VatAmount { get; set; }
        public string VatAccountName { get; set; }
        public string BankAccountName { get;set; }
    }

    public class ExpensesViewModelItem
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public int AccountId { get; set; }
        public int VatAccountId { get; set; }
        public int VatOptionId { get; set; }
        public int BankAccountId { get; set; }
    }
}