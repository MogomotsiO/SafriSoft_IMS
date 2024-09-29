using Newtonsoft.Json;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Invoicing")]
    public class InvoicingController : BaseApiController
    {
        [HttpGet, Route("GetInvoiceDetails/{id}")]
        public IHttpActionResult GetInvoiceDetails(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new InvoicingService();

                result.obj = service.GetFullInvoiceDetails(id, organisationId);

                result.Success = true;
                result.Message = "Invoice retrieved";

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("GetInvoices")]
        public IHttpActionResult GetInvoices(DateParameters vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new InvoicingService();

                result.obj = service.GetInvoices(organisationId, vm);

                result.Success = true;
                result.Message = "Invoices retrieved";

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("SaveInvoice")]
        public IHttpActionResult SaveInvoice(BaseViewModel vm)
        {
            var result = new Result();

            try
            {
                var invoiceVm = JsonConvert.DeserializeObject<InvoicingViewModel>(vm.JsonString);
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var service = new InvoicingService();

                result = service.SaveInvoice(invoiceVm, organisationId, userId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("SendInvoiceEmail")]
        public IHttpActionResult SendInvoiceEmail(BaseEmailViewModel vm)
        {
            var result = new Result();

            try
            {
                var db = new SafriSoftDbContext();
                var createEmail = new SafriSoftEmailService();
                var toAddress = new List<string>();
                var toCCAddress = new List<string>();

                var invoiceDetails = db.Invoices.FirstOrDefault(x => x.Id == vm.InvoiceId);

                var customerDetails = db.Customers.FirstOrDefault(x => x.Id == invoiceDetails.CustomerId);

                toAddress.Add(customerDetails.CustomerEmail);

                var subject = vm.EmailSubject;

                var downloadLink = $"https://ims.safrisoft.com/Invoicing/CustomerInvoicePdf?id={invoiceDetails.Id}&organisationId={invoiceDetails.OrganisationId}";
                var emailBody = $"{vm.EmailBody} <br /><br/> Download invoice here: <a href='{downloadLink}'>Invoice</a>";

                createEmail.SaveEmail(subject, emailBody, "support@safrisoft.com", toAddress.ToArray(), toCCAddress.ToArray());

                return Json(new { Success = true, Message = "Email has been sent" });
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("PayInvoice/{Id}/{Paid}")]
        public IHttpActionResult PayInvoice(int Id, bool Paid)
        {
            var result = new Result();
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["files[]"];

            if (postedFile == null)
            {
                result.Success = false;
                result.Message = "File could not be processed";
                return Json(result);
            }

            string fileName = postedFile.FileName;
            string fileContentType = postedFile.ContentType;

            var saveFileDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Documents/InvoicesProofOfPayment");

            //var saveFileDir = $"{AppDomain.CurrentDomain.BaseDirectory}/Documents/InvoicesProofOfPayment";

            if (Directory.Exists(saveFileDir) == false)
            {
                Directory.CreateDirectory(saveFileDir);
            }

            var fullFileName = $"{saveFileDir}/{fileName}";

            try
            {
                postedFile.SaveAs(fullFileName);

                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var ivSvc = new InvoicingService();

                result = ivSvc.PayInvoice(Id, fileName, Paid, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("PayInvoiceDetails")]
        public IHttpActionResult PayInvoiceDetails(PayInvoiceDetailsVm vm)
        {
            var result = new Result();            

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var ivSvc = new InvoicingService();

                result = ivSvc.PayInvoiceDetails(vm, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetOverdueInvoices")]
        public IHttpActionResult GetOverdueInvoices()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new InvoicingService();

                result.obj = service.GetInvoices(organisationId).Where(x => x.OverDueInvoice == true).ToList();

                result.Success = true;
                result.Message = "Invoices retrieved";

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }
    }
}