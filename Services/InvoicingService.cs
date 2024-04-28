using SafriSoftv1._3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Services
{
    public class InvoicingService : BaseService
    {
        public InvoicingViewModel GetInvoiceDetails(int organisationId)
        {
            var vm = new InvoicingViewModel();

            vm.Organisation = GetOrganisationDetails(organisationId);

            vm.Customers = db.Customers.Where(x => x.OrganisationId == organisationId).ToList();

            return vm;
        }
    }
}