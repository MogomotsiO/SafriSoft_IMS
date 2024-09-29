using Newtonsoft.Json;
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
    [RoutePrefix("api/Journal")]
    public class JournalController : BaseApiController
    {
        [HttpPost, Route("GetJournals")]
        public async Task<IHttpActionResult> GetJournals(DateParameters vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var jSvc = new JournalService();

                result = jSvc.GetJournals(organisationId, vm);

                result.Success = true;
                result.Message = "Journals";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetJournalEntries/{id}")]
        public async Task<IHttpActionResult> GetJournalEntries(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var jSvc = new JournalService();

                result = jSvc.GetJournalEntries(id,organisationId);

                result.Success = true;
                result.Message = "Journal entries";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveJournal")]
        public async Task<IHttpActionResult> SaveJournal(BaseViewModel vm)
        {
            var result = new Result();

            try
            {
                var jnl = JsonConvert.DeserializeObject<JournalDetailViewModel>(vm.JsonString);
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var jSvc = new JournalService();

                if(jnl.Id == 0)
                {
                    result = jSvc.SaveJournal(jnl, organisationId, userId);
                }
                else
                {
                    result = jSvc.UpdateJournal(jnl, organisationId, userId);
                }
                

                result.Success = true;
                result.Message = "Saved Journal";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("ActivateJournal/{id}")]
        public async Task<IHttpActionResult> ActivateJournal(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var jSvc = new JournalService();

                result = jSvc.ActivateJournal(id, organisationId, userId);

                result.Success = true;
                result.Message = "Journal activated";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("DeactivateJournal/{id}")]
        public async Task<IHttpActionResult> DeactivateJournal(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var jSvc = new JournalService();

                result = jSvc.DeactivateJournal(id, organisationId, userId);

                result.Success = true;
                result.Message = "Journal deactivated";
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