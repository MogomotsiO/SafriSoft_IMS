using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
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
        public int Type { get; set; }
    }

    public class BalanceSheetYearly
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHeading { get; set; }
        public bool IsSubtotal { get; set; }
        public int SubtotalAccountId { get; set; }
        public string YearOne { get; set; }
        public double YearOneBalance { get; set; }
        public string YearTwo { get; set;}
        public double YearTwoBalance { get; set;}
        public string YearThree { get; set;}
        public double YearThreeBalance { get; set;}
    }

    public class BalanceSheetMonthly
    {
        public string Name { get; set; }
        public string MonthOne { get; set; }
        public double MonthBalance { get; set; }
        public string MonthTwo { get; set; }
        public double MonthTwoBalance { get; set; }
        public string MonthThree { get; set; }
        public double MonthThreeBalance { get; set; }
        public string MonthFour { get; set; }
        public double MonthFourBalance { get; set; }
        public string MonthFive { get; set; }
        public double MonthFiveBalance { get; set; }
        public string MonthSix { get; set; }
        public double MonthSixBalance { get; set; }
        public string MonthSeven { get; set; }
        public double MonthSevenBalance { get; set; }
        public string MonthEight { get; set; }
        public double MonthEightBalance { get; set; }
        public string MonthNine { get; set; }
        public double MonthNineBalance { get; set; }
        public string MonthTen { get; set; }
        public double MonthTenBalance { get; set; }
        public string MonthEleven { get; set; }
        public double MonthElevenBalance { get; set; }
        public string MonthTwelve { get; set; }
        public double MonthTwelveBalance { get; set; }
    }
}