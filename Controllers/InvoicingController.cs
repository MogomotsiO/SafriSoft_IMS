using Rotativa;
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

        public ActionResult Invoice(int id)
        {

            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            var service = new InvoicingService();

            var vm = service.GetFullInvoiceDetails(id, organisationId);

            return View(vm);
        }

        public ActionResult InvoicePDF(int id, int organisationId)
        {
            var service = new InvoicingService();

            var vm = service.GetFullInvoiceDetails(id, organisationId);

            return View(vm);
        }

        public ActionResult CustomerInvoicePdf(int id, int organisationId)
        {
            return new ActionAsPdf("InvoicePDF", new { id = id, organisationId = organisationId })
            {
                FileName = Server.MapPath("CustomerInvoice.pdf")
            };
        }

    }
}