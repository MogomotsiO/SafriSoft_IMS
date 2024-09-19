using SafriSoftv1._3.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class ReportIncomeStatementAccount: BaseModel
    {
        public string Name { get; set; }
        public bool IsSubtotal { get; set; }
        public int SubtotalAccountId { get; set; }
        public MathOperatorType OperatorType { get; set; }
        public bool IsHeading { get; set; }
        public int HeadingAccountId { get; set; }
        public bool IsEmptySpace { get; set; }
        public int Index { get; set; }
        public bool IsGlobal { get; set; }
    }
}