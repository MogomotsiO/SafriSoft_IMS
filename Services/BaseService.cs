using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SafriSoftv1._3.Services
{
    public class BaseService
    {
        public SafriSoftDbContext db;
        public ApplicationDbContext dbSafriSoftApp;

        public BaseService()
        {
            db = new SafriSoftDbContext();
            dbSafriSoftApp = new ApplicationDbContext();
        }

        public static int GetOrganisationId(string organisationName)
        {
            int organisationId = 0;

            if (string.IsNullOrEmpty(organisationName))
            {
                return 0;
            }

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [OrganisationId] from [{0}].[dbo].[Organisations] where OrganisationName = '{1}'", conn.Database, organisationName);
                organisationId = (int)cmd.ExecuteScalar();
                conn.Close();
            }

            return organisationId;
        }

        public Organisations GetOrganisationDetails(int organisationId)
        {
            return dbSafriSoftApp.Organisations.Where(x => x.OrganisationId == organisationId).FirstOrDefault();
        }

        public static bool CheckPackageAccess(string feature, int orgnaisationId)
        {
            SafriSoftDbContext SafriSoftImsDb = new SafriSoftDbContext();
            var identityConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString());
            identityConn.Open();
            var identityPackageCmd = identityConn.CreateCommand();
            identityPackageCmd.CommandText = string.Format("SELECT pf.[Limit], os.[PackageId] from [dbo].[Organisations] o JOIN [dbo].[OrganisationSoftwares] os on os.[OrganisationId] = o.[OrganisationId] AND os.[SoftwareId] = 1 JOIN [dbo].[PackageFeatures] pf on pf.PackageId = os.[PackageId] AND pf.FeatureName = '{1}' WHERE o.[OrganisationId] = {2}", identityConn.Database, feature, orgnaisationId);
            var identityPackageReader = identityPackageCmd.ExecuteReader();
            var packageFeatureLimit = 0;
            var limitExceeded = false;

            if (identityPackageReader.Read())
            {
                packageFeatureLimit = identityPackageReader.GetInt32(0);
                var package = identityPackageReader.GetInt32(1);
                if (feature == "Customers")
                {
                    var numberOfCustomers = SafriSoftImsDb.Customers.ToList().Count();
                    if (numberOfCustomers >= packageFeatureLimit && package != 3)
                    {
                        limitExceeded = true;
                    }
                }
                else if (feature == "Orders")
                {
                    var numberOfOrders = SafriSoftImsDb.Orders.ToList().Count();
                    if (numberOfOrders >= packageFeatureLimit && package != 3)
                    {
                        limitExceeded = true;
                    }
                }
            }

            identityConn.Close();

            return limitExceeded;
        }

        public Result AddNewCompany(AddNewCompanyViewModel vm, int organisationId, string userId)
        {
            var result = new Result();
            string safriLogo = "data:image/png;base64,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";

            var item = dbSafriSoftApp.Organisations.Where(x => x.OrganisationName == vm.CompanyName).FirstOrDefault();

            if (item != null)
            {
                result.Success = false;
                result.Message = "Company name already exists";
                return result;
            }

            var organisation = new Organisations()
            {
                OrganisationName = vm.CompanyName,
                OrganisationEmail = vm.CompanyEmail,
                OrganisationCell = vm.CompanyTel,
                OrganisationCode = vm.CopmanyPostalCode,
                OrganisationCity = vm.CompanyCity,
                OrganisationProvince = vm.CompanyProvince,
                OrganisationStreet = vm.CompanyAddress,
                UserId = userId,
                OrganisationLogo = safriLogo,
            };

            var org = dbSafriSoftApp.Organisations.Add(organisation);

            var res = dbSafriSoftApp.SaveChanges();

            if(res > 0)
            {
                var organisationSoftware = new OrganisationSoftware();
                organisationSoftware.OrganisationId = organisation.OrganisationId;
                organisationSoftware.SoftwareId = 1;
                organisationSoftware.Granted = true;
                organisationSoftware.PackageId = 3;

                dbSafriSoftApp.OrganisationSoftwares.Add(organisationSoftware);
                dbSafriSoftApp.SaveChanges();                

                UpdateClaim(org.OrganisationName, userId);

                result.Success = true;
                result.Message = "New copmany added";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not add new company";
            }

            return result;
        }

        public Result SwitchCompany(string companyName, int organisationId, string userId)
        {
            var result = new Result();

            UpdateClaim(companyName, userId);

            result.Success = true;
            result.Message = "Switching Companies";

            return result;
        }

        public void UpdateClaim(string organisationName, string userId)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format($"UPDATE [{{0}}].[dbo].[AspNetUserClaims] SET ClaimValue = '{organisationName}' WHERE UserId = '{userId}' AND ClaimType = 'Organisation'", conn.Database);
                var claimRes = cmd.ExecuteNonQuery();
                conn.Close();
            }

            var Identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            Identity.RemoveClaim(Identity.FindFirst("Organisation"));
            Identity.AddClaim(new Claim("Organisation", organisationName));
            var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties() { IsPersistent = true });
        }

    }
}