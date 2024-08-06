using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class AccountingViewModel
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class GlAccountViewModel
    {
        public int Id { get; set; }
        public string DateStr { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime Date { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }

    public class DateParameters
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class UnmappedGlAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string DateStr { get; set; }
    }

    public class MappedGlAccounts
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class MapGeneralLedgerAccount
    {
        public int TrialBalanceAccountId { get; set; }
        public int GeneralLedgerId { get; set; }
    }

    public class TrialBalanceContainer
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Period { get; set; }
        public double Total { get; set; }
        public int Index { get; set; }
        public List<GeneralLedger> TrialBalanceItems { get; set; } = new List<GeneralLedger>();
    }

    public class SafriSoftAccounts
    {
        public string Date { get; set; }
        public int AccountNumber { get; set; }
        public string AccountName { get; set; }
        public double Amount { get; set; }
    }

    //move to report view model when created
    public class VatReport
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public string Account { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public double Exclusive { get; set; }
        public double Inclusive { get; set; }
        public double TaxAmount { get; set; }
    }
}