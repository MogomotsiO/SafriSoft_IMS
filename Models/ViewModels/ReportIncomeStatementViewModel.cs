using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.ViewModels
{
    public class ReportIncomeStatementViewModel
    {
        public List<ReportIncomeStatementAccount> HeadingAccounts { get; set; } = new List<ReportIncomeStatementAccount>();
        public List<ReportIncomeStatementAccount> SubtotalAccounts { get; set; } = new List<ReportIncomeStatementAccount>();
    }

    public class ReportIncomeStatementDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HeadingAccountId { get; set; }
        public string HeadingAccountName { get; set; }
        public int SubtotalAccountId { get; set; }
        public string SubtotalAccountName { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsHeading { get; set; }
        public int Index { get; set; }
    }

    public class ReportIncomeStatementLinkedAccounts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }

    public class ReportIncomeStatementUnlinkedAccounts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }
}