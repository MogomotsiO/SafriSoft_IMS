using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace SafriSoftv1._3.Services
{
    public class InventoryService: BaseService
    {
        public SupplierViewModel GetSupplierVm(int organisationId)
        {
            var vm = new SupplierViewModel();

            var aSvc = new AccountingService();

            vm.TrialBalanceAccounts = aSvc.GetTrialBalanceAccounts(organisationId);

            vm.SuppliersInvoices = db.SupplierInvoices.Where(x => x.OrganisationId == organisationId).ToList();

            vm.Products = db.Products.Where(x => x.OrganisationId == organisationId).ToList();

            return vm;
        }

        public List<SupplierDetailsViewModel> GetSuppliers(int organisationId)
        {
            var result = new Result();

            var vm = new List<SupplierDetailsViewModel>();

            var aSvc = new AccountingService();

            var suppliers = db.Suppliers.Where(x => x.OrganisationId == organisationId).ToList();

            var products = db.Products.Where(x => x.OrganisationId == organisationId).ToList();

            var trialBalanceAccounts = aSvc.GetTrialBalanceAccounts(organisationId);
            
            foreach(var supplier in suppliers)
            {
                var product = products.Where(x => x.Id == supplier.ProductId).FirstOrDefault();

                //fix later
                var supplierTransactions = db.SupplierTransactions.Where(x => x.SupplierId == supplier.Id).ToList();

                var balance = supplierTransactions.Sum(x => x.Amount);

                vm.Add(new SupplierDetailsViewModel()
                {
                    Id = supplier.Id,
                    CompanyName = supplier.CompanyName,
                    TradingAs = supplier.TradingAs,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    City = supplier.City,
                    Country = supplier.Country,
                    ContactPersonName = supplier.ContactPersonName,
                    ContactPersonEmail = supplier.ContactPersonEmail,
                    ContactPersonPhone = supplier.ContactPersonPhone,
                    ContactPersonPosition = supplier.ContactPersonPosition,
                    ProductName = product != null ? product.ProductName : "",
                    Products = products,
                    TrialBalanceAccounts = trialBalanceAccounts,
                    Balance = balance
                });
            }

            if(vm.Count == 0) {
                
            }

            return vm;
        }

        public Supplier GetSupplier(int id, int organisationId)
        {
            var result = new Result();

            var supplier = db.Suppliers.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();
            
            return supplier;
        }

        public Result SaveSupplierDetails(SupplierViewModel vm, int organisationId)
        {
            var result = new Result();

            //var productLinked = db.Suppliers.Where(x => x.ProductId == vm.Supplier.ProductId && x.OrganisationId == organisationId).FirstOrDefault();

            //if(productLinked != null)
            //{
            //    result.Success = false;
            //    result.Message = $"Product already assigned to supplier {productLinked.CompanyName}";
            //    return result;
            //}

            var supplierExist = db.Suppliers.Where(x => x.CompanyName == vm.Supplier.CompanyName && x.OrganisationId == organisationId).FirstOrDefault();

            if(supplierExist != null)
            {
                result.Success = false;
                result.Message = $"Supplier already exists {supplierExist.CompanyName}";
                return result;
            }

            vm.Supplier.OrganisationId = organisationId;
            vm.Supplier.Inserted = DateTime.Now;
            vm.Supplier.Updated = DateTime.Now;

            db.Suppliers.Add(vm.Supplier);

            var res = db.SaveChanges();

            if(res > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved supplier details";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save supplier";
            }

            return result;
        }

        public Result UpdateSupplierDetails(SupplierViewModel vm, int organisationId)
        {
            var result = new Result();

            var supplier = db.Suppliers.Where(x => x.Id == vm.Supplier.Id).FirstOrDefault();

            vm.Supplier.OrganisationId = supplier.OrganisationId;
            vm.Supplier.Inserted = supplier.Inserted;
            vm.Supplier.Updated = DateTime.Now;

            db.Entry(supplier).State = EntityState.Detached;

            db.Entry(vm.Supplier).State = EntityState.Modified;

            var res = db.SaveChanges();

            if (res > 0)
            {
                result.Success = true;
                result.Message = "Successfully updated supplier details";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not update supplier";
            }

            return result;
        }

        public Result SaveInvoiceFileDetails(string fileName, string fileContentType, DateTime date, string description, int qty, double vatAmount, double amount, int vatAccountId, int invoiceAccountId, int organisationId, int id, int productId)
        {
            var result = new Result();

            var aSvc = new AccountingService();

            var invoice = db.SupplierInvoices.Where(x => x.FileName == fileName && x.FileContentType == fileContentType && x.OrganisationId == organisationId).FirstOrDefault();

            if(invoice != null)
            {
                result.Success = false;
                result.Message = $"File with name {fileName} already exists";
                return result;
            }

            amount *= -1;
            vatAmount *= -1;

            var invoiceFile = new SupplierInvoice()
            {
                InvoiceDate = date,
                Description = description,
                FileName = fileName,
                FileContentType = fileContentType,
                Qty = qty,
                VatAmount = vatAmount,
                Amount = amount,
                OrganisationId = organisationId,
                SupplierId = id,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                VatAccountId = vatAccountId,
                InvoiceAccountId = invoiceAccountId,
                ProductId = productId,
            };

            db.SupplierInvoices.Add(invoiceFile);

            var res = db.SaveChanges();

            if (res > 0)
            {
                if(productId > 0)
                {
                    var product = db.Products.Where(x => x.Id == productId).FirstOrDefault();

                    if(product != null)
                    {
                        product.ItemsAvailable += qty;
                    }

                    db.SaveChanges();
                }

                // save gls
                var vatAccount = db.TrialBalanceAccounts.Where(x => x.Id == vatAccountId).FirstOrDefault();

                if (vatAccount != null)
                {
                    var gl = new GlAccountViewModel
                    {
                        AccountReference = $"REF(ID) - SVA",
                        AccountName = $"{vatAccount.AccountName}",
                        AccountNumber = vatAccount.AccountNumber,
                        Description = $"{description} - {vatAccount.AccountName}",
                        Debit = vatAmount > 0 ? vatAmount : 0,
                        Credit = vatAmount < 0 ? vatAmount * -1 : 0,
                        Date = date,
                        Month = date.Month,
                        Year = date.Year,
                    };

                    var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                    var stVat = new SupplierTransaction
                    {
                        Description = $"{description} - {vatAccount.AccountName}",
                        Amount = vatAmount,
                        SupplierInvoiceId = res,
                        OrganisationId = organisationId,
                        SupplierId = id,
                        Inserted = DateTime.Now,
                        Updated = DateTime.Now,
                    };

                    SaveSupplierTransaction(stVat);

                    var vt = new VatTransaction()
                    {
                        Date = date,
                        TypeId = 0,
                        Account = vatAccount.AccountNumber,
                        Description = $"{description} - {vatAccount.AccountName}",
                        Exclusive = amount,
                        Inclusive = amount + vatAmount,
                        TaxAmount = vatAmount,
                        Inserted = DateTime.Now,
                        Updated = DateTime.Now,
                        OrganisationId = organisationId,
                    };

                    var vtRes = aSvc.SaveVatTransaction(vt, organisationId);
                }

                var invoiceAccount = db.TrialBalanceAccounts.Where(x => x.Id == invoiceAccountId).FirstOrDefault();

                if (invoiceAccount != null)
                {
                    var gl = new GlAccountViewModel
                    {
                        AccountReference = $"REF(ID) - SIA",
                        AccountName = $"{invoiceAccount.AccountName}",
                        AccountNumber = invoiceAccount.AccountNumber,
                        Description = $"{description} - {invoiceAccount.AccountName}",
                        Debit = amount > 0 ? amount : 0,
                        Credit = amount < 0 ? amount * - 1 : 0,
                        Date = date,
                        Month = date.Month,
                        Year = date.Year,
                    };

                    var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                    if (glRes.Success == true)
                    {
                        var st = new SupplierTransaction
                        {
                            Description = $"{description} - {invoiceAccount.AccountName}",
                            Amount = amount,
                            SupplierInvoiceId = res,
                            OrganisationId = organisationId,
                            SupplierId = id,
                            Inserted = DateTime.Now,
                            Updated = DateTime.Now,
                        };

                        SaveSupplierTransaction(st);
                    }
                }


                result.Success = true;
                result.Message = "File processed";
            }
            else
            {
                result.Success = true;
                result.Message = "Could not save file details";
            }

            return result;
        }

        public List<SupplierInvoiceDetailViewModel> GetSupplierInvoices(int id, int organisationId)
        {
            var vm = new List<SupplierInvoiceDetailViewModel>();

            var supplierInvoices = db.SupplierInvoices.Where(x => x.SupplierId == id && x.OrganisationId == organisationId).ToList();

            foreach (var inv in supplierInvoices)
            {
                var product = db.Products.Where(x => x.Id == inv.ProductId && x.OrganisationId == organisationId).FirstOrDefault();

                vm.Add(new SupplierInvoiceDetailViewModel()
                {
                    Id = inv.Id,
                    Date = inv.InvoiceDate.GetValueOrDefault().ToString("dd/MM/yyyy"),
                    FileName = inv.FileName,
                    Product = product != null ? product.ProductName : string.Empty,
                    Qty = inv.Qty,
                    VatAmount = inv.VatAmount,
                    Amount = inv.Amount
                });
            }

            return vm;
        }

        public SupplierInvoiceDetailViewModel GetSupplierInvoice(int id)
        {
            var vm = new SupplierInvoiceDetailViewModel();

            var supplierInvoice = db.SupplierInvoices.Where(x => x.Id == id).FirstOrDefault();

            if(supplierInvoice != null)
            {
                vm.Id = supplierInvoice.Id;
                vm.Date = supplierInvoice.Inserted.ToString("dd/MM/yyyy");
                vm.FileName = supplierInvoice.FileName;
                vm.FileContentType = supplierInvoice.FileContentType;
                vm.Qty = supplierInvoice.Qty;
                vm.VatAmount = supplierInvoice.VatAmount;
                vm.Amount = supplierInvoice.Amount;
            }

            return vm;
        }

        public bool SaveSupplierTransaction(SupplierTransaction st)
        {
            st.Date = DateTime.Now;

            db.SupplierTransactions.Add(st);
            var res = db.SaveChanges();

            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
               
        public Result PaySupplier(PaySupplierViewModel vm, int organisationId)
        {
            var result = new Result();

            var aSvc = new AccountingService();

            var account = db.TrialBalanceAccounts.Where(x => x.Id == vm.AccountId).FirstOrDefault();

            if (account != null)
            {
                var gl = new GlAccountViewModel
                {
                    AccountReference = $"REF(ID) - SPR",
                    AccountName = $"{account.AccountName}",
                    AccountNumber = account.AccountNumber,
                    Description = $"{vm.Description} - {account.AccountName}",
                    Debit = vm.Amount > 0 ? vm.Amount : 0,
                    Credit = vm.Amount < 0 ? vm.Amount : 0,
                    Date = vm.Date,
                    Month = vm.Date.Month,
                    Year = vm.Date.Year,
                };

                var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                if (glRes.Success == true)
                {
                    var st = new SupplierTransaction
                    {
                        Description = $"{vm.Description} - {account.AccountName}",
                        Amount = vm.Amount,
                        SupplierInvoiceId = vm.SupplierInvoiceId,
                        OrganisationId = organisationId,
                        SupplierId = vm.SupplierId,
                        Inserted = DateTime.Now,
                        Updated = DateTime.Now,
                    };

                    var res = SaveSupplierTransaction(st);

                    if(res == true)
                    {
                        result.Success = true;
                        result.Message = "Successfully saved payment";
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Could not save payment";
                    }
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Could not find account provided";
            }

            return result;

        }
    }
}