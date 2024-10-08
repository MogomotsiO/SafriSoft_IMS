﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Models
{
    public class BaseViewModel
    {
        public string JsonString { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
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

    public class AuditViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Changed { get; set; }
        public string CreatedDate { get; set; }
        public string UserId { get; set; }
        public int OrderId { get; set; }
    }
}