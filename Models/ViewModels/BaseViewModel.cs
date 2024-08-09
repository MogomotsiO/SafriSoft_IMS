using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class BaseViewModel
    {
        public string JsonString { get; set; }
    }

    public class BaseEmailViewModel
    {
        public int InvoiceId { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }

    public class AddNewCompanyViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyAddress { get; set; }
        public int CopmanyPostalCode { get; set; }
        public string CompanyProvince { get; set; }
        public string CompanyCity { get; set; }
    }
}