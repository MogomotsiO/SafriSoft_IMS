using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class TrialBalanceAccount: BaseModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }     
        public int Index { get; set; }
        public bool? IsGlobal { get; set; }
    }
}