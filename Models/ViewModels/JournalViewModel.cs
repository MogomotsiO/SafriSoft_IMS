using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace SafriSoftv1._3.Models.ViewModels
{
    public class JournalViewModel
    {
        public List<TrialBalanceAccount> TrialBalanceAccounts { get; set; } = new List<TrialBalanceAccount>();
    }

    public class JournalDetailViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public List<JournalEntryViewModel> Entries { get; set; } = new List<JournalEntryViewModel>();
    }

    public class JournalListViewModel
    {
        public int Id { get; set; }
        public string DateStr { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public double Balance { get; set; }
        public string CreatedDateStr { get; set; }
        public string CreatedBy { get; set; }
        public string ActivatedDateStr { get; set; }
        public string ActivatedBy { get;set; }
        public bool IsActive { get; set; }
        public List<JournalEntry> Entries { get; set; } = new List<JournalEntry>();
    }

    public class JournalEntryViewModel
    {
        public string JournalNumber { get; set; }
        public string DateStr { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
        public string Narration { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
    }
}