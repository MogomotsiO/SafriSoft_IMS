using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class InvoicingController : BaseController
    {
        // GET: Invoicing
        public ActionResult Create()
        {
            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new InvoicingService();

                var vm = service.GetInvoiceDetails(organisationId);

                return View(vm);
            }
            catch (Exception ex)
            {
                return View();
            }

            
        }

        public ActionResult Invoices()
        {
            return View();
        }
    }
}