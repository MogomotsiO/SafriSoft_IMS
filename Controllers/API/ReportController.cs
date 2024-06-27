using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SafriSoftv1._3.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Report")]
    public class ReportController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpGet, Route("GetCustomers")]
        public async Task<IHttpActionResult> GetCustomers()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var CustomerViewModel = new List<CustomerViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[CustomerName],[CustomerEmail],[CustomerCell],[CustomerAddress],[DateCustomerCreated],[NumberOfOrders] from [{0}].[dbo].[Customer] where Status = '{1}' AND OrganisationId = '{2}'", conn.Database, "Active", organisationId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Customers = new CustomerViewModel();
                        countUsers += 1;
                        Customers.Id = reader.GetInt32(0);
                        Customers.CustomerName = reader.GetString(1);
                        Customers.CustomerEmail = reader.GetString(2);
                        Customers.CustomerCell = reader.GetString(3);
                        Customers.CustomerAddress = reader.GetString(4);
                        Customers.DateCustomerCreated = reader.GetString(5);
                        var numberOfOrders = SafriSoft.Orders.Where(x => x.CustomerId == Customers.Id).Count();
                        var customer = SafriSoft.Customers.Where(x => x.Id == Customers.Id).FirstOrDefault();
                        customer.NumberOfOrders = numberOfOrders;
                        SafriSoft.SaveChanges();
                        Customers.NumberOfOrders = numberOfOrders;
                        CustomerViewModel.Add(Customers);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(CustomerViewModel);
            }

        }

        [HttpGet, Route("GetProducts")]
        public async Task<IHttpActionResult> GetProducts()
        {
            SafriSoftDbContext SafriSoftDb = new SafriSoftDbContext();
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var ProductViewModel = new List<ProductViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[ProductName],[ProductReference],[SellingPrice],[ItemsSold],[ItemsAvailable],[Status],[ProductCategory],[ProductImage],[ProductCode],[Cost] from [{0}].[dbo].[Product] where Status = '{1}' AND OrganisationId = '{2}'", conn.Database, "Active", organisationId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Products = new ProductViewModel();
                        countUsers += 1;
                        Products.Id = reader.GetInt32(0);
                        Products.ProductName = reader.GetString(1);
                        Products.ProductReference = reader.GetString(2);
                        Products.Cost = reader.GetDouble(10);
                        Products.SellingPrice = reader.GetDouble(3);
                        Products.ItemsSold = reader.GetInt32(4);
                        Products.ItemsAvailable = reader.GetInt32(5);
                        Products.ProductCategory = reader.GetString(7);
                        Products.ProductImage = reader.GetString(8);
                        Products.ProductCode = reader.GetString(9);

                        // calculations very serious stuff
                        Products.TotalItems = Products.ItemsSold + Products.ItemsAvailable;
                        Products.TotalItemsCost = Products.TotalItems * Products.Cost;
                        Products.ExpectedProfit = (Products.TotalItems * Products.SellingPrice) - (Products.TotalItemsCost);
                        Products.CurrProfit = Products.ItemsSold * Products.SellingPrice - (Products.ItemsSold * Products.Cost);

                        ProductViewModel.Add(Products);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(ProductViewModel);
            }
        }

        [HttpGet, Route("GetOrders")]
        public async Task<IHttpActionResult> GetOrders()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrderViewModel = new List<OrderViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [OrderId],[ProductName],[NumberOfItems],[CustomerName],[OrderStatus],[OrderProgress],[DateOrderCreated],[ExpectedDeliveryDate],[OrderWorth],[ShippingCost] from [{0}].[dbo].[Orders] where Status = '{1}' AND OrganisationId = '{2}' ORDER BY OrderProgress ASC", conn.Database, "Active", organisationId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Orders = new OrderViewModel();
                        countUsers += 1;
                        Orders.OrderId = reader.GetString(0);
                        Orders.ProductName = reader.GetString(1);
                        Orders.NumberOfItems = reader.GetInt32(2);
                        Orders.CustomerName = reader.GetString(3);
                        Orders.OrderStatus = reader.GetString(4);
                        Orders.OrderProgress = reader.GetInt32(5);
                        Orders.DateOrderCreated = reader.GetString(6);
                        Orders.ExpectedDeliveryDate = reader.GetString(7);
                        Orders.OrderWorth = reader.GetDecimal(8);
                        Orders.ShippingCost = reader.GetDecimal(9);
                        OrderViewModel.Add(Orders);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(OrderViewModel);
            }
        }

        [HttpGet, Route("GetSystemNotifications")]
        public async Task<IHttpActionResult> GetSystemNotifications()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrderViewModel = new List<OrderViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [OrderId],[ProductName],[NumberOfItems],[CustomerName],[OrderStatus],[ExpectedDeliveryDate],[OrderWorth],[DateOrderCreated] from [{0}].[dbo].[Orders] where OrganisationId = {1} AND Status = 'Active' AND ExpectedDeliveryDate < DateOrderCreated and (OrderStatus = 'Processed' or OrderStatus = 'Packaged') Order By OrderProgress ASC, Status ASC", conn.Database, organisationId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Orders = new OrderViewModel();
                        countUsers += 1;
                        Orders.OrderId = reader.GetString(0);
                        Orders.ProductName = reader.GetString(1);
                        Orders.NumberOfItems = reader.GetInt32(2);
                        Orders.CustomerName = reader.GetString(3);
                        Orders.OrderStatus = reader.GetString(4);
                        Orders.ExpectedDeliveryDate = reader.GetString(5);
                        Orders.OrderWorth = reader.GetDecimal(6);
                        Orders.DateOrderCreated = reader.GetString(7);
                        OrderViewModel.Add(Orders);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(OrderViewModel);
            }
        }

        public int GetOrganisationId(string organisationName)
        {
            int organisationId = 0;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [OrganisationId] from [{0}].[dbo].[Organisations] where OrganisationName = '{1}'", conn.Database, organisationName);
                organisationId = (int)cmd.ExecuteScalar();
            }

            return organisationId;
        }
    }
}