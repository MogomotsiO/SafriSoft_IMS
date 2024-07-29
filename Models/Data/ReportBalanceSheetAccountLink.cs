using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class ReportBalanceSheetAccountLink:BaseModel
    {
        public int BsAccountId { get; set; }
        public int TbAccountId { get; set; }
    }
}