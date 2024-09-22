using Microsoft.Ajax.Utilities;
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

            var aSvc = new AccountingService();

            vm.Organisation = GetOrganisationDetails(organisationId);

            vm.Customers = db.Customers.Where(x => x.OrganisationId == organisationId).ToList();

            vm.VatOptions = db.VatOptions.Where(x => x.OrganisationId == organisationId).ToList();

            vm.TrialBalanceAccounts = aSvc.GetTrialBalanceAccounts(organisationId);

            vm.Products = db.Products.Where(x => x.OrganisationId == organisationId).ToList();

            return vm;
        }

        public InvoicingViewModel GetFullInvoiceDetails(int invoiceId, int organisationId)
        {
            var vm = new InvoicingViewModel();

            vm.InvoiceDetails = db.Invoices.Where(x => x.Id == invoiceId).FirstOrDefault();

            vm.InvoiceItems = db.InvoiceItems.Where(x => x.InvoiceId == vm.InvoiceDetails.Id).ToList();

            vm.Organisation = GetOrganisationDetails(organisationId);

            vm.Customers = db.Customers.Where(x => x.OrganisationId == organisationId && x.Id == vm.InvoiceDetails.CustomerId).ToList();

            vm.VatOptions = db.VatOptions.Where(x => x.OrganisationId == organisationId).ToList();

            return vm;
        }

        public List<InvoiceDetalsVm> GetInvoices(int organisationId)
        {
            var vm = new List<InvoiceDetalsVm>();

            var invoices = db.Invoices.Where(x => x.OrganisationId == organisationId).ToList();

            foreach (var invoice in invoices)
            {
                var transactions = db.InvoiceTransactions.Where(x => x.InvoiceId == invoice.Id).ToList();

                var overDueInvoice = false;

                var paidInvoice = false;

                var invoicePaidAmounts = transactions.Sum(x => x.Amount);

                if (invoice.InvoiceDueDate < DateTime.Now && (invoicePaidAmounts * -1) < invoice.Amount)
                {
                    overDueInvoice = true;
                }

                if ((invoicePaidAmounts * -1) >= invoice.Amount)
                {
                    paidInvoice = true;
                }

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
                    Paid = paidInvoice,
                    OverDueInvoice = overDueInvoice,
                    AmountPaid = invoicePaidAmounts,
                    Pop = invoice.ProofOfPoayment
                });
            }

            return vm;
        }

        public Result SaveInvoice(InvoicingViewModel vm, int organisationId, string userId = "")
        {
            var result = new Result();

            var aSvc = new AccountingService();

            vm.InvoiceDetails.InvoiceNumber = $"INV-{GetNextInvoiceNumber(organisationId)}";
            vm.InvoiceDetails.OrganisationId = organisationId;
            vm.InvoiceDetails.Inserted = DateTime.Now;
            vm.InvoiceDetails.Updated = DateTime.Now;

            db.Invoices.Add(vm.InvoiceDetails);

            var res = db.SaveChanges();

            var invoiceAmountExclVat = vm.InvoiceDetails.Amount / ((100 + vm.InvoiceDetails.VatPercentage) / 100);

            if(res > 0)
            {
                if(vm.InvoiceDetails.InvoiceAccountId != -100)
                {
                    // save invoice gl
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == vm.InvoiceDetails.InvoiceAccountId).FirstOrDefault();

                    if(account != null)
                    {
                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - INV",
                            AccountName = $"{account.AccountName}",
                            AccountNumber = account.AccountNumber,
                            Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                            Debit = invoiceAmountExclVat > 0 ? invoiceAmountExclVat : 0,
                            Credit = invoiceAmountExclVat < 0 ? invoiceAmountExclVat : 0,
                            Date = vm.InvoiceDetails.InvoiceDate,
                            Month = vm.InvoiceDetails.InvoiceDate.Month,
                            Year = vm.InvoiceDetails.InvoiceDate.Year,
                        };

                        aSvc.CreateUpdateGlAccount(gl, organisationId);
                    }
                    
                }

                if(vm.InvoiceDetails.VatOptionId != -100)
                {
                    // save invoice vat gl
                    var vat = db.VatOptions.Where(x => x.Id == vm.InvoiceDetails.VatOptionId).FirstOrDefault();

                    var account = db.TrialBalanceAccounts.Where(x => x.Id == vat.TaxAccountId).FirstOrDefault();

                    if (account != null)
                    {
                        var vatAmount = invoiceAmountExclVat * (vm.InvoiceDetails.VatPercentage / 100);

                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - INV",
                            AccountName = $"{account.AccountName}",
                            AccountNumber = account.AccountNumber,
                            Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                            Debit = vatAmount > 0 ? vatAmount : 0,
                            Credit = vatAmount < 0 ? vatAmount : 0,
                            Date = vm.InvoiceDetails.InvoiceDate,
                            Month = vm.InvoiceDetails.InvoiceDate.Month,
                            Year = vm.InvoiceDetails.InvoiceDate.Year,
                        };

                        var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                        if(glRes.Success == true)
                        {
                            var vt = new VatTransaction()
                            {
                                Date = vm.InvoiceDetails.InvoiceDate,
                                TypeId = vm.InvoiceDetails.VatOptionId,
                                Account = account.AccountNumber,
                                Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                                Exclusive = invoiceAmountExclVat,
                                Inclusive = invoiceAmountExclVat + vatAmount,
                                TaxAmount = vatAmount,
                                Inserted = DateTime.Now,
                                Updated = DateTime.Now,
                                OrganisationId = organisationId,
                            };

                            var vtRes = aSvc.SaveVatTransaction(vt, organisationId);
                        }
                        
                    }
                }

                var ctRecord = new CustomerTransaction
                {
                    Description = $"DR - {vm.InvoiceDetails.InvoiceNumber}",
                    Amount = vm.InvoiceDetails.Amount,
                    InvoiceId = vm.InvoiceDetails.Id,
                    OrganisationId = organisationId,
                    CustomerId = vm.InvoiceDetails.CustomerId,
                    Inserted = DateTime.Now,
                    Updated = DateTime.Now,
                };

                aSvc.SaveCustomerTransaction(ctRecord);

                foreach (var item in vm.InvoiceItems)
                {
                    item.InvoiceId = vm.InvoiceDetails.Id;
                    item.Inserted = DateTime.Now;
                    item.Updated = DateTime.Now;

                    var itemRes = db.InvoiceItems.Add(item);

                    if (item.ItemAccountId > 0)
                    {
                        // save item gl account
                        var account = db.TrialBalanceAccounts.Where(x => x.Id == item.ItemAccountId).FirstOrDefault();

                        if (account != null)
                        {
                            var gl = new GlAccountViewModel
                            {
                                AccountReference = "REF(ID) - INV",
                                AccountName = $"{account.AccountName}",
                                AccountNumber = account.AccountNumber,
                                Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                                Debit = item.Amount > 0 ? item.Amount : 0,
                                Credit = item.Amount < 0 ? item.Amount : 0,
                                Date = vm.InvoiceDetails.InvoiceDate,
                                Month = vm.InvoiceDetails.InvoiceDate.Month,
                                Year = vm.InvoiceDetails.InvoiceDate.Year,
                            };

                            var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                        }
                    }

                    if (item.VatOptionId > 0)
                    {
                        // save vat gl account
                        var vat = db.VatOptions.Where(x => x.Id == item.VatOptionId).FirstOrDefault();

                        var account = db.TrialBalanceAccounts.Where(x => x.Id == vat.TaxAccountId).FirstOrDefault();

                        if (account != null)
                        {
                            var vatAmount = item.Amount * (vat.Percentage / 100);

                            var gl = new GlAccountViewModel
                            {
                                AccountReference = "REF(ID) - INV",
                                AccountName = $"{account.AccountName}",
                                AccountNumber = account.AccountNumber,
                                Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                                Debit = vatAmount > 0 ? vatAmount : 0,
                                Credit = vatAmount < 0 ? vatAmount : 0,
                                Date = vm.InvoiceDetails.InvoiceDate,
                                Month = vm.InvoiceDetails.InvoiceDate.Month,
                                Year = vm.InvoiceDetails.InvoiceDate.Year,
                            };

                            var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                            if(glRes.Success == true)
                            {
                                var vt = new VatTransaction()
                                {
                                    Date = vm.InvoiceDetails.InvoiceDate,
                                    TypeId = item.VatOptionId,
                                    Account = account.AccountNumber,
                                    Description = $"{vm.InvoiceDetails.InvoiceNumber} - {vm.InvoiceDetails.InvoiceDescription}",
                                    Exclusive = item.Amount,
                                    Inclusive = item.Amount + vatAmount,
                                    TaxAmount = vatAmount,
                                    Inserted = DateTime.Now,
                                    Updated = DateTime.Now,
                                    OrganisationId = organisationId,
                                };

                                var vtRes = aSvc.SaveVatTransaction(vt, organisationId);
                            }
                                                        
                        }
                    }

                    if(item.ProductId > 0)
                    {
                        var product = db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault();

                        if (product != null)
                        {
                            product.ItemsAvailable -= item.Qty;
                            product.ItemsSold += item.Qty;

                            var inventoryAccount = db.TrialBalanceAccounts.Where(x => x.Id == product.InventoryAccountId).FirstOrDefault();

                            var gl = new GlAccountViewModel
                            {
                                AccountReference = $"REF(ID) - INVENTORY",
                                AccountName = $"{inventoryAccount.AccountName}",
                                AccountNumber = inventoryAccount.AccountNumber,
                                Description = $"{item.Description} (QTY - {item.Qty}) - (COST - {product.Cost}) ",
                                Debit = product.Cost * item.Qty,
                                Credit = 0,
                                Date = vm.InvoiceDetails.InvoiceDate,
                                Month = vm.InvoiceDetails.InvoiceDate.Month,
                                Year = vm.InvoiceDetails.InvoiceDate.Year,
                            };

                            var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                            db.SaveChanges();

                            var invSvc = new InventoryService();

                            var auditResult = invSvc.SaveProductAudit(product.Id, $"Qty sold {item.Qty} Invoice: {vm.InvoiceDetails.InvoiceNumber}", organisationId, userId);
                        }
                    }
                }

                if (vm.CreateOrder == true)
                {
                    var qty = 0;
                    var productName = string.Empty;
                    string generateOrderId = $"#{vm.InvoiceDetails.InvoiceNumber}";

                    foreach (var item in vm.InvoiceItems)
                    {
                        qty += item.Qty;
                        productName += $" - {item.Description}";
                    }

                    var customer = db.Customers.Where(x => x.Id == vm.InvoiceDetails.CustomerId).FirstOrDefault();

                    var order = new Orders
                    {
                        OrderId = generateOrderId,
                        CustomerId = vm.InvoiceDetails.CustomerId,
                        CustomerName = customer != null ? customer.CustomerName : string.Empty,
                        DateOrderCreated = DateTime.Now.ToString("dd/MM/yyyy"),
                        ExpectedDeliveryDate = vm.InvoiceDetails.InvoiceDueDate.ToString("dd/MM/yyyy"),
                        NumberOfItems = qty,
                        ProductName = productName.Trim(' ').Trim('-'),
                        OrderStatus = "Processed",
                        ShippingCost = Convert.ToDecimal(vm.InvoiceDetails.Shipping),
                        OrderWorth = Convert.ToDecimal(vm.InvoiceDetails.Amount),
                        OrderProgress = 10,
                        OrganisationId = organisationId,
                        Status = "Active",
                        UserId = userId,
                    };

                    var orderDetails = db.Orders.Add(order);

                    var orderRes = db.SaveChanges();

                    if(orderDetails.Id > 0)
                    {
                        var description = "Inception - This order was created!";
                        var changed = "Inception";

                        var orderAudit = new OrderAudit
                        {
                            Changed = changed,
                            Description = description,
                            CreatedDate = DateTime.Now,
                            OrderId = generateOrderId,
                            UserId = userId,
                        };

                        var orderAuditDetails = db.OrderAudit.Add(orderAudit);

                        var auditRes = db.SaveChanges();
                    }

                }

            }            

            res = db.SaveChanges();

            if (res > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved invoice";
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

        public Result PayInvoiceDetails(PayInvoiceDetailsVm vm, int organisationId)
        {
            var result = new Result();

            var invoiceTransaction = db.InvoiceTransactions.Where(x => x.InvoiceId == vm.InvoiceId && x.OrganisationId == organisationId).FirstOrDefault();

            vm.Amount *= -1;

            var transaction = new InvoiceTransaction
            {
                Date = vm.Date,
                Amount = vm.Amount,
                AccountId = vm.AccountId,
                InvoiceId = vm.InvoiceId,
                OrganisationId = organisationId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
            };

            db.InvoiceTransactions.Add(transaction);

            var res = db.SaveChanges();

            if (res > 0)
            {
                //save account transaction
                if (vm.AccountId > 0)
                {
                    // save payment gl
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == vm.AccountId).FirstOrDefault();

                    if (account != null)
                    {
                        var invoiceDetails = db.Invoices.Where(x => x.Id == vm.InvoiceId).FirstOrDefault();
                        if (invoiceDetails != null)
                        {
                            var gl = new GlAccountViewModel
                            {
                                AccountReference = "REF(ID) - PR",
                                AccountName = $"{account.AccountName}",
                                AccountNumber = account.AccountNumber,
                                Description = $"PR - {invoiceDetails.InvoiceNumber} - {invoiceDetails.InvoiceDescription}",
                                Debit = vm.Amount > 0 ? vm.Amount * -1 : 0,
                                Credit = vm.Amount < 0 ? vm.Amount * -1 : 0,
                                Date = vm.Date,
                                Month = vm.Date.Month,
                                Year = vm.Date.Year,
                            };

                            var aSvc = new AccountingService();
                            var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                            if(glRes != null)
                            {
                                var ct = new CustomerTransaction
                                {
                                    Description = $"PR - {invoiceDetails.InvoiceNumber} - {account.AccountName}",
                                    Amount = vm.Amount,
                                    InvoiceId = vm.InvoiceId,
                                    OrganisationId = organisationId,
                                    CustomerId = invoiceDetails.CustomerId,
                                    Inserted = DateTime.Now,
                                    Updated = DateTime.Now,
                                };
                                
                                aSvc.SaveCustomerTransaction(ct);
                            }
                        }
                        
                    }

                }

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

        public string GetProofOfPayment(int invoiceId)
        {
            var result = new Result();

            var invoice = db.Invoices.Where(x => x.Id == invoiceId).FirstOrDefault();

            return invoice.ProofOfPoayment;
        }
    }
}