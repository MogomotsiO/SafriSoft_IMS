using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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

        
    }
}