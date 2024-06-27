using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Services
{
    public class SettingService : BaseService
    {
        public Result GetVatOptions(int orgId)
        {
            var result = new Result();

            result.obj = db.VatOptions.Where(x => x.OrganisationId == orgId).ToList();

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
    }
}