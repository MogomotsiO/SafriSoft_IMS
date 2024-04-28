using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class TrialBalanceAccount
    {
        [Key]
        public int Id { get; set; }
        public Int64 AccountNumber { get; set; }
        public string AccountName { get; set; }     
        public int Index { get; set; }
        public int OrganisationId { get; set; }
    }
}