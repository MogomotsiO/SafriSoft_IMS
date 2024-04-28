using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class TrialBalanceGeneralLedgerMapping
    {
        [Key]
        public int Id { get; set; }
        public int TrialBalanceAccountId { get; set; }
        public int GeneralLedgerId { get; set; }
        public int OrganisationId { get; set; }
    }
}