using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Base")]
    public class BaseApiController : ApiController
    {
        [HttpPost, Route("AddNewCompany")]
        public IHttpActionResult AddNewCompany(AddNewCompanyViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var service = new BaseService();

                result = service.AddNewCompany(vm, organisationId, userId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("SwitchCompany/{companyName}")]
        public IHttpActionResult SwitchCompany(string companyName)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var userId = GetUserId();

                var service = new BaseService();

                result = service.SwitchCompany(companyName, organisationId, userId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        public string GetOrganisationName()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            return getOrgClaim;
        }

        public string GetUserId()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            return userId;
        }

    }
}