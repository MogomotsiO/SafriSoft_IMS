using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class BaseController : Controller
    {
        public string GetOrganisationName()
        {

            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            return getOrgClaim;
        }

        public string GetUserId()
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            return userId;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if(filterContext.Exception != null)
            {
                using(var writer = new StreamWriter("Error.txt"))
                {
                    writer.WriteLine(filterContext.Exception.Message);
                }
            }
        }
    }
}