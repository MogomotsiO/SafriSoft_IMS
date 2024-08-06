using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Models.SystemModels
{
    public class Enums
    {
    }

    public enum NominalAccountType
    {
        Income = 1,
        Expense = 2,
    }

    public enum SettingType
    {
        ExpenseCategories = 1,
    }

    public enum ReportViewType
    {
        Yearly = 1,
        Monthly = 2
    }

    public enum MonthFull
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum MonthShort
    {
        Jan = 1,
        Feb = 2,
        Mar = 3,
        Apr = 4,
        May = 5,
        Jun = 6,
        Jul = 7,
        Aug = 8,
        Sep = 9,
        Oct = 10,
        Nov = 11,
        Dec = 12
    }
}