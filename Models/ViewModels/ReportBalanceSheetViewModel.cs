using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.ViewModels
{
    public class ReportBalanceSheetViewModel
    {
        public List<ReportBalanceSheetAccount> HeadingAccounts { get; set; } = new List<ReportBalanceSheetAccount>();
        public List<ReportBalanceSheetAccount> SubtotalAccounts { get; set; } = new List<ReportBalanceSheetAccount>();
    }

    public class ReportBalanceSheetDetailViewModel
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public int HeadingAccountId { get; set; }
        public string HeadingAccountName { get; set; }
        public int SubtotalAccountId { get; set; }
        public string SubtotalAccountName { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsHeading { get; set; }
        public bool IsEmptySpace { get; set; }
        public int Index { get; set; }
    }

    public class ReportBalanceSheetLinkedAccounts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }

    public class ReportBalanceSheetUnlinkedAccounts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }

    public class BalanceSheetViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ReportViewType Type { get; set; }
    }

    public class BalanceSheetContainer
    {
        public List<string> Coloumns { get; set; } = new List<string>();
        public List<BalanceSheet> Items { get; set; }
    }

    public class BalanceSheet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHeading { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsEmptySpace { get; set; }
        public int SubtotalAccountId { get; set; }
        public Dictionary<string, double> Balances { get; set; } = new Dictionary<string, double>();
    }

}