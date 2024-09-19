using ExcelDataReader;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.SystemModels;
using SafriSoftv1._3.Models.ViewModels;
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
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result.obj = service.GetTrialBalanceAccounts(organisationId);

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

        [HttpGet, Route("ImportFromSafriSoft")]
        public IHttpActionResult ImportFromSafriSoft()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                //result.obj = aSvc.ImportFromSafriSoft(organisationId);

                result.Success = true;
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetBalanceSheetAccounts")]
        public IHttpActionResult GetBalanceSheetAccounts()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                result.obj = aSvc.GetBalanceSheetAccounts(organisationId);

                result.Success = true;
                return Json(result.obj);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetIncomeStatementAccounts")]
        public IHttpActionResult GetIncomeStatementAccounts()
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                result.obj = aSvc.GetIncomeStatementAccounts(organisationId);

                result.Success = true;
                return Json(result.obj);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetBalanceSheetAccount/{id}")]
        public IHttpActionResult GetBalanceSheetAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                result.obj = aSvc.GetBalanceSheetAccount(id, organisationId);

                result.Success = true;
                return Json(result.obj);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("GetIncomeStatementAccount/{id}")]
        public IHttpActionResult GetIncomeStatementAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                result.obj = aSvc.GetIncomeStatementAccount(id, organisationId);

                result.Success = true;
                return Json(result.obj);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveBalanceSheetAccount")]
        public IHttpActionResult SaveBalanceSheetAccount(ReportBalanceSheetDetailViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                if(vm.Id == 0)
                {
                    result = aSvc.SaveBalanceSheetAccount(vm, organisationId);
                }
                else
                {
                    result = aSvc.UpdateBalanceSheetAccount(vm, organisationId);
                }                

                result.Success = true;
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveIncomeStatementAccount")]
        public IHttpActionResult SaveIncomeStatementAccount(ReportIncomeStatementDetailViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var aSvc = new AccountingService();

                if(vm.Id == 0)
                {
                    result = aSvc.SaveIncomeStatementAccount(vm, organisationId);
                }
                else
                {
                    result = aSvc.UpdateIncomeStatementAccount(vm, organisationId);
                }
                

                result.Success = true;
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpGet, Route("MoveUpBalanceSheetAccount/{id}")]
        public IHttpActionResult MoveUpBalanceSheetAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveUpBalanceSheetAccount(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("MoveDownBalanceSheetAccount/{id}")]
        public IHttpActionResult MoveDownBalanceSheetAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveDownBalanceSheetAccount(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("MoveUpIncomeStatementAccount/{id}")]
        public IHttpActionResult MoveUpIncomeStatementAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveUpIncomeStatementAccount(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("MoveDownIncomeStatementAccount/{id}")]
        public IHttpActionResult MoveDownIncomeStatementAccount(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.MoveDownIncomeStatementAccount(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("GetUnlinkedBsAccounts/{id}")]
        public IHttpActionResult GetUnlinkedBsAccounts(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetUnlinkedBsAccounts(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("GetLinkedBsAccounts/{id}")]
        public IHttpActionResult GetLinkedBsAccounts(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetLinkedBsAccounts(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("GetUnlinkedIsAccounts/{id}")]
        public IHttpActionResult GetUnlinkedIsAccounts(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetUnlinkedIsAccounts(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("GetLinkedIsAccounts/{id}")]
        public IHttpActionResult GetLinkedIsAccounts(int id)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetLinkedIsAccounts(id, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("LinkBsAccount/{tbAccountId}/{bsAccountId}")]
        public IHttpActionResult LinkBsAccount(int tbAccountId, int bsAccountId)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.LinkBsAccount(bsAccountId, tbAccountId, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("LinkIsAccount/{tbAccountId}/{isAccountId}")]
        public IHttpActionResult LinkIsAccount(int tbAccountId, int isAccountId)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.LinkIsAccount(isAccountId, tbAccountId, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("UnlinkBsAccount/{tbAccountId}/{bsAccountId}")]
        public IHttpActionResult UnlinkBsAccount(int tbAccountId, int bsAccountId)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.UnlinkBsAccount(bsAccountId, tbAccountId, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpGet, Route("UnlinkIsAccount/{tbAccountId}/{isAccountId}")]
        public IHttpActionResult UnlinkIsAccount(int tbAccountId, int isAccountId)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.UnlinkIsAccount(isAccountId, tbAccountId, organisationId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpPost, Route("RunBalanceSheet")]
        public IHttpActionResult RunBalanceSheet(BalanceSheetViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                if(vm.Type == ReportViewType.Yearly)
                {
                    result.obj = service.RunBalanceSheetYearly(vm, organisationId);
                }
                else
                {
                    result.obj = service.RunBalanceSheetMonthly(vm, organisationId);
                }
                
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpPost, Route("RunIncomeStatement")]
        public IHttpActionResult RunIncomeStatement(IncomeStatementViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                if (vm.Type == ReportViewType.Yearly)
                {
                    result.obj = service.RunIncomeStatementYearly(vm, organisationId);
                }
                else
                {
                    result.obj = service.RunIncomeStatementMonthly(vm, organisationId);
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpPost, Route("GetDebtorsTransactions")]
        public IHttpActionResult GetDebtorsTransactions(ReportParametersViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetDebtorsTransactions(vm, organisationId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpPost, Route("GetCreditorsTransactions")]
        public IHttpActionResult GetCreditorsTransactions(ReportParametersViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetCreditorsTransactions(vm, organisationId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

        [HttpPost, Route("GetVatTransactions")]
        public IHttpActionResult GetVatTransactions(ReportParametersViewModel vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var service = new AccountingService();

                result = service.GetVatTransactions(vm, organisationId);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(new { Success = result.Success, Message = result.Message, obj = result.obj });
        }

    }


}
