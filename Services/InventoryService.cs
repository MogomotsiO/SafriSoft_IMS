using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

        public Result SaveInvoiceFileDetails(string fileName, string fileContentType, DateTime date, string description, int qty, double vatAmount, double amount, int vatAccountId, int invoiceAccountId, int organisationId, int id, int productId, string userId)
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
                UserId = userId
            };

            db.SupplierInvoices.Add(invoiceFile);

            var res = db.SaveChanges();

            if (res > 0)
            {
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

        public Result LoadProductStock(int invoiceId, int organisationId, string userId)
        {
            var result = new Result();

            var invoice = db.SupplierInvoices.Where(x => x.Id == invoiceId && x.OrganisationId == organisationId).FirstOrDefault();

            if(invoice != null)
            {
                if(invoice.ProductId > 0)
                {
                    var product = db.Products.Where(x => x.Id == invoice.ProductId).FirstOrDefault();

                    if (product != null)
                    {
                        var qty = invoice.Qty;

                        var inventoryAmt = product.Cost * qty;
                        var productName = product.ProductName;

                        var inventoryAccount = db.TrialBalanceAccounts.Where(x => x.Id == product.InventoryAccountId).FirstOrDefault();

                        if(inventoryAccount == null)
                        {
                            result.Success = false;
                            result.Message = "Could not find the trial balance account";
                            return result;
                        }

                        var gl = new GlAccountViewModel
                        {
                            AccountReference = $"REF(ID) - INVENTORY",
                            AccountName = $"{inventoryAccount.AccountName}",
                            AccountNumber = inventoryAccount.AccountNumber,
                            Description = $"{productName} (QTY - {qty}) (COST - {product.Cost})",
                            Debit = 0,
                            Credit = inventoryAmt,
                            Date = DateTime.Now,
                            Month = DateTime.Now.Month,
                            Year = DateTime.Now.Year,
                        };

                        var aSvc = new AccountingService();

                        var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                        var supplier = db.Suppliers.Where(x => x.Id == invoice.SupplierId && x.OrganisationId == organisationId).FirstOrDefault();

                        SaveProductAudit(product.Id, $"Stock loaded from supplier: {supplier.CompanyName} invoice: {invoice.Description} QTY: {qty}", organisationId, userId);

                        //db updates
                        product.ItemsAvailable += qty;
                        invoice.StockLoaded = DateTime.Now;
                        invoice.StockLoadedUser = userId;

                        db.SaveChanges();

                        result.Success = true;
                        result.Message = "Stock Loaded";
                        return result;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Product not found in the database";
                    }                    
                }
                else
                {
                    result.Success = false;
                    result.Message = "Invoice does not contain a product";
                }                
            }
            else
            {
                result.Success = false;
                result.Message = "Could not find the invoice";
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

                var user = dbSafriSoftApp.Users.Where(x => x.Id == inv.StockLoadedUser).FirstOrDefault();

                vm.Add(new SupplierInvoiceDetailViewModel()
                {
                    Id = inv.Id,
                    Date = inv.InvoiceDate.GetValueOrDefault().ToString("dd/MM/yyyy"),
                    FileName = inv.FileName,
                    Product = product != null ? product.ProductName : string.Empty,
                    Qty = inv.Qty,
                    VatAmount = inv.VatAmount,
                    Amount = inv.Amount,
                    DateStockLoadedStr = inv.StockLoaded.HasValue ? inv.StockLoaded.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty,
                    StockLoadedUser = user != null ? $"{user.UserName}" : string.Empty,
                    StockLoaded = inv.StockLoaded.HasValue
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
                    Debit = 0,
                    Credit = vm.Amount,
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

        public Result SaveSupplierInvoice(SaveSupplierInvoiceDetails vm, int organisationId, string userId)
        {
            var result = new Result();

            var date = vm.Date;
            var description = vm.Description;
            var qty = vm.Qty;
            var amount = vm.Amount;
            var vatAmount = vm.VatAmount;
            var id = vm.SupplierId;
            var vatAccountId = vm.VatAccountId;
            var invoiceAccountId = vm.InvoiceAccountId;
            var productId = vm.ProductId;
            var fileName = vm.Filename;
            var file = vm.File.Split(',').Last();

            var aSvc = new AccountingService();

            var invoice = db.SupplierInvoices.Where(x => x.FileName == fileName && x.OrganisationId == organisationId).FirstOrDefault();

            if (invoice != null)
            {
                result.Success = false;
                result.Message = $"File with name {fileName} already exists";
                return result;
            }

            var saveFileDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Documents/SupplierInvoices");

            var fullFileName = $"{saveFileDir}/{fileName}";

            File.WriteAllBytes(fullFileName, Convert.FromBase64String(file));

            //amount *= -1;
            //vatAmount *= -1;

            var invoiceFile = new SupplierInvoice()
            {
                InvoiceDate = date,
                Description = description,
                FileName = fileName,
                FileContentType = "application/pdf",
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
                UserId = userId
            };

            db.SupplierInvoices.Add(invoiceFile);

            var res = db.SaveChanges();

            if (res > 0)
            {
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
                        Debit = vatAmount,
                        Credit = 0,
                        Date = date,
                        Month = date.Month,
                        Year = date.Year,
                    };

                    var glRes = aSvc.CreateUpdateGlAccount(gl, organisationId);

                    //var stVat = new SupplierTransaction
                    //{
                    //    Description = $"{description} - {vatAccount.AccountName}",
                    //    Amount = vatAmount,
                    //    SupplierInvoiceId = res,
                    //    OrganisationId = organisationId,
                    //    SupplierId = id,
                    //    Inserted = DateTime.Now,
                    //    Updated = DateTime.Now,
                    //};

                    //SaveSupplierTransaction(stVat);

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
                        Debit = amount,
                        Credit = 0,
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
                            Amount = (amount + vatAmount) * -1,
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
                result.Success = false;
                result.Message = "Could not save file details";
            }

            return result;
        }

        public Result SaveProductRequirements(int id, List<RequirementViewModel> requirements, int organisationId)
        {
            var result = new Result();

            var items = db.ProductRequirements.Where(x => x.ProductId == id && x.OrganisationId == organisationId).ToList();

            var removeReq = db.ProductRequirements.RemoveRange(items);

            foreach(var requirement in requirements)
            {
                var req = new ProductRequirement()
                {
                    ProductId = id,
                    ProductName = requirement.Name,
                    RequiredProductId = requirement.Id,
                    QtyRequired = requirement.Qty,
                    Inserted = DateTime.Now,
                    Updated = DateTime.Now,
                    OrganisationId = organisationId
                };

                var item = db.ProductRequirements.Add(req);

                var resSave = db.SaveChanges();
            }            

            return result;
        }

        public Result SaveProductAudit(int id, string description, int organisationId, string userId)
        {
            var result = new Result();

            var audit = new ProductAudit()
            {
                ProductId = id,
                Description = description,
                UserId = userId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                OrganisationId = organisationId
            };

            var item = db.ProductAudits.Add(audit);

            var resSave = db.SaveChanges();

            return result;
        }

        public Result AddQuantity(AddQuantityViewModel vm, int organisationId, string userId)
        {
            var result = new Result();

            // check quantity rules
            foreach(var req in vm.Requirements)
            {
                if(req.Qty > req.QtyAvailable)
                {
                    result.Success = false;
                    result.Message = $"Quantity required is more than quantity available Product: {req.Name}";
                    return result;
                }

                var totalLine = req.Qty * vm.Qty;

                if (totalLine > req.QtyAvailable)
                {
                    result.Success = false;
                    result.Message = $"Quantity required to produce is more than quantity available Product: {req.Name}";
                    return result;
                }
            }

            var item = db.Products.Where(x => x.Id == vm.ProductId).FirstOrDefault();

            foreach (var req in vm.Requirements)
            {
                var totalLine = req.Qty * vm.Qty;

                var product = db.Products.Where(x => x.Id == req.Id).FirstOrDefault();

                product.ItemsAvailable -= totalLine;
                product.ItemsSold += totalLine;

                db.SaveChanges();

                var auditRes = SaveProductAudit(product.Id, $"Qty used {vm.Qty} for product {item.ProductName}", organisationId, userId);
            }

            item.ItemsAvailable += vm.Qty;

            db.SaveChanges();

            var auditResult = SaveProductAudit(item.Id, $"Qty added {vm.Qty}", organisationId, userId);

            result.Success = true;
            result.Message = $"Success";
            return result;
        }

        public Result GetCurrencies()
        {
            var result = new Result();

            var items = db.Countries.ToList();

            result.obj = items;

            result.Success = true;
            result.Message = "Loaded";

            return result;
        }

        public List<NameValuePair> GetUserRoles()
        {
            var result = new List<NameValuePair>();

            var roles = dbSafriSoftApp.Roles.ToList();

            foreach ( var role in roles)
            {
                result.Add(new NameValuePair()
                {
                    Value = role.Id,
                    Name = role.Name
                });
            }

            return result;
        }

        public Result UpdateUser(RegisterViewModel vm, int organisationId)
        {
            var result = new Result();

            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = dbSafriSoftApp.Users.Where(x => x.Id == vm.UserId).FirstOrDefault();
            
            if(user != null)
            {
                var userRoles = userManager.GetRoles(user.Id).ToList();

                foreach (var userRole in userRoles)
                {
                    var removeRes = userManager.RemoveFromRole(user.Id, userRole);
                }                

                foreach(var role in vm.Roles)
                {
                    var userRoleDetails = dbSafriSoftApp.Roles.Where(x => x.Id == role).FirstOrDefault();

                    var res = userManager.AddToRole(user.Id, userRoleDetails.Name);
                }

                user.FirstName = vm.FirstName;
                user.LastName = vm.LastName;
                user.Read = vm.Read;
                user.Write = vm.Write;

                dbSafriSoftApp.SaveChanges();
            }

            return result;
        }

        public Result UploadCustomerDocuments(string fileName, int customerId, int organisationId)
        {
            var result = new Result();

            var document = new CustomerDocument()
            {
                FileName = fileName,
                CustomerId = customerId,
                OrganisationId = organisationId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now
            };

            var addRes = db.CustomerDocuments.Add(document);

            var saveRes = db.SaveChanges();

            if(saveRes > 0)
            {
                result.Success = true;
                result.Message = "File saved";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save file";
            }

            return result;
        }

        public Result GetDocuments(int id, int organisationId)
        {
            var result = new Result();

            var documentsVm = new List<CustomersDocumentsViewModel>();

            var items = db.CustomerDocuments.Where(x => x.CustomerId == id).ToList();

            foreach(var item in items)
            {
                documentsVm.Add(new CustomersDocumentsViewModel()
                {
                    Id = item.Id,
                    FileName = item.FileName,
                    CustomerId = item.CustomerId,
                    DateFileCreated = item.Inserted.ToString("dd/MM/yyyy")
                });
            }

            if(documentsVm.Count > 0)
            {
                result.Success = true;
                result.Message = "Documents loaded";
                result.obj = documentsVm;
            }
            else
            {
                result.Success = false;
                result.Message = "No documents";
                result.obj = documentsVm;
            }

            return result;
        }

        public string GetCustomerDocument(int id)
        {
            var file = db.CustomerDocuments.Where(x => x.Id == id).FirstOrDefault();

            if(file != null && string.IsNullOrEmpty(file.FileName) == false)
            {
                return file.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        public Result GetProductAudit(int id, int organisationId)
        {
            var result = new Result();

            var vm = new List<AuditViewModel>();

            var product = db.Products.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();

            var audits = db.ProductAudits.Where(x => x.ProductId == id && x.OrganisationId == organisationId).ToList();

            foreach(var audit in audits)
            {
                var user = dbSafriSoftApp.Users.Where(x => x.Id == audit.UserId).FirstOrDefault();

                vm.Add(new AuditViewModel()
                {
                    Id = audit.Id,
                    Description = audit.Description,
                    UserId = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty,
                    CreatedDate = audit.Inserted.ToString("MM/dd/yyyy HH:mm:ss"),
                    Changed = string.Empty
                });
                
            }

            result.Message = product.ProductName;
            result.obj = vm;

            return result;
        }


    }
}