using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Services
{
    public class InventoryService: BaseService
    {
        public List<SupplierDetailsViewModel> GetSuppliers(int organisationId)
        {
            var result = new Result();

            var vm = new List<SupplierDetailsViewModel>();

            var suppliers = db.Suppliers.Where(x => x.OrganisationId == organisationId).ToList();

            var products = db.Products.Where(x => x.OrganisationId == organisationId).ToList();

            foreach(var supplier in suppliers)
            {
                var product = products.Where(x => x.Id == supplier.ProductId).FirstOrDefault();

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

            var productLinked = db.Suppliers.Where(x => x.ProductId == vm.Supplier.ProductId && x.OrganisationId == organisationId).FirstOrDefault();

            if(productLinked != null)
            {
                result.Success = false;
                result.Message = $"Product already assigned to supplier {productLinked.CompanyName}";
                return result;
            }

            var supplierExist = db.Suppliers.Where(x => x.CompanyName == vm.Supplier.CompanyName && x.OrganisationId == organisationId).FirstOrDefault();

            if(supplierExist != null)
            {
                result.Success = false;
                result.Message = $"Supplier already exists {productLinked.CompanyName}";
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

        public Result SaveInvoiceFileDetails(string fileName, string fileContentType, int qty, double vatAmount, double amount, int organisationId, int id)
        {
            var result = new Result();

            var invoice = db.SupplierInvoices.Where(x => x.FileName == fileName && x.FileContentType == fileContentType && x.OrganisationId == organisationId).FirstOrDefault();

            if(invoice != null)
            {
                result.Success = false;
                result.Message = $"File with name {fileName} already exists";
                return result;
            }

            var invoiceFile = new SupplierInvoice()
            {
                FileName = fileName,
                FileContentType = fileContentType,
                Qty = qty,
                VatAmount = vatAmount,
                Amount = amount,
                OrganisationId = organisationId,
                SupplierId = id,
                Inserted = DateTime.Now,
                Updated = DateTime.Now
            };

            db.SupplierInvoices.Add(invoiceFile);

            var res = db.SaveChanges();

            if(res > 0)
            {
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
                vm.Add(new SupplierInvoiceDetailViewModel()
                {
                    Id = inv.Id,
                    Date = inv.Inserted.ToString("dd/MM/yyyy"),
                    FileName = inv.FileName,
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
                vm.Qty = supplierInvoice.Qty;
                vm.VatAmount = supplierInvoice.VatAmount;
                vm.Amount = supplierInvoice.Amount;
            }

            return vm;
        }
               
    }
}