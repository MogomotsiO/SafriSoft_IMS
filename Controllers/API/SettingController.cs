using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.ViewModels;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Setting")]
    public class SettingController : BaseApiController
    {
        [HttpGet, Route("GetVatOptions")]
        public async Task<IHttpActionResult> GetVatOptions()
        {
            var result = new Result();            

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.GetVatOptions(organisationId);

                result.Success = true;
                result.Message = "Options";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveVatOption")]
        public async Task<IHttpActionResult> SaveVatOption(VatOptionVm vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.SaveVatOption(vm, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("RemoveVatOption/{Id}")]
        public async Task<IHttpActionResult> RemoveVatOption(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.RemoveVatOption(id, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetExpenseCategories")]
        public async Task<IHttpActionResult> GetExpenseCategories()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.GetExpenseCategories(organisationId);

                result.Success = true;
                result.Message = "Categories";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveExpenseCategory")]
        public async Task<IHttpActionResult> SaveExpenseCategory(SettingVm vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.SaveExpenseCategory(vm, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("RemoveExpenseCategory/{Id}")]
        public async Task<IHttpActionResult> RemoveExpenseCategory(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var sSvc = new SettingService();

                result = sSvc.RemoveExpenseCategory(id, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }
    }
}