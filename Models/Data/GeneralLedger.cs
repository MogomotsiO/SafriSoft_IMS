using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.Data
{
    public class GeneralLedger
    {
        [Key]
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public double Amount { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public DateTime AccountDate { get; set; }
        public string DateStr { get { return AccountDate.ToString("dd/MM/yyyy"); } }
        public int Month { get; set; }
        public int Year { get; set; }
        public int OrganisationId { get; set; }
    }
}