using Newtonsoft.Json;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
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

        public InvoicingViewModel GetFullInvoiceDetails(int invoiceId, int organisationId)
        {
            var vm = new InvoicingViewModel();

            vm.InvoiceDetails = db.Invoices.Where(x => x.Id == invoiceId).FirstOrDefault();

            vm.InvoiceItems = db.InvoiceItems.Where(x => x.InvoiceId == vm.InvoiceDetails.Id).ToList();

            vm.Organisation = GetOrganisationDetails(organisationId);

            vm.Customers = db.Customers.Where(x => x.OrganisationId == organisationId && x.Id == vm.InvoiceDetails.CustomerId).ToList();

            return vm;
        }

        public List<InvoiceDetalsVm> GetInvoices(int organisationId)
        {
            var vm = new List<InvoiceDetalsVm>();

            var invoices = db.Invoices.Where(x => x.OrganisationId == organisationId).ToList();

            foreach (var invoice in invoices)
            {
                vm.Add(new InvoiceDetalsVm()
                {
                    DateIssuedStr = invoice.InvoiceDate.ToString("dd/MM/yyyy"),
                    DateDueStr = invoice.InvoiceDueDate.ToString("dd/MM/yyyy"),
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceName = invoice.InvoiceDescription,
                    Customer = db.Customers.Where(x => x.Id == invoice.CustomerId).FirstOrDefault().CustomerName,
                    Id = invoice.Id,
                    Amount = invoice.Amount,
                    OrganisationId = organisationId,
                    Paid = invoice.Paid
                });
            }

            return vm;
        }

        public Result SaveInvoice(InvoicingViewModel vm, int organisationId)
        {
            var result = new Result();

            vm.InvoiceDetails.InvoiceNumber = $"INV-{GetNextInvoiceNumber(organisationId)}";
            vm.InvoiceDetails.OrganisationId = organisationId;
            vm.InvoiceDetails.Inserted = DateTime.Now;
            vm.InvoiceDetails.Updated = DateTime.Now;

            db.Invoices.Add(vm.InvoiceDetails);

            var res = db.SaveChanges();

            if(res > 0)
            {
                foreach (var item in vm.InvoiceItems)
                {
                    item.InvoiceId = vm.InvoiceDetails.Id;
                    item.Inserted = DateTime.Now;
                    item.Updated = DateTime.Now;

                    db.InvoiceItems.Add(item);
                }
            }            

            res = db.SaveChanges();

            if (res > 0)
            {
                result.Success = true;
                result.Message = "Successfully save invoice";
            }
            else
            {
                result.Success = true;
                result.Message = "Could not save invoice";
            }

            return result;

        }

        public int GetNextInvoiceNumber(int organisationId)
        {
            int invoiceNumber = 1;

            var lastInvoice = db.Invoices.Where(x => x.OrganisationId == organisationId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (lastInvoice != null)
            {
                var lastInvoiceNumber = lastInvoice.InvoiceNumber;

                invoiceNumber = Convert.ToInt32(lastInvoiceNumber.Substring(lastInvoiceNumber.IndexOf('-') + 1));

                invoiceNumber += 1;
            }

            return invoiceNumber;
        }

        public Result PayInvoice(int id, string popFileName, bool paid, int organisationId)
        {
            var result = new Result();

            var invoice = db.Invoices.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();

            invoice.Paid = paid;
            invoice.ProofOfPoayment = popFileName;

            var res = db.SaveChanges();

            if (res > 0)
            {
                result.Success = true;
                result.Message = "Invoice updated";
                return result;
            }
            else
            {
                result.Success = false;
                result.Message = "Could not update invoice";
                return result;
            }
        }
    }
}