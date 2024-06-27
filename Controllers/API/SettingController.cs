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
        public async Task<IHttpActionResult> GetVatOptions(int id)
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
    }
}