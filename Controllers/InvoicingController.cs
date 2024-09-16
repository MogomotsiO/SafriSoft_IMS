using Newtonsoft.Json;
using Rotativa;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.IO;
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
            var vm = new InvoicingViewModel();

            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            var aSvc = new AccountingService();

            var accounts = aSvc.GetTrialBalanceAccounts(organisationId);

            var json = JsonConvert.SerializeObject(accounts);

            vm.TrialBalanceAccounts = JsonConvert.DeserializeObject<List<TrialBalanceAccount>>(json);

            return View(vm);
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

        public ActionResult DownloadPopFile(int Id)
        {
            var Isvc = new InvoicingService();

            var filename = Isvc.GetProofOfPayment(Id);

            var saveFileDir = $@"{AppDomain.CurrentDomain.BaseDirectory}\\Documents\\InvoicesProofOfPayment";

            var fullFileName = $@"{saveFileDir}\\{filename}";

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullFileName);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

        }
        
    }
}