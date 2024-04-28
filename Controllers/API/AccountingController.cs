using ExcelDataReader;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Accounting")]
    public class AccountingController : BaseApiController
    {
        [HttpGet, Route("GetTrialBalanceAccounts")]
        public IHttpActionResult GetTrialBalanceAccounts()
        {
            var result = new Result();

            try
            {
                var service = new AccountingService();

                result = service.GetTrialBalanceAccounts();

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("CreateUpdateTBAccount")]
        public IHttpActionResult CreateUpdateTBAccount(AccountingViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.CreateUpdateTBAccount(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("MoveUp")]
        public IHttpActionResult MoveUp(AccountingViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveUp(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("MoveDown")]
        public IHttpActionResult MoveDown(AccountingViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveDown(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpGet, Route("GetUnmappedGlAccounts")]
        public IHttpActionResult GetUnmappedGlAccounts()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetUnmappedGlAccounts(organisationId);
                result.Success = true;
                result.Message = "Successfully retrieved unmapped gl accounts";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("MapGeneralLedger")]
        public IHttpActionResult MapGeneralLedger(MapGeneralLedgerAccount vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MapGeneralLedgerAccount(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpGet, Route("GetMappedGlAccounts/{id}")]
        public IHttpActionResult GetMappedGlAccounts(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetMappedGlAccounts(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("UnmapGeneralLedger")]
        public IHttpActionResult UnmapGeneralLedger(MapGeneralLedgerAccount vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.UnmapGeneralLedgerAccount(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("GetGeneralLedgerAccounts")]
        public IHttpActionResult GetGeneralLedgerAccounts(DateParameters vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetGeneralLedgerAccounts(vm, organisationId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("CreateUpdateGlAccount")]
        public IHttpActionResult CreateUpdateGlAccount(GlAccountViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.CreateUpdateGlAccount(vm, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }

        [HttpPost, Route("UploadExcelData/{Id}")]
        public IHttpActionResult UploadExcelData(HttpRequestMessage request, string Id)
        {
            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            try
            {
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

                var service = new AccountingService();

                var tbItems = new List<AccountingViewModel>();

                while (excelReader.Read())
                {
                    string accountName = excelReader.GetString(0);

                    tbItems.Add(new AccountingViewModel()
                    {
                        AccountName = accountName
                    });

                    //excelReader.NextResult();
                }

                excelReader.Close();

                var results = new List<Result>();

                foreach (var item in tbItems)
                {
                    var result = service.CreateUpdateTBAccount(item, organisationId);

                    results.Add(result);
                }

                return Json(new { Success = results[results.Count() -1].Success, Message = results[results.Count() -1].Message, Obj = results });

            }
            catch (Exception ex)
            {
                var result = new Result();
                result.Success = false;
                result.Message = ex.Message;

                return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
            }
        }

        [HttpPost, Route("UploadGlExcelData/{Id}")]
        public IHttpActionResult UploadGlExcelData(HttpRequestMessage request, string Id)
        {
            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            try
            {
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

                var service = new AccountingService();

                var glItems = new List<GlAccountViewModel>();

                var results = new List<Result>();

                var result = new Result();

                var rowCounter = 1;

                while (excelReader.Read())
                {
                    if (excelReader.GetValue(0) == null)
                    {
                        result.Success = false;
                        result.Message = $"Date (first column) cannot be empty row: {rowCounter}";
                        results.Add(result);
                        continue;
                    }

                    if (excelReader.GetValue(1) == null)
                    {
                        result.Success = false;
                        result.Message = $"Account Number (Second column) cannot be empty row: {rowCounter}";
                        results.Add(result);
                        continue;
                    }

                    if (excelReader.GetValue(2) == null)
                    {
                        result.Success = false;
                        result.Message = $"Account Name (Third column) cannot be empty row: {rowCounter}";
                        results.Add(result);
                        continue;
                    }

                    if (excelReader.GetValue(4) == null)
                    {
                        result.Success = false;
                        result.Message = $"Debit (Fifth column) cannot be empty row: {rowCounter}";
                        results.Add(result);
                        continue;
                    }

                    if (excelReader.GetValue(5) == null)
                    {
                        result.Success = false;
                        result.Message = $"Credit (Sixth column) cannot be empty row: {rowCounter}";
                        results.Add(result);
                        continue;
                    }

                    string date = excelReader.GetValue(0).ToString();
                    string accounNumber = excelReader.GetValue(1).ToString();
                    string accountName = excelReader.GetValue(2).ToString();
                    string description = excelReader.GetValue(3) != null ? excelReader.GetValue(3).ToString() : "";
                    string debit = excelReader.GetValue(4).ToString();
                    string credit = excelReader.GetValue(5).ToString();

                    glItems.Add(new GlAccountViewModel()
                    {
                        AccountNumber = accounNumber,
                        AccountName = accountName,
                        Description = description,
                        Date = Convert.ToDateTime(date),
                        Debit = Convert.ToDouble(debit),
                        Credit = Convert.ToDouble(credit)
                    });

                    //excelReader.NextResult();
                }

                excelReader.Close();

                

                foreach (var item in glItems)
                {
                    var res = service.CreateUpdateGlAccount(item, organisationId);

                    results.Add(res);
                }

                return Json(new { Success = results[results.Count() - 1].Success, Message = results[results.Count() - 1].Message, Obj = results });

            }
            catch (Exception ex)
            {
                var result = new Result();
                result.Success = false;
                result.Message = ex.Message;

                return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
            }
        }

        [HttpPost, Route("GetTrialBalanceReport")]
        public IHttpActionResult GetTrialBalanceReport(DateParameters vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetTrialBalanceReport(vm, organisationId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, Obj = result.obj });
        }
    }


}
