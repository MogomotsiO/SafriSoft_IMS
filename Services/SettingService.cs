using Microsoft.Owin.Security.Twitter;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Razor.Text;

namespace SafriSoftv1._3.Services
{
    public class SettingService : BaseService
    {
        public Result GetVatOptions(int orgId)
        {
            var result = new Result();

            var vatOptions = new List<VatOptionVm>();

            var items = db.VatOptions.Where(x => x.OrganisationId == orgId).ToList();

            foreach(var item in items)
            {
                var account = db.TrialBalanceAccounts.Where(x => x.Id == item.TaxAccountId).FirstOrDefault();

                vatOptions.Add(new VatOptionVm
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Percentage = item.Percentage,
                    TaxAccount = account.AccountNumber + "-" + account.AccountName
                });
            }

            result.obj = vatOptions;

            return result;
        }

        public Result SaveVatOption(VatOptionVm vm, int orgId)
        {
            var result = new Result();

            var item = new VatOption
            {
                Name = vm.Name,
                Description = vm.Description,
                Percentage = vm.Percentage,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                TaxAccountId = vm.TaxAccountId,
                OrganisationId = orgId
            };

            var option = db.VatOptions.Add(item);
            var res = db.SaveChanges();

            if(res > 0)
            {
                result.Success = true;
                result.Message = "Saved";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save option";
            }

            return result;
        }

        public Result RemoveVatOption(int id, int orgId)
        {
            var result = new Result();

            var item = db.VatOptions.FirstOrDefault(x => x.Id == id && x.OrganisationId == orgId);

            db.VatOptions.Remove(item);

            var res = db.SaveChanges();

            if(res > 0)
            {
                result.Success = true;
                result.Message = "Option removed";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not remove option";
            }

            return result;
        }

        public Result GetExpenseCategories(int orgId)
        {
            var result = new Result();

            var items = db.Settings.Where(x => x.SettingType == Models.SystemModels.SettingType.ExpenseCategories && x.OrganisationId == orgId).ToList();

            result.obj = items;

            return result;
        }

        public Result SaveExpenseCategory(SettingVm vm, int orgId)
        {
            var result = new Result();

            var setting = new Setting
            {
                Name = vm.Name,
                Description = vm.Description,
                SettingType = Models.SystemModels.SettingType.ExpenseCategories,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                OrganisationId = orgId
            };

            db.Settings.Add(setting);

            var res = db.SaveChanges();

            if(res > 0 )
            {
                result.Success = true;
                result.Message = "Successfully save setting";
            }
            else
            {
                result .Success = false;
                result.Message = "Could not save setting";
            }

            return result;
        }

        public Result RemoveExpenseCategory(int id, int orgId)
        {
            var result = new Result();

            var item = db.Settings.FirstOrDefault(x => x.Id == id && x.OrganisationId == orgId);

            db.Settings.Remove(item);

            var res = db.SaveChanges();

            if (res > 0)
            {
                result.Success = true;
                result.Message = "Category removed";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not remove option";
            }

            return result;
        }
    }
}