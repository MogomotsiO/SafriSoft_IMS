﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class ReportIncomeStatementAccountLink: BaseModel
    {
        public int IsAccountId { get; set; }
        public int TbAccountId { get; set; }
    }
}