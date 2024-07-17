using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models.ViewModels
{
    public class SettingViewModel
    {
        public List<TrialBalanceAccount> Accounts { get; set; }
    }

    public class VatOptionVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Percentage { get; set; }
        public int TaxAccountId { get; set; }
        public string TaxAccount { get; set; }
    }

    public class SettingVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}