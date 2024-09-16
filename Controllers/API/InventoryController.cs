using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.SystemModels;
using SafriSoftv1._3.Models.ViewModels;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Inventory")]
    public class InventoryController : BaseApiController
    {

        // GET: api/Inventory
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Inventory/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Inventory
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Inventory/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Inventory/5
        public void Delete(int id)
        {
        }

        [HttpGet, Route("GetUserData")]
        public async Task<IHttpActionResult> GetUserData()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var UserViewModel = new List<UserViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var username = "";
            var userState = "";

            var safriDbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString());
            safriDbConn.Open();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT u.[Id],[Email],[UserName], [FirstName], [LastName], [Read], [Write] from [dbo].[AspNetUsers] u, [dbo].[AspNetUserClaims] c WHERE u.Id = c.UserId AND c.ClaimType = 'Organisation' AND c.ClaimValue = '{1}'", conn.Database, getOrgClaim);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var usernameClaim = userManager.GetClaims(reader.GetString(0)).First(x => x.Type == "Username");
                            username = usernameClaim.Value;
                        }
                        catch (Exception Ex)
                        {
                            username = reader.GetString(1);
                        }
                        try
                        {
                            var usernameState = userManager.GetClaims(reader.GetString(0)).First(x => x.Type == "AccountLocked");
                            userState = usernameState.Value;
                        }
                        catch (Exception Ex)
                        {
                            userState = "";
                        }

                        var getUserId = reader.GetString(0);
                        var userRoles = userManager.GetRoles(getUserId).ToList();

                        var Users = new UserViewModel();
                        countUsers += 1;
                        Users.Id = countUsers;
                        Users.UserId = getUserId;
                        Users.FirstName = Convert.ToString(reader.GetValue(3));
                        Users.LastName = Convert.ToString(reader.GetValue(4));
                        Users.Read = Convert.ToBoolean(reader.GetValue(5));
                        Users.Write = Convert.ToBoolean(reader.GetValue(6));
                        Users.Email = reader.GetString(1);
                        Users.Username = username;
                        Users.UserRoles = userRoles;
                        Users.UserState = userState;
                        var numberOfOrdersCmd = conn.CreateCommand();
                        numberOfOrdersCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE UserId = '{1}'", safriDbConn.Database, reader.GetString(0));
                        Users.NumberOfOrders = (Int32)numberOfOrdersCmd.ExecuteScalar();
                        var randValueSoldCmd = conn.CreateCommand();
                        randValueSoldCmd.CommandText = string.Format("SELECT sum([OrderWorth]) from [{0}].[dbo].[Orders] WHERE UserId = '{1}'", safriDbConn.Database, reader.GetString(0));
                        try
                        {
                            Users.RandValueSold = (double)randValueSoldCmd.ExecuteScalar();
                        }
                        catch (Exception Ex)
                        {
                            Users.RandValueSold = 0;
                        }

                        UserViewModel.Add(Users);
                    }
                    reader.NextResult();
                    reader.Close();
                }
                safriDbConn.Close();
                conn.Close();
                return Json(UserViewModel);
            }
        }

        [HttpGet, Route("GetUserDetails/{id}")]
        public async Task<IHttpActionResult> GetUserDetails(string id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var username = "";
            var userState = "";

            var appDbContext = new ApplicationDbContext();

            var safriDbConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString());
            safriDbConn.Open();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT u.[Id],[Email],[UserName], [FirstName], [LastName], [Read], [Write] from [dbo].[AspNetUsers] u, [dbo].[AspNetUserClaims] c WHERE u.Id = '{2}' AND c.ClaimType = 'Organisation' AND c.ClaimValue = '{1}'", conn.Database, getOrgClaim, id);

                var Users = new UserViewModel();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            var usernameClaim = userManager.GetClaims(reader.GetString(0)).First(x => x.Type == "Username");
                            username = usernameClaim.Value;
                        }
                        catch (Exception Ex)
                        {
                            username = reader.GetString(1);
                        }
                        try
                        {
                            var usernameState = userManager.GetClaims(reader.GetString(0)).First(x => x.Type == "AccountLocked");
                            userState = usernameState.Value;
                        }
                        catch (Exception Ex)
                        {
                            userState = "";
                        }

                        Users.UserId = reader.GetString(0);

                        var userRoles = userManager.GetRoles(Users.UserId).ToList();
                        var userRolesDetails = appDbContext.Roles.Where(x => userRoles.Contains(x.Name)).Select(x => x.Id).ToList();
                                                
                        countUsers += 1;
                        Users.Id = countUsers;
                        
                        Users.Email = reader.GetString(1);
                        Users.Username = username;
                        Users.UserRoles = userRoles;
                        Users.UserRoleIds = userRolesDetails;
                        Users.UserState = userState;
                        Users.FirstName = Convert.ToString(reader.GetValue(3));
                        Users.LastName = Convert.ToString(reader.GetValue(4));
                        Users.Read = Convert.ToBoolean(reader.GetValue(5));
                        Users.Write = Convert.ToBoolean(reader.GetValue(6));
                        var numberOfOrdersCmd = conn.CreateCommand();
                        numberOfOrdersCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE UserId = '{1}'", safriDbConn.Database, reader.GetString(0));
                        Users.NumberOfOrders = (Int32)numberOfOrdersCmd.ExecuteScalar();
                        var randValueSoldCmd = conn.CreateCommand();
                        randValueSoldCmd.CommandText = string.Format("SELECT sum([OrderWorth]) from [{0}].[dbo].[Orders] WHERE UserId = '{1}'", safriDbConn.Database, reader.GetString(0));
                        try
                        {
                            Users.RandValueSold = (double)randValueSoldCmd.ExecuteScalar();
                        }
                        catch (Exception Ex)
                        {
                            Users.RandValueSold = 0;
                        }

                    }
                    reader.NextResult();
                    reader.Close();
                }
                safriDbConn.Close();
                conn.Close();
                return Json(Users);
            }
        }

        [HttpPost, Route("UserCreate")]
        public async Task<IHttpActionResult> UserCreate(RegisterViewModel UserData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var UserViewModel = new List<UserViewModel>();

            var user = new ApplicationUser { UserName = UserData.Email, Email = UserData.Email, FirstName = UserData.FirstName, LastName = UserData.LastName, Read = UserData.Read, Write = UserData.Write };
            var result = await userManager.CreateAsync(user, UserData.Password);

            if (result.Succeeded)
            {
                Claim OrganisationClaim = new Claim("Organisation", UserData.OrganisationName);
                var saveClaim = await userManager.AddClaimAsync(user.Id, OrganisationClaim);
                Claim UsernameClaim = new Claim("Username", UserData.Username);
                var saveUsernameClaim = await userManager.AddClaimAsync(user.Id, UsernameClaim);

                var appDbContext = new ApplicationDbContext();

                foreach (var role in UserData.Roles)
                {
                    var userRoleDetails = appDbContext.Roles.Where(x => x.Id == role).FirstOrDefault();

                    var saveRole = userManager.AddToRole(user.Id, userRoleDetails.Name);
                }                

                var subject = "SafriSoft - Access";
                var emailBody = $"You have been granted access to the SafriSoft Business Management Software by your Organisation Admin. <br/><br/> Please use the below details to access the software: <br/> Username: {user.UserName} <br/> Password: {UserData.Password}";

                var toAddress = new List<string>();
                var toCCAddress = new List<string>();
                toAddress.Add(user.Email);
                var createEmail = new SafriSoftEmailService();
                createEmail.SaveEmail(subject, emailBody, "support@safrisoft.com", toAddress.ToArray(), toCCAddress.ToArray());
                //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            }

            return Json(result);
        }

        [HttpPost, Route("UpdateUser")]
        public async Task<IHttpActionResult> UpdateUser(RegisterViewModel UserData)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var iSvc = new InventoryService();

                result.obj = iSvc.UpdateUser(UserData, organisationId);

                result.Success = true;
                result.Message = "User updated";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("UserLock")]
        public async Task<IHttpActionResult> UserLock(UserViewModel UserData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var UserViewModel = new List<UserViewModel>();
            var userState = "";

            try
            {
                var usernameState = userManager.GetClaims(UserData.UserId).First(x => x.Type == "AccountLocked");
                userState = usernameState.Value;
            }
            catch (Exception Ex)
            {
                userState = "Error";
            }

            try
            {
                if (userState == "Error")
                {
                    Claim UsernameStateClaim = new Claim("AccountLocked", "Locked");
                    var saveUsernameStateClaim = await userManager.AddClaimAsync(UserData.UserId, UsernameStateClaim);
                }
                else if (userState == "Locked")
                {
                    Claim UsernameStateClaim = new Claim("AccountLocked", "Locked");
                    userManager.RemoveClaim(UserData.UserId, UsernameStateClaim);
                    Claim UsernameStateClaimU = new Claim("AccountLocked", "UnLocked");
                    var saveUsernameStateClaim = await userManager.AddClaimAsync(UserData.UserId, UsernameStateClaimU);
                }
                else if (userState == "UnLocked")
                {
                    Claim UsernameStateClaimU = new Claim("AccountLocked", "UnLocked");
                    userManager.RemoveClaim(UserData.UserId, UsernameStateClaimU);
                    Claim UsernameStateClaimL = new Claim("AccountLocked", "Locked");
                    var saveUsernameStateClaim = await userManager.AddClaimAsync(UserData.UserId, UsernameStateClaimL);
                }
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.Message.ToString() });
            }

        }

        [HttpGet, Route("GetProducts/{Id}")]
        public async Task<IHttpActionResult> GetProducts(int Id)
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
                cmd.CommandText = string.Format("SELECT [Id],[ProductName],[ProductReference],[SellingPrice],[ItemsSold],[ItemsAvailable],[Status],[ProductCategory],[ProductImage],[ProductCode],[Cost],[InventoryAccountId],[ProductType] from [{0}].[dbo].[Product] where Id = {1} AND OrganisationId = '{2}'", conn.Database, Id, organisationId);

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
                        Products.InventoryAccountId = reader.GetInt32(11);
                        Products.ProductType = (ProductType)reader.GetInt32(12);
                        Products.ProductTypeStr = Products.ProductType.ToString() == "0" ? string.Empty : Products.ProductType.ToString();

                        var requirements = SafriSoftDb.ProductRequirements.Where(x => x.ProductId == Id).ToList();

                        foreach (var req in requirements)
                        {
                            Products.Requirements.Add(new RequirementViewModel{
                                Id = req.RequiredProductId,
                                Name = req.ProductName,
                                Qty = req.QtyRequired
                            });
                        }

                        if(Products.Requirements != null && Products.Requirements.Count() > 0)
                        {
                            Products.Products = Products.Requirements.Select(x => x.Id).ToArray();
                        }                       

                        ProductViewModel.Add(Products);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(ProductViewModel);
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
            var aSvc = new AccountingService();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[ProductName],[ProductReference],[SellingPrice],[ItemsSold],[ItemsAvailable],[Status],[ProductCategory],[ProductImage],[ProductCode],[Cost],[InventoryAccountId],[ProductType] from [{0}].[dbo].[Product] where Status = '{1}' AND OrganisationId = '{2}'", conn.Database, "Active", organisationId);

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
                        Products.InventoryAccountId = reader.GetInt32(11);
                        Products.InventoryAccountName = aSvc.GetTrialBalanceAccountDetails(Products.InventoryAccountId, organisationId);
                        Products.ProductType = (ProductType)reader.GetInt32(12);
                        Products.ProductTypeStr = Products.ProductType.ToString() == "0" ? string.Empty : Products.ProductType.ToString();

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

        [HttpPost, Route("ProductCreate")]
        public async Task<IHttpActionResult> ProductCreate(BaseViewModel vm)
        {
            var ProductData = JsonConvert.DeserializeObject<ProductViewModel>(vm.JsonString);

            var productCode = ProductData.ProductCode;
            var productCategory = ProductData.ProductCategory;
            var productName = ProductData.ProductName;
            var productReference = ProductData.ProductReference;
            var cost = ProductData.Cost;
            var sellingPrice = ProductData.SellingPrice;
            var itemsSold = 0;
            var itemsAvailable = 0;
            var productImage = ProductData.ProductImage;
            var inventoryAccountId = ProductData.InventoryAccountId;
            var productType = ProductData.ProductType;
            var products = ProductData.Products;

            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            var product = SafriSoft.Products.FirstOrDefault(x => x.ProductReference == productReference || x.ProductCode == productCode);

            if (product != null)
            {
                return BadRequest("Product already exists");
            }

            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[Product] ([ProductName],[ProductReference],[SellingPrice],[ItemsSold],[ItemsAvailable],[Status],[ProductCategory],[ProductImage],[ProductCode],[OrganisationId],[Cost],[InventoryAccountId],[ProductType]) output INSERTED.Id " +
                                                    "VALUES('{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}', {12},{13})",
                                                    conn.Database, productName, productReference, sellingPrice, itemsSold, itemsAvailable, "Active", productCategory, productImage, productCode, organisationId, cost, inventoryAccountId, (int)productType);
                    
                    var id = (int)cmd.ExecuteScalar();

                    var iSvc = new InventoryService();

                    if (id > 0 && products.Count() > 0)
                    {
                        var result = iSvc.SaveProductRequirements(id, ProductData.Requirements, organisationId);
                    }

                    var auditResult = iSvc.SaveProductAudit(id,"Product created",organisationId,userId);

                }
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }

        }

        [HttpPost, Route("ProductUpdate")]
        public async Task<IHttpActionResult> ProductUpdate(BaseViewModel vm)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            var ProductData = JsonConvert.DeserializeObject<ProductViewModel>(vm.JsonString);

            var sb = new StringBuilder();

            try
            {
                var dbProduct = SafriSoft.Products.First(x => x.Id == ProductData.Id);

                if (dbProduct.ProductName != ProductData.ProductName)
                {
                    dbProduct.ProductName = ProductData.ProductName;
                }

                if (dbProduct.ProductReference != ProductData.ProductReference)
                {
                    dbProduct.ProductReference = ProductData.ProductReference;
                }

                if (dbProduct.SellingPrice != ProductData.SellingPrice)
                {                    
                    sb.AppendLine($"Selling Price: From - {dbProduct.SellingPrice} To - {ProductData.SellingPrice} <br>");
                    dbProduct.SellingPrice = ProductData.SellingPrice;
                }

                //if (dbProduct.ItemsAvailable != ProductData.ItemsAvailable)
                //{
                //    dbProduct.ItemsAvailable = ProductData.ItemsAvailable;
                //}

                if (dbProduct.ProductCode != ProductData.ProductCode)
                {
                    dbProduct.ProductCode = ProductData.ProductCode;
                }

                if (dbProduct.ProductCategory != ProductData.ProductCategory)
                {
                    dbProduct.ProductCategory = ProductData.ProductCategory;
                }

                if (dbProduct.Cost != ProductData.Cost)
                {                    
                    sb.AppendLine($"Cost: From - {dbProduct.Cost} To - {ProductData.Cost} <br>");
                    dbProduct.Cost = ProductData.Cost;
                }
                
                if (dbProduct.InventoryAccountId != ProductData.InventoryAccountId)
                {
                    //var fromAccount = SafriSoft.TrialBalanceAccounts.Where(x => x.Id == dbProduct.InventoryAccountId).FirstOrDefault();
                    //var toAccount = SafriSoft.TrialBalanceAccounts.Where(x => x.Id == ProductData.InventoryAccountId).FirstOrDefault();

                    //sb.AppendLine($"Inventory Account: From - {fromAccount.AccountName} To - {toAccount.AccountName} <br>");
                    dbProduct.InventoryAccountId = ProductData.InventoryAccountId;
                }

                if (dbProduct.ProductType != ProductData.ProductType)
                {
                    sb.AppendLine($"Product Type: From - {dbProduct.ProductType.ToString()} To - {ProductData.ProductType.ToString()} <br>");
                    dbProduct.ProductType = ProductData.ProductType;
                }

                var iSvc = new InventoryService();

                if (ProductData.Products != null && ProductData.Products.Count() > 0)
                {                    
                    var result = iSvc.SaveProductRequirements(dbProduct.Id, ProductData.Requirements, dbProduct.OrganisationId);
                }

                if(string.IsNullOrEmpty(sb.ToString()) == false)
                {
                    var auditResult = iSvc.SaveProductAudit(dbProduct.Id, sb.ToString(), dbProduct.OrganisationId, User.Identity.GetUserId());
                }

                //if (dbProduct.ProductImage != ProductData.ProductImage)
                //{
                //    dbProduct.ProductImage = ProductData.ProductImage;
                //}

                SafriSoft.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }

        }

        [HttpPost, Route("ProductDelete")]
        public async Task<IHttpActionResult> ProductDelete(ProductViewModel ProductData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            try
            {
                var dbProduct = SafriSoft.Products.First(x => x.Id == ProductData.Id);
                dbProduct.Status = "Deleted";
                SafriSoft.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }

        }

        [HttpPost, Route("AddQuantity")]
        public async Task<IHttpActionResult> AddQuantity(BaseViewModel vm)
        {
            var result = new Result();

            try
            {
                var quantityVm = JsonConvert.DeserializeObject<AddQuantityViewModel>(vm.JsonString);
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var svc = new InventoryService();

                result = svc.AddQuantity(quantityVm, organisationId);

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;

                return Json(result);
            }
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
                        var customerId = reader.GetInt32(0);

                        var balance = 0.0;
                        var transactions = SafriSoft.CustomerTransactions.Where(x => x.CustomerId == customerId).ToList();

                        if (transactions.Count > 0)
                        {
                            balance = transactions.Sum(x => x.Amount);
                        }

                        var Customers = new CustomerViewModel();
                        countUsers += 1;
                        Customers.Id = customerId;
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
                        Customers.Balance = balance;
                        CustomerViewModel.Add(Customers);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(CustomerViewModel);
            }
        }

        [HttpGet, Route("GetCustomer/{Id}")]
        public async Task<IHttpActionResult> GetCustomer(int Id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var CustomerViewModel = new List<CustomerViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[CustomerName],[CustomerEmail],[CustomerCell],[CustomerAddress],[DateCustomerCreated],[NumberOfOrders] from [{0}].[dbo].[Customer] where Id = {1}", conn.Database, Id);

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

        [HttpPost, Route("CustomerCreate")]
        public async Task<IHttpActionResult> CustomerCreate(CustomerViewModel CustomerData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            var customerName = CustomerData.CustomerName;
            var customerEmail = CustomerData.CustomerEmail;
            var customerAddress = CustomerData.CustomerAddress;
            var customerCell = CustomerData.CustomerCell;
            var dateCustomerCreated = CustomerData.DateCustomerCreated;

            var result = CheckPackageAccess("Customers", organisationId);

            if (result == true)
            {
                return Json(new { Success = false, Error = "You have exceeded the number of customers to add. Please upgrade to Premium" });
            }

            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[Customer] ([CustomerName],[CustomerEmail],[CustomerCell],[CustomerAddress],[DateCustomerCreated],[Status],[NumberOfOrders],[OrganisationId]) VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", conn.Database, customerName, customerEmail, customerCell, customerAddress, dateCustomerCreated, "Active", 1, organisationId);
                    await cmd.ExecuteNonQueryAsync();

                }
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpPost, Route("CustomerUpdate")]
        public async Task<IHttpActionResult> CustomerUpdate(CustomerViewModel CustomerData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            try
            {
                var dbCustomer = SafriSoft.Customers.First(x => x.Id == CustomerData.Id);

                if (dbCustomer.CustomerName != CustomerData.CustomerName)
                {
                    dbCustomer.CustomerName = CustomerData.CustomerName;
                }

                if (dbCustomer.CustomerEmail != CustomerData.CustomerEmail)
                {
                    dbCustomer.CustomerEmail = CustomerData.CustomerEmail;
                }

                if (dbCustomer.CustomerCell != CustomerData.CustomerCell)
                {
                    dbCustomer.CustomerCell = CustomerData.CustomerCell;
                }

                if (dbCustomer.CustomerAddress != CustomerData.CustomerAddress)
                {
                    dbCustomer.CustomerAddress = CustomerData.CustomerAddress;
                }

                SafriSoft.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }

        }

        [HttpPost, Route("CustomerDelete")]
        public async Task<IHttpActionResult> CustomerDelete(CustomerViewModel CustomerData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            try
            {
                var dbCustomer = SafriSoft.Customers.First(x => x.Id == CustomerData.Id);
                dbCustomer.Status = "Deleted";
                SafriSoft.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex.ToString());
            }

        }

        [HttpPost, Route("UploadDocument/{Id}")]
        public async Task<IHttpActionResult> UploadDocument(HttpRequestMessage request, int Id)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["files[]"];

            string fileName = postedFile.FileName;
            string fileContentType = postedFile.ContentType;

            var saveFileDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Documents/Customers");

            if (Directory.Exists(saveFileDir) == false)
            {
                Directory.CreateDirectory(saveFileDir);
            }

            var fullFileName = $"{saveFileDir}/{fileName}";

            var result = new Result();

            try
            {
                postedFile.SaveAs(fullFileName);

                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var ivSvc = new InventoryService();
                result = ivSvc.UploadCustomerDocuments(fileName, Id, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }

        }

        [HttpGet, Route("GetCustomerDocuments/{Id}")]
        public async Task<IHttpActionResult> GetCustomerDocuments(int Id = 0)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);
                var ivSvc = new InventoryService();
                result = ivSvc.GetDocuments(Id, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
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

        [HttpGet, Route("GetOrders/{id}")]
        public async Task<IHttpActionResult> GetOrders(int id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrderViewModel = new List<OrderViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [OrderId],[ProductName],[NumberOfItems],[CustomerName],[OrderStatus],[OrderProgress],[DateOrderCreated],[ExpectedDeliveryDate],[OrderWorth] from [{0}].[dbo].[Orders] WHERE CustomerId = {1} ORDER BY OrderProgress ASC", conn.Database, id);

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
                        OrderViewModel.Add(Orders);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(OrderViewModel);
            }
        }

        [HttpPost, Route("OrderCreate")]
        public async Task<IHttpActionResult> OrderCreate(OrderViewModel OrderData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

            var customerId = OrderData.CustomerId;
            var customerName = OrderData.CustomerName;
            var productName = OrderData.ProductName;
            var productReference = OrderData.ProductReference.Replace('_', '/');
            var numberOfItems = OrderData.NumberOfItems;
            var dateOrderCreated = OrderData.DateOrderCreated;
            var expectedDateOfDelivery = OrderData.ExpectedDeliveryDate;
            var description = "Inception - This order was created!";
            var changed = "Inception";

            var result = CheckPackageAccess("Orders", organisationId);

            if (result == true)
            {
                return Json(new { Success = false, Error = "You have exceeded the number of orders to add. Please upgrade to Premium" });
            }

            //Random rnd = new Random();

            //string generateOrderId = "#0";

            //for (int i = 0; i <= 9; i++)
            //{
            //    generateOrderId = generateOrderId + rnd.Next(1, 9).ToString();
            //}

            var service = new InvoicingService();

            var invoiceNumber = service.GetNextInvoiceNumber(organisationId);

            string generateOrderId = $"#{invoiceNumber}";

            var product = SafriSoft.Products.Where(x => x.ProductReference == productReference).FirstOrDefault();

            if (product.ItemsAvailable == 0)
            {
                return Json(new { Success = false, Error = "This product is no longer available!" });
            }

            if (product.ItemsAvailable < numberOfItems)
            {
                return Json(new { Success = false, Error = "The number of items exceed what's in stock!" });
            }

            var orderWorth = product.SellingPrice * numberOfItems;

            var finalNumberOfItems = product.ItemsAvailable - numberOfItems;
            var finalSold = product.ItemsSold + numberOfItems;
            product.ItemsAvailable = finalNumberOfItems;
            product.ItemsSold = finalSold;
            SafriSoft.SaveChanges();

            try
            {
                var vat = orderWorth * (OrderData.VatPercentage / 100);

                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[Orders] ([OrderId],[ProductName],[NumberOfItems],[CustomerId],[CustomerName],[OrderStatus],[OrderProgress],[DateOrderCreated],[ExpectedDeliveryDate],[UserId],[OrganisationId]) VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", conn.Database, generateOrderId, productName, numberOfItems, customerId, customerName, "Processed", 10, dateOrderCreated, expectedDateOfDelivery, userId, organisationId);
                    await cmd.ExecuteNonQueryAsync();

                    var auditCmd = conn.CreateCommand();
                    auditCmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[OrderAudit] ([Description],[Changed],[CreatedDate],[UserId],[OrderId]) VALUES('{1}','{2}','{3}','{4}','{5}')", conn.Database, description, changed, DateTime.Now, userId, generateOrderId);
                    await auditCmd.ExecuteNonQueryAsync();

                }
                if (orderWorth > 0)
                {
                    var order = SafriSoft.Orders.First(x => x.OrderId == generateOrderId);
                    order.OrderWorth = Decimal.Parse(orderWorth.ToString()) + OrderData.ShippingCost + Decimal.Parse(vat.ToString());
                    order.ShippingCost = Decimal.Parse(OrderData.ShippingCost.ToString());
                    order.Status = "Active";
                    SafriSoft.SaveChanges();
                }
                var customerIdParse = int.Parse(customerId);
                var customer = SafriSoft.Customers.FirstOrDefault(x => x.Id == customerIdParse);
                var subject = "SafriSoft - Order Received";
                var downloadLink = "https://ims.safrisoft.com/Inventory/CustomerInvoicePdf?OrderId=" + generateOrderId.Substring(1, generateOrderId.Length - 1) + "&organisationName=" + getOrgClaim;
                var emailBody = "Your order has been received.<br/><br/> We will communicate further updates about your order through email.<br /><br/> Download invoice here: <a href='" + downloadLink + "'>Invoice</a>";

                var toAddress = new List<string>();
                var toCCAddress = new List<string>();
                toAddress.Add(customer.CustomerEmail);
                var createEmail = new SafriSoftEmailService();
                createEmail.SaveEmail(subject, emailBody, "support@safrisoft.com", toAddress.ToArray(), toCCAddress.ToArray());

                

                var invoiceVm = new InvoicingViewModel
                {
                    CustomerId = Convert.ToInt32(customerId),
                    InvoiceDetails = new Invoice
                    {
                        InvoiceNumber = $"INV-{invoiceNumber}",
                        InvoiceDescription = $"{generateOrderId} - {OrderData.ProductName}",
                        InvoiceDate = DateTime.Now,
                        InvoiceDueDate = OrderData.InvoiceDueDate,
                        Shipping = Convert.ToDouble(OrderData.ShippingCost),
                        Amount = Convert.ToDouble(orderWorth + Double.Parse(OrderData.ShippingCost.ToString()) + vat),
                        CustomerId = Convert.ToInt32(customerId),
                        Reference = generateOrderId,
                        VatOptionId = OrderData.VatOptionId,
                        InvoiceAccountId = OrderData.InvoiceAccountId,
                        DebtorsAccountId = OrderData.DebtorsAccountId
                    },
                    InvoiceItems = new List<InvoiceItem>() { new InvoiceItem()
                    {
                        Description = product.ProductName,
                        Amount = orderWorth,
                        Qty = numberOfItems,                        
                    }},
                    
                };

                service.SaveInvoice(invoiceVm,organisationId);

                return Json(new { Success = true, CustomerID = customerId, CustomerName = customer.CustomerName });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString(), OrderWorth = orderWorth, OrderWorthToString = orderWorth.ToString("#.##") });
            }

        }

        [HttpPost, Route("ChangeStatus")]
        public async Task<IHttpActionResult> ChangeStatus(OrderViewModel OrderData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var description = "Status - This order was changed to " + OrderData.OrderStatus + " !";
            var changed = OrderData.OrderStatus;

            try
            {
                var order = SafriSoft.Orders.Where(x => x.OrderId == OrderData.OrderId).FirstOrDefault();
                order.OrderProgress = OrderData.OrderProgress;
                order.OrderStatus = OrderData.OrderStatus;
                SafriSoft.SaveChanges();

                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var auditCmd = conn.CreateCommand();
                    auditCmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[OrderAudit] ([Description],[Changed],[CreatedDate],[UserId],[OrderId]) VALUES('{1}','{2}','{3}','{4}','{5}')", conn.Database, description, changed, DateTime.Now, userId, order.OrderId);
                    await auditCmd.ExecuteNonQueryAsync();

                }

                //var customer = SafriSoft.Customers.FirstOrDefault(x => x.Id == order.CustomerId);
                //var subject = OrderData.OrderStatus;
                //var emailBody = "Your order has been updated.<br/><br/> Order Id: " + OrderData.OrderId + "<br/><br/>" + description;

                //var toAddress = new List<string>();
                //var toCCAddress = new List<string>();
                //toAddress.Add(customer.CustomerEmail);
                //var createEmail = new SafriSoftEmailService();
                //createEmail.SaveEmail(subject,emailBody,"support@safrisoft.com",toAddress.ToArray(), toCCAddress.ToArray());

                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpPost, Route("OrderDelete")]
        public async Task<IHttpActionResult> OrderDelete(OrderViewModel OrderData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var status = "Deleted";

            try
            {
                var order = SafriSoft.Orders.Where(x => x.OrderId == OrderData.OrderId).FirstOrDefault();
                order.Status = status;
                SafriSoft.SaveChanges();
                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpGet, Route("Home")]
        public IHttpActionResult Home()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            var customerCount = 0;
            var stockSold = 0;
            var stockAvailable = 0;
            var ordersProcessed = 0;
            var ordersPackaged = 0;
            var ordersInTransit = 0;
            var ordersDelivered = 0;
            double randValueSold = 0;

            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var customerCountCmd = conn.CreateCommand();
                    customerCountCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Customer] WHERE [Status] = 'Active' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        customerCount = (Int32)customerCountCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        customerCount = 0;
                    }


                    var stockSoldCmd = conn.CreateCommand();
                    stockSoldCmd.CommandText = string.Format("SELECT sum([ItemsSold]) from [{0}].[dbo].[Product] WHERE [Status] = 'Active' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        stockSold = (Int32)stockSoldCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        stockSold = 0;
                    }


                    var stockAvailableCmd = conn.CreateCommand();
                    stockAvailableCmd.CommandText = string.Format("SELECT sum([ItemsAvailable]) from [{0}].[dbo].[Product] WHERE [Status] = 'Active' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        stockAvailable = (Int32)stockAvailableCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        stockAvailable = 0;
                    }


                    var ordersProcessedCmd = conn.CreateCommand();
                    ordersProcessedCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE [OrderStatus] = 'Processed' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        ordersProcessed = (Int32)ordersProcessedCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        ordersProcessed = 0;
                    }


                    var ordersPackagedCmd = conn.CreateCommand();
                    ordersPackagedCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE [OrderStatus] = 'Packaged' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        ordersPackaged = (Int32)ordersPackagedCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        ordersProcessed = 0;
                    }


                    var ordersInTransitCmd = conn.CreateCommand();
                    ordersInTransitCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE [OrderStatus] = 'InTransit' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        ordersInTransit = (Int32)ordersInTransitCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        ordersInTransit = 0;
                    }


                    var ordersDeliveredCmd = conn.CreateCommand();
                    ordersDeliveredCmd.CommandText = string.Format("SELECT count([Id]) from [{0}].[dbo].[Orders] WHERE [OrderStatus] = 'Delivered' AND [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        ordersDelivered = (Int32)ordersDeliveredCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        ordersDelivered = 0;
                    }


                    var randValueSoldCmd = conn.CreateCommand();
                    randValueSoldCmd.CommandText = string.Format("SELECT sum([Amount]) from [{0}].[dbo].[Invoice] WHERE [OrganisationId] = {1}", conn.Database, organisationId);
                    try
                    {
                        randValueSold = (double)randValueSoldCmd.ExecuteScalar();
                    }
                    catch (Exception Ex)
                    {
                        randValueSold = 0.0;
                    }


                }
                return Json(new { Success = true, Customers = customerCount, StockSold = stockSold, StockAvailable = stockAvailable, OrdersProcessed = ordersProcessed, OrdersPackaged = ordersPackaged, OrdersInTransit = ordersInTransit, OrdersDelivered = ordersDelivered, RandValueSold = randValueSold });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpGet, Route("GetMessages")]
        public async Task<IHttpActionResult> GetMessages()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var InboxMessageViewModel = new List<InboxViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Body],[From],[To],[Status],[DateCreated],[Id] from [{0}].[dbo].[InboxMessages] where [To] = '{1}' and Status <> '{2}' order by DateCreated DESC", conn.Database, userId, "Deleted");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var InboxMessages = new InboxViewModel();
                        var usernameFrom = userManager.GetEmail(reader.GetString(1));
                        var usernameTo = userManager.GetEmail(reader.GetString(2));
                        countUsers += 1;
                        InboxMessages.Id = reader.GetInt32(5);
                        InboxMessages.Body = reader.GetString(0);
                        InboxMessages.From = usernameFrom;
                        InboxMessages.To = usernameTo;
                        InboxMessages.Status = reader.GetString(3);
                        var date = reader.GetDateTime(4);
                        InboxMessages.DateCreated = date.ToString("MM/dd/yyyy HH:mm:ss");
                        InboxMessageViewModel.Add(InboxMessages);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(InboxMessageViewModel);
            }
        }

        [HttpGet, Route("GetChats/{Id}")]
        public async Task<IHttpActionResult> GetChats(string Id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var InboxMessageViewModel = new List<InboxViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[Body],[From],[To],[Status],[DateCreated] from [{0}].[dbo].[InboxMessages] where ([To] = '{1}' and [From] = '{2}') or ([To] = '{2}' and [From] = '{1}') order by DateCreated ASC", conn.Database, Id, userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var InboxMessages = new InboxViewModel();
                        var usernameFrom = userManager.GetEmail(reader.GetString(2));
                        var usernameTo = userManager.GetEmail(reader.GetString(3));
                        countUsers += 1;
                        InboxMessages.Id = countUsers;
                        InboxMessages.Body = reader.GetString(1);
                        InboxMessages.From = usernameFrom;
                        InboxMessages.To = usernameTo;
                        InboxMessages.Status = reader.GetString(4);
                        var date = reader.GetDateTime(5);
                        InboxMessages.DateCreated = date.ToString("MM/dd/yyyy HH:mm:ss");
                        InboxMessages.MainUser = userManager.GetEmail(userId);

                        var cmdReplies = conn.CreateCommand();
                        cmdReplies.CommandText = string.Format("SELECT [Subject],[Body],[From],[To],[Status],[MessageId],[DateCreated] from [{0}].[dbo].[InboxReplies] where [MessageId] = '{1}' order by DateCreated ASC", conn.Database, reader.GetInt32(0));

                        using (var repliesReader = cmdReplies.ExecuteReader())
                        {
                            while (repliesReader.Read())
                            {
                                InboxMessages.Replies.Add(new InboxReplies { Body = repliesReader.GetString(1), MessageId = repliesReader.GetInt32(5), DateCreated = repliesReader.GetDateTime(6) });
                            }
                            repliesReader.NextResult();
                            repliesReader.Close();
                        }
                        InboxMessageViewModel.Add(InboxMessages);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(InboxMessageViewModel);
            }
        }

        [HttpPost, Route("MessageCreate")]
        public async Task<IHttpActionResult> MessageCreate(InboxViewModel MessageData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            try
            {
                SafriSoft.InboxMessages.Add(new InboxMessages { From = userId, To = MessageData.To, Body = MessageData.Body, Status = "Created", DateCreated = DateTime.Now });
                SafriSoft.SaveChanges();
                return Json(new { Success = true, Id = MessageData.To });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpPost, Route("MessageStatusChange")]
        public async Task<IHttpActionResult> MessageStatusChange(InboxViewModel MessageData)
        {
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);

            try
            {
                var Message = SafriSoft.InboxMessages.First(x => x.Id == MessageData.Id);
                Message.Status = MessageData.Status;
                SafriSoft.SaveChanges();
                return Json(new { Success = true, UserToId = Message.From });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }

        }

        [HttpGet, Route("GetNotifications")]
        public async Task<IHttpActionResult> GetNotifications()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var InboxMessageViewModel = new List<InboxViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Body],[From],[To],[Status],[DateCreated] from [{0}].[dbo].[InboxMessages] where [To] = '{1}' and Status = '{2}' order by DateCreated ASC", conn.Database, userId, "Created");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var InboxMessages = new InboxViewModel();
                        var usernameFrom = userManager.GetEmail(reader.GetString(1));
                        var usernameTo = userManager.GetEmail(reader.GetString(2));
                        countUsers += 1;
                        InboxMessages.Id = countUsers;
                        InboxMessages.Body = reader.GetString(0);
                        InboxMessages.From = usernameFrom;
                        InboxMessages.To = usernameTo;
                        InboxMessages.Status = reader.GetString(3);
                        var date = reader.GetDateTime(4);
                        InboxMessages.DateCreated = date.ToString("MM/dd/yyyy HH:mm:ss");
                        InboxMessageViewModel.Add(InboxMessages);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(InboxMessageViewModel);
            }
        }

        [HttpPost, Route("GetOrderAudit")]
        public async Task<IHttpActionResult> GetOrderAudit(OrderViewModel OrderData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrderAuditViewModel = new List<OrderAuditViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format("SELECT [Id],[Description],[Changed],[CreatedDate],[UserId],[OrderId] from [{0}].[dbo].[OrderAudit] where [OrderId] = '{1}' order by CreatedDate ASC", conn.Database, OrderData.OrderId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var OrderAuditReords = new OrderAuditViewModel();
                        var usernameUser = userManager.GetEmail(reader.GetString(4));
                        countUsers += 1;
                        OrderAuditReords.Id = countUsers;
                        OrderAuditReords.Description = reader.GetString(1);
                        OrderAuditReords.Changed = reader.GetString(2);
                        OrderAuditReords.UserId = usernameUser;
                        var date = reader.GetDateTime(3);
                        OrderAuditReords.CreatedDate = date.ToString("MM/dd/yyyy HH:mm:ss");
                        OrderAuditViewModel.Add(OrderAuditReords);
                    }
                    reader.NextResult();
                    reader.Close();
                }

                return Json(OrderAuditViewModel);
            }
        }

        [HttpGet, Route("GetOrganisationDetails")]
        public async Task<IHttpActionResult> GetOrganisationDetails()
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrganisationViewModel = new List<OrganisationViewModel>();
            var countUsers = 0;
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            ApplicationDbContext SafriSoftAppDb = new ApplicationDbContext();

            var getOrganisation = SafriSoftAppDb.Organisations.FirstOrDefault(x => x.OrganisationName == getOrgClaim);

            //var OrganisationDetails = new OrganisationViewModel();

            //OrganisationDetails.OrganisationId = getOrganisation.OrganisationId;
            //OrganisationDetails.OrganisationName = getOrganisation.OrganisationName;
            //OrganisationDetails.OrganisationEmail = getOrganisation.OrganisationEmail;
            //OrganisationDetails.OrganisationCell = getOrganisation.OrganisationCell;
            //OrganisationDetails.OrganisationLogo = getOrganisation.OrganisationLogo;
            //OrganisationDetails.OrganisationStreet = getOrganisation.OrganisationStreet;
            //OrganisationDetails.OrganisationSuburb = getOrganisation.OrganisationSuburb;
            //OrganisationDetails.OrganisationCity = getOrganisation.OrganisationCity;
            //OrganisationDetails.OrganisationCode = getOrganisation.OrganisationCode;
            //OrganisationDetails.AccountName = "";
            //OrganisationDetails.AccountNo = 0;
            //OrganisationDetails.BankName = "";
            //OrganisationDetails.BranchName = "";
            //OrganisationDetails.BranchCode = "";
            //OrganisationDetails.ClientReference = "";
            //OrganisationDetails.VATNumber = 0;
            //OrganisationViewModel.Add(OrganisationDetails);

            return Json(getOrganisation);

            //using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString()))
            //{
            //    conn.Open();
            //    var cmd = conn.CreateCommand();
            //    cmd.CommandText = string.Format("SELECT [OrganisationId],[OrganisationName],[OrganisationEmail],[OrganisationCell],[OrganisationLogo],[OrganisationStreet],[OrganisationSuburb],[OrganisationCity],[OrganisationCode],[AccountName],[AccountNo],[BankName],[BranchName],[BranchCode],[ClientReference],[VATNumber] from [{0}].[dbo].[Organisations] WHERE [OrganisationName] = '{1}'", conn.Database, getOrgClaim);

            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            var OrganisationDetails = new OrganisationViewModel();
            //            OrganisationDetails.OrganisationId      = reader.GetInt32(0);
            //            OrganisationDetails.OrganisationName    = reader.GetString(1);
            //            OrganisationDetails.OrganisationEmail   = reader.GetString(2) != null ? reader.GetString(2) : "";
            //            OrganisationDetails.OrganisationCell    = reader.GetString(3) != null ? reader.GetString(3) : "";
            //            OrganisationDetails.OrganisationLogo    = reader.GetString(4);
            //            OrganisationDetails.OrganisationStreet  = reader.GetString(5);
            //            OrganisationDetails.OrganisationSuburb  = reader.GetString(6);
            //            OrganisationDetails.OrganisationCity    = reader.GetString(7);
            //            OrganisationDetails.OrganisationCode    = reader.GetInt32(8);
            //            OrganisationDetails.AccountName         = reader.GetString(9);
            //            OrganisationDetails.AccountNo           = reader.GetInt32(10);
            //            OrganisationDetails.BankName            = reader.GetString(11);
            //            OrganisationDetails.BranchName          = reader.GetString(12);
            //            OrganisationDetails.BranchCode          = reader.GetString(13);
            //            OrganisationDetails.ClientReference     = reader.GetString(14);
            //            OrganisationDetails.VATNumber           = reader.GetInt32(15);
            //            OrganisationViewModel.Add(OrganisationDetails);
            //        }
            //        reader.NextResult();
            //        reader.Close();
            //    }

            //    return Json(OrganisationViewModel);
            //}
        }

        [HttpPost, Route("SaveOrganisationDetails")]
        public async Task<IHttpActionResult> SaveOrganisationDetails(Organisations Organisation)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var OrganisationViewModel = new List<OrganisationViewModel>();
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            ApplicationDbContext SafriSoft = new ApplicationDbContext();

            try
            {
                Organisation.UserId = userId;
                SafriSoft.Entry(Organisation).State = EntityState.Modified;
                SafriSoft.SaveChanges();

                return Json(new { Success = true });
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }
        }

        [HttpPost, Route("GetInvoiceData")]
        public async Task<IHttpActionResult> GetInvoiceData(OrderViewModel OrderData)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;

            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            var InvoiceViewModel = new List<InvoiceViewModel>();
            var OrganisationViewModel = new List<OrganisationViewModel>();
            var OrderViewModel = new List<OrderViewModel>();
            var InvoiceDetails = new InvoiceViewModel();
            var customerId = "";

            try
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();

                    var orgConn = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbContext"].ToString());
                    var cmd = orgConn.CreateCommand();
                    cmd.CommandText = string.Format("SELECT [OrganisationId],[OrganisationName],[OrganisationEmail],[OrganisationCell],[OrganisationLogo],[OrganisationStreet],[OrganisationSuburb],[OrganisationCity],[OrganisationCode],[AccountName],[AccountNo],[BankName],[BranchName],[BranchCode],[ClientReference],[VATNumber] from [{0}].[dbo].[Organisations] WHERE [OrganisationName] = '{1}'", orgConn.Database, getOrgClaim);
                    orgConn.Open();

                    var orderCmd = conn.CreateCommand();
                    orderCmd.CommandText = string.Format("SELECT [OrderId],[ProductName],[NumberOfItems],[CustomerId],[OrderWorth],[ShippingCost],[DateOrderCreated] from [{0}].[dbo].[Orders] where OrderId = '{1}'", conn.Database, OrderData.OrderId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InvoiceDetails.OrganisationName = reader.GetString(1);
                            InvoiceDetails.OrganisationEmail = reader.GetString(2) != null ? reader.GetString(2) : "";
                            InvoiceDetails.OrganisationCell = reader.GetString(3) != null ? reader.GetString(3) : "";
                            InvoiceDetails.OrganisationLogo = reader.GetString(4);
                            InvoiceDetails.OrganisationStreet = reader.GetString(5);
                            InvoiceDetails.OrganisationSuburb = reader.GetString(6);
                            InvoiceDetails.OrganisationCity = reader.GetString(7);
                            InvoiceDetails.OrganisationCode = reader.GetInt32(8);
                            InvoiceDetails.AccountName = reader.GetString(9);
                            InvoiceDetails.AccountNo = reader.GetInt32(10);
                            InvoiceDetails.BankName = reader.GetString(11);
                            InvoiceDetails.BranchName = reader.GetString(12);
                            InvoiceDetails.BranchCode = reader.GetString(13);
                            InvoiceDetails.ClientReference = reader.GetString(14);
                            InvoiceDetails.VATNumber = reader.GetInt32(15);
                        }
                        reader.NextResult();
                        reader.Close();
                    }

                    orgConn.Close();

                    using (var reader = orderCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InvoiceDetails.OrderId = reader.GetString(0);
                            InvoiceDetails.ProductName = reader.GetString(1);
                            InvoiceDetails.NumberOfItems = reader.GetInt32(2);
                            customerId = reader.GetInt32(3).ToString();
                            InvoiceDetails.OrderWorth = reader.GetDecimal(4);
                            InvoiceDetails.ShippingCost = reader.GetDecimal(5);
                            InvoiceDetails.DateOrderCreated = reader.GetString(6);
                        }
                        reader.NextResult();
                        reader.Close();
                    }

                    var customerCmd = conn.CreateCommand();
                    customerCmd.CommandText = string.Format("SELECT [CustomerName],[CustomerEmail],[CustomerCell],[CustomerAddress] from [{0}].[dbo].[Customer] where Id = '{1}'", conn.Database, customerId);

                    using (var reader = customerCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            InvoiceDetails.CustomerName = reader.GetString(0);
                            InvoiceDetails.CustomerEmail = reader.GetString(1);
                            InvoiceDetails.CustomerCell = reader.GetString(2);
                            InvoiceDetails.CustomerAddress = reader.GetString(3);
                        }
                        reader.NextResult();
                        reader.Close();
                    }

                    // Totals
                    InvoiceDetails.VatAmount = InvoiceDetails.OrderWorth * (decimal)0.15;
                    InvoiceDetails.InvoiceTotal = InvoiceDetails.OrderWorth + InvoiceDetails.VatAmount + InvoiceDetails.ShippingCost;

                    InvoiceViewModel.Add(InvoiceDetails);
                }
                return Json(InvoiceViewModel);
            }
            catch (Exception Ex)
            {
                return Json(new { Success = false, Error = Ex.ToString() });
            }
        }

        [HttpPost, Route("SafriOrderRequest")]
        public async Task<IHttpActionResult> SafriOrderRequest(OrderRequestViewModel orderRequest)
        {
            try
            {
                var createEmail = new SafriSoftEmailService();
                var toAddress = new List<string>();
                var toCCAddress = new List<string>();
                toAddress.Add("support@safrisoft.com");

                var subject = "Order Request - " + orderRequest.UserOrganisation;
                var emailBody = "An order has been request for: " + orderRequest.Package + "-Business <br/><br/> This email has been sent by: " + orderRequest.UserEmail;
                createEmail.SaveEmail(subject, emailBody, "support@safrisoft.com", toAddress.ToArray(), toCCAddress.ToArray());
                return Json(new { Success = true, Message = "Order request has been sent"});
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.ToString() });
            }
        }

        [HttpPost, Route("SendCustomEmail")]
        public async Task<IHttpActionResult> SendCustomEmail(EmailViewModel email)
        {
            try
            {
                ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                string userId = IdentityExtensions.GetUserId(User.Identity);
                var safriSoftImsDb = new SafriSoftDbContext();
                var createEmail = new SafriSoftEmailService();
                var toAddress = new List<string>();
                var toCCAddress = new List<string>();
                
                var changed = "Email Client";
                var description = $"Email Client - ({email.EmailSubject}): {email.EmailBody}";

                var orderDetails = safriSoftImsDb.Orders.FirstOrDefault(x => x.OrderId == email.OrderId);

                var customerDetails = safriSoftImsDb.Customers.FirstOrDefault(x => x.Id == orderDetails.CustomerId);

                toAddress.Add(customerDetails.CustomerEmail);

                var subject = email.EmailSubject;
                var emailBody = email.EmailBody;

                createEmail.SaveEmail(subject, emailBody, "support@safrisoft.com", toAddress.ToArray(), toCCAddress.ToArray());

                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                {
                    conn.Open();
                    var auditCmd = conn.CreateCommand();
                    auditCmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[OrderAudit] ([Description],[Changed],[CreatedDate],[UserId],[OrderId]) VALUES('{1}','{2}','{3}','{4}','{5}')", conn.Database, description, changed, DateTime.Now, userId, orderDetails.OrderId);
                    await auditCmd.ExecuteNonQueryAsync();
                    conn.Close();
                }

                return Json(new { Success = true, Message = "Email has been sent" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.ToString() });
            }
        }

        [HttpPost, Route("UploadExcelData/{Id}")]
        public async Task<IHttpActionResult> UploadExcelData(HttpRequestMessage request, string Id)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string userId = IdentityExtensions.GetUserId(User.Identity);
            var organisationClaim = userManager.GetClaims(userId).First(x => x.Type == "Organisation");
            var getOrgClaim = organisationClaim.Value;
            var organisationId = GetOrganisationId(getOrgClaim);

            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["files[]"];

            string fileName = postedFile.FileName;
            string fileContentType = postedFile.ContentType;
            byte[] fileBytes = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(postedFile.ContentLength));

            //Process the file
            IExcelDataReader excelReader;
            //ExcelPackage excelPackage = null;
            MemoryStream ms = new MemoryStream(fileBytes);

            if (postedFile.FileName.Contains("xlsx"))
            {
                //1. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(ms);
            }
            else
            {
                //2. Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(ms);
            }

            Dictionary<string, int> fileColumns = new Dictionary<string, int>();
            excelReader.Read();

            //if (excelReader.re)
            var fieldCount = excelReader.FieldCount;
            try
            {
                while (excelReader.Read())
                {
                    var productCode = excelReader.GetValue(0).ToString();
                    var productCategory = excelReader.GetValue(1).ToString();
                    var productName = excelReader.GetValue(2).ToString();
                    var NOIA = excelReader.GetValue(3).ToString();
                    var productPrice = excelReader.GetValue(5).ToString();
                    var cost = excelReader.GetValue(4).ToString();
                    var productReference = productCode + "-" + DateTime.Now.ToString("yyyy/MM/dd");

                    SafriSoftDbContext SafriSoft = new SafriSoftDbContext();

                    var product = SafriSoft.Products.FirstOrDefault(x => x.ProductReference == productReference || x.ProductCode == productCode);

                    if (product == null)
                    {
                        using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SafriSoftDbContext"].ToString()))
                        {
                            conn.Open();
                            var cmd = conn.CreateCommand();
                            cmd.CommandText = string.Format("INSERT INTO  [{0}].[dbo].[Product] ([ProductName],[ProductReference],[SellingPrice],[ItemsSold],[ItemsAvailable],[Status],[ProductCode],[ProductCategory],[ProductImage], [OrganisationId], [Cost]) VALUES('{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}', '{11}')", conn.Database, productName, productReference, productPrice, 0, NOIA, "Active", productCode, productCategory, "None", organisationId, cost);
                            await cmd.ExecuteNonQueryAsync();
                            conn.Close();
                        }
                    }

                    //excelReader.NextResult();
                }

                excelReader.Close();

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                return Json(new { Success = true });
            }
            

            
        }

        [HttpGet, Route("GetSuppliers")]
        public async Task<IHttpActionResult> GetSuppliers()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var svc = new InventoryService();

                result.obj = svc.GetSuppliers(organisationId);

                result.Message = "Successfully retrieved suppliers";
                result.Success = true;

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;

                return Json(result);
            }
        }

        [HttpGet, Route("GetSupplier/{Id}")]
        public async Task<IHttpActionResult> GetSupplier(int Id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var svc = new InventoryService();

                result.obj = svc.GetSupplier(Id, organisationId);

                result.Message = "Successfully retrieved supplier";
                result.Success = true;

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;

                return Json(result);
            }
        }

        [HttpPost, Route("SaveSupplierDetails")]
        public async Task<IHttpActionResult> SaveSupplierDetails(BaseViewModel vm)
        {
            var result = new Result();

            try
            {
                var supplierVm = JsonConvert.DeserializeObject<SupplierViewModel>(vm.JsonString);
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var svc = new InventoryService();

                result = svc.SaveSupplierDetails(supplierVm, organisationId);

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;

                return Json(result);
            }
        }

        [HttpPost, Route("UpdateSupplierDetails")]
        public async Task<IHttpActionResult> UpdateSupplierDetails(BaseViewModel vm)
        {
            var result = new Result();

            try
            {
                var supplierVm = JsonConvert.DeserializeObject<SupplierViewModel>(vm.JsonString);
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var svc = new InventoryService();

                result = svc.UpdateSupplierDetails(supplierVm, organisationId);

                return Json(result);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;

                return Json(result);
            }
        }

        [HttpPost, Route("UploadSupplierInvoices/{Id}")]
        public async Task<IHttpActionResult> UploadSupplierInvoices(int Id)
        {
            var result = new Result();
            SafriSoftDbContext SafriSoft = new SafriSoftDbContext();
            HttpContext context = HttpContext.Current;
            HttpPostedFile postedFile = context.Request.Files["files[]"];

            var date = Convert.ToDateTime(context.Request.Headers["safrisoft"].Replace('|', '-'));
            var description = context.Request.Headers["description"];
            var qty = Convert.ToInt32(context.Request.Headers["qty"]);
            var vatAmount = Convert.ToDouble(context.Request.Headers["vatAmount"]);
            var amount = Convert.ToDouble(context.Request.Headers["amount"]);
            var vatAccountId = Convert.ToInt32(context.Request.Headers["vatAccountId"]);
            var invoiceAccountId = Convert.ToInt32(context.Request.Headers["invoiceAccountId"]);
            var productId = Convert.ToInt32(context.Request.Headers["productId"]);

            if(postedFile == null)
            {
                result.Success = false;
                result.Message = "File could not be processed";
                return Json(result);
            }

            string fileName = postedFile.FileName;
            string fileContentType = postedFile.ContentType;

            var saveFileDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Documents/SupplierInvoices");

            //var saveFileDir = $"{AppDomain.CurrentDomain.BaseDirectory}/Documents/SupplierInvoices";

            if(Directory.Exists(saveFileDir) == false)
            {
                Directory.CreateDirectory(saveFileDir);
            }

            var fullFileName = $"{saveFileDir}/{fileName}";
            
            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var userId = GetUserId();

                postedFile.SaveAs(fullFileName);

                var iSvc = new InventoryService();

                result = iSvc.SaveInvoiceFileDetails(fileName, fileContentType, date, description, qty, vatAmount, amount, vatAccountId, invoiceAccountId, organisationId, Id, productId, userId);

                if (result.Success == false)
                    return Json(result);

                result.Success = true;
                result.Message = "File processed";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("LoadProductStock/{Id}")]
        public async Task<IHttpActionResult> LoadProductStock(int Id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var userId = GetUserId();

                var iSvc = new InventoryService();

                result = iSvc.LoadProductStock(Id, organisationId, userId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetSupplierInvoices/{Id}")]
        public async Task<IHttpActionResult> GetSupplierInvoices(int Id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var iSvc = new InventoryService();

                result.obj = iSvc.GetSupplierInvoices(Id, organisationId);

                result.Success = true;
                result.Message = "File processed";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("PaySupplier")]
        public async Task<IHttpActionResult> PaySupplier(PaySupplierViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var iSvc = new InventoryService();

                result = iSvc.PaySupplier(vm, organisationId);

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetCurrencies")]
        public async Task<IHttpActionResult> GetCurrencies()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var iSvc = new InventoryService();

                result = iSvc.GetCurrencies();

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        // functions
        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            //Convert Base64 string to byte[]
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
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

        public bool CheckPackageAccess(string feature, int orgnaisationId)
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
