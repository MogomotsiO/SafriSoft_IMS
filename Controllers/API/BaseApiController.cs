using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    public class BaseApiController : ApiController
    {
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