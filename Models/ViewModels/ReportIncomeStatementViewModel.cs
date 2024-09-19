using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.SystemModels;
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
        public MathOperatorType OperatorType { get; set; }
        public string OperatorTypeName { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsHeading { get; set; }
        public bool IsEmptySpace { get; set; }
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

    public class IncomeStatementViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public ReportViewType Type { get; set; }
    }

    public class IncomeStatementContainer
    {
        public List<string> Coloumns { get; set; } = new List<string>();
        public List<IncomeStatement> Items { get; set; }
    }

    public class IncomeStatement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHeading { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsEmptySpace { get; set; }
        public int SubtotalAccountId { get; set; }
        public MathOperatorType OperatorType { get; set; }
        public List<Balance> Balances { get; set; } = new List<Balance>();
        public Dictionary<string, double> ViewBalances { get; set; } = new Dictionary<string, double>();
    }

    public class Balance
    {
        public string Year { get; set; }
        public MathOperatorType OperatoryType { get; set; }
        public double Amount { get; set; }

    }
}