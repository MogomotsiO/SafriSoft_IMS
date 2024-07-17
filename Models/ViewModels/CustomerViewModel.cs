using SafriSoftv1._3.Models.Data;
using System.Collections.Generic;

namespace SafriSoftv1._3.Models
{
    public class CustomerContainerViewModel
    {
        public List<TrialBalanceAccount> TrialBalanceAccounts { get; set; } = new List<TrialBalanceAccount>();
        public List<VatOption> VatOptions { get; set; } = new List<VatOption>();
    }
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCell { get; set; }
        public string CustomerAddress { get; set; }
        public string DateCustomerCreated { get; set; }
        public int NumberOfOrders { get; set; }
        public double Balance { get; set; }
    }
}