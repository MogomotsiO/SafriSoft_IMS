﻿using Microsoft.Ajax.Utilities;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.SystemModels;
using SafriSoftv1._3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace SafriSoftv1._3.Services
{
    public class AccountingService : BaseService
    {
        public Result CreateUpdateTBAccount(AccountingViewModel vm, int organisationId)
        {
            var result = new Result();

            var validTb = db.TrialBalanceAccounts.Where(x => x.AccountName == vm.AccountName).FirstOrDefault();

            if(validTb != null)
            {
                result.Success = false;
                result.Message = $"We already have Trial balance account name {vm.AccountName}";
                return result;
            }

            //get the last record
            var existingTb = db.TrialBalanceAccounts.OrderByDescending(x => x.Id).FirstOrDefault();

            // default start
            long accountNumber = 100;

            if (existingTb != null)
                accountNumber = Convert.ToInt64(existingTb.AccountNumber) + 1;

            var tb = new TrialBalanceAccount()
            {
                Id = existingTb != null ? existingTb.Id : 0, // new or existing
                AccountNumber = accountNumber.ToString(),
                AccountName = vm.AccountName,
                Index = existingTb != null ? existingTb.Index + 1 : 1,
                OrganisationId = organisationId,
                Updated = DateTime.Now,
                Inserted = DateTime.Now
            };

            db.TrialBalanceAccounts.Add(tb);
            var id = db.SaveChanges();

            if (id > 0) //created
            {
                result.Success = true;
                result.Message = $"Successfully Saved Trial Balance Account {vm.AccountName}";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save Tb";
            }

            return result;
        } 

        public Result GetTrialBalanceAccounts()
        {
            var result = new Result();

            var items = db.TrialBalanceAccounts.Where(x => x.IsGlobal == true).ToList().OrderBy(x => x.Index);

            if(items.Count() > 0)
            {
                result.Success = true;
                result.Message = "Successfully retrieved Accounts";
                result.obj = items;
            }
            else
            {
                result.Success = false;
                result.obj = new List<TrialBalanceAccount>();
            }

            return result;
        }

        public Result MoveUp(AccountingViewModel vm, int organisationId)
        {
            var result = new Result();

            if (vm.Id != 0)
            {
                var item = db.TrialBalanceAccounts.Where(x => x.Id == vm.Id && x.OrganisationId == organisationId).FirstOrDefault();
                var prevItem = db.TrialBalanceAccounts.Where(x => x.Index == item.Index - 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if (prevItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account up";
                    return result;
                }

                var currIndex = 0;
                var prevIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (prevItem != null)
                    prevIndex = prevItem.Index;

                item.Index = prevIndex;
                prevItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result MoveDown(AccountingViewModel vm, int organisationId)
        {
            var result = new Result();

            if (vm.Id != 0)
            {
                var item = db.TrialBalanceAccounts.Where(x => x.Id == vm.Id && x.OrganisationId == organisationId).FirstOrDefault();
                var nextItem = db.TrialBalanceAccounts.Where(x => x.Index == item.Index + 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if(nextItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account down";
                    return result;
                }

                var currIndex = 0;
                var nextItemIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (nextItem != null)
                    nextItemIndex = nextItem.Index;

                item.Index = nextItemIndex;
                nextItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result GetUnmappedGlAccounts(int organisationId)
        {
            var result = new Result();

            var mappedGls = db.TrialBalanceGeneralLedgerMappings.Where(x => x.OrganisationId == organisationId).Select(x => x.GeneralLedgerId).ToList();
            //var unmappedGls = db.GeneralLedgers.Where(x => x.OrganisationId == organisationId).GroupBy(x => new { x.AccountNumber, x.AccountName }).Select(x => x.FirstOrDefault()).ToList();
            var unmappedGls = db.GeneralLedgers.Where(x => x.OrganisationId == organisationId).ToList();

            var unmappedGlAccounts = new List<UnmappedGlAccount>();

            foreach (var gl in unmappedGls)
            {
                if (mappedGls.Contains(gl.Id) == false)
                {
                    unmappedGlAccounts.Add(new UnmappedGlAccount()
                    {
                        Id = gl.Id,
                        AccountNumber = gl.AccountNumber,
                        AccountName = gl.AccountName,
                        DateStr = gl.DateStr,
                        Credit = gl.Credit,
                        Debit = gl.Debit

                    });
                }
            }

            result.obj = unmappedGlAccounts;

            return result;
        }

        public Result GetMappedGlAccounts(int id, int organisationId)
        {
            var result = new Result();

            var items = new List<MappedGlAccounts>();

            var account = db.TrialBalanceAccounts.Where(x => x.Id == id).FirstOrDefault();

            if(account != null)
            {
                var gls = db.GeneralLedgers.Where(x => x.AccountNumber == account.AccountNumber && x.OrganisationId == organisationId).ToList();

                foreach(var item in gls)
                {
                    items.Add(new MappedGlAccounts()
                    {
                        Id = item.Id,
                        AccountNumber = item.AccountNumber,
                        AccountName = item.AccountName,
                        AccountDescription = item.AccountDescription
                    });
                }

                var jnls = db.Journals.Where(x => x.OrganisationId == organisationId && x.IsActive == true).ToList();

                foreach (var jnl in jnls)
                {
                    var entries = db.JournalEntries.Where(x => x.JournalId == jnl.Id && x.AccountId == account.Id).ToList();

                    foreach (var entry in entries)
                    {
                        items.Add(new MappedGlAccounts()
                        {
                            Id = jnl.Id,
                            AccountNumber = account.AccountNumber,
                            AccountName = account.AccountName,
                            AccountDescription = $"{jnl.Number} - {account.AccountName}"
                        });
                    }
                    
                }
            }

            //var mappedGlItems = db.TrialBalanceGeneralLedgerMappings.Where(x => x.TrialBalanceAccountId == id && x.OrganisationId == organisationId).ToList();

            //var items = new List<MappedGlAccounts>();

            //foreach (var mappedItem in mappedGlItems)
            //{
            //    var item = db.GeneralLedgers.Where(x => x.Id == mappedItem.GeneralLedgerId).FirstOrDefault();

            //    if(item != null)
            //    {
            //        items.Add(new MappedGlAccounts()
            //        {
            //            Id = item.Id,
            //            AccountNumber = item.AccountNumber,
            //            AccountName = item.AccountName
            //        });
            //    }
            //}

            result.Success = true;
            result.obj = items;

            return result;
        }

        public Result MapGeneralLedgerAccount(MapGeneralLedgerAccount vm, int organisationId)
        {
            var result = new Result();

            var mapping = new TrialBalanceGeneralLedgerMapping()
            {
                TrialBalanceAccountId = vm.TrialBalanceAccountId,
                GeneralLedgerId = vm.GeneralLedgerId,
                OrganisationId = organisationId
            };

            db.TrialBalanceGeneralLedgerMappings.Add(mapping);
            var id = db.SaveChanges();

            if(id > 0)
            {
                result.Success = true;
                result.Message = "Accounts have been mapped";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not map accounts";
            }

            return result;
        }

        public Result UnmapGeneralLedgerAccount(MapGeneralLedgerAccount vm, int organisationId)
        {
            var result = new Result();

            var item = db.TrialBalanceGeneralLedgerMappings.Where(x => x.TrialBalanceAccountId == vm.TrialBalanceAccountId && x.GeneralLedgerId == vm.GeneralLedgerId && x.OrganisationId == organisationId).FirstOrDefault();

            db.TrialBalanceGeneralLedgerMappings.Remove(item);

            db.SaveChanges();

            result.Success = true;
            result.Message = "Accounts have been unmapped";

            return result;
        }

        public Result CreateUpdateGlAccount(GlAccountViewModel vm, int organisationId)
        {
            var result = new Result();
            var id = 0;

            try
            {
                var gl = new GeneralLedger();

                gl.Id = id;
                gl.AccountNumber = vm.AccountNumber;
                gl.AccountName = vm.AccountName;
                gl.AccountDescription = vm.Description;
                gl.AccountDate = vm.Date;
                gl.Month = vm.Date.Month;
                gl.Year = vm.Date.Year;
                gl.Debit = vm.Debit;
                gl.Credit = vm.Credit;
                gl.OrganisationId = organisationId;

                var newId = db.GeneralLedgers.Add(gl);
                var res = db.SaveChanges();

                if(vm.AccountReference != null)
                {
                    gl.AccountReference = vm.AccountReference.Replace("(ID)", newId.Id.ToString());
                }

                db.SaveChanges();

                if (newId.Id > 0) //created
                {
                    result.Success = true;
                    result.Message = $"Successfully Saved General Ledger Account {vm.AccountName}";
                }
                else
                {
                    result.Success = false;
                    result.Message = $"Could not save Gl {vm.AccountName}";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Could not save Gl {vm.AccountName} - {ex.Message}";
                return result;
            }            
        }

        public Result GetGeneralLedgerAccounts(DateParameters vm, int organisationId)
        {
            var result = new Result();

            var items = db.GeneralLedgers.Where(x => x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).OrderBy(x => x.AccountDate).ThenBy(x => x.AccountNumber).ToList();

            var jeGls = GetJournalEntriesAsGls(vm, organisationId);

            items.AddRange(jeGls);

            if (items.Count() > 0)
            {
                result.Success = true;
                result.Message = "Successfully retrieved Accounts";
                result.obj = items;
            }
            else
            {
                result.Success = false;
                result.Message = "No data for period";
                result.obj = new List<GeneralLedger>();
            }

            return result;
        }

        public Result GetTrialBalanceReport(DateParameters vm, int organisationId)
        {
            var result = new Result();

            var glAccountsForPeriod = db.GeneralLedgers.Where(x => x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).ToList();

            var jeGls = GetJournalEntriesAsGls(vm, organisationId);

            if (glAccountsForPeriod.Count() == 0 && jeGls.Count() == 0)
            {
                result.Success = false;
                result.Message = "No data for period";
                result.obj = new List<TrialBalanceContainer>();
                return result;
            }

            var trialBalanceItems = db.TrialBalanceAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).ToList();

            var items = new List<TrialBalanceContainer>();

            var trialBalanceTotal = 0.00;

            foreach (var tbItem in trialBalanceItems)
            {

                var gls = db.GeneralLedgers.Where(x => x.AccountNumber == tbItem.AccountNumber && x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).ToList();
                
                gls.AddRange(jeGls.Where(x => x.AccountNumber == tbItem.AccountNumber).ToList());

                var total = 0.00;

                foreach (var glAccount in gls)
                {
                    total += glAccount.Debit;
                    total += (glAccount.Credit * -1);
                }

                trialBalanceTotal += total;

                items.Add(new TrialBalanceContainer()
                {
                    AccountNumber = tbItem.AccountNumber,
                    AccountName = tbItem.AccountName,
                    Index = tbItem.Index,
                    Period = $"{vm.Start:dd MMMM yyyy} - {vm.End:dd MMMM yyyy}",
                    Total = Math.Round(total, 2),
                    TrialBalanceItems = gls
                });

            }

            items.Add(new TrialBalanceContainer()
            {
                AccountName = "Balance",
                AccountNumber = string.Empty,
                Total = trialBalanceTotal,
                Index = 1000000
            });

            result.Success = true;
            result.obj = items.OrderBy(x => x.Index);

            return result;
        }

        public Result ImportFromSafriSoft(DateParameters vm, int organisationId)
        {
            var result = new Result();

            var items = new List<AccountingViewModel>();

            // supplier
            var supplierTotal = db.SupplierInvoices.Where(x => x.OrganisationId == organisationId && x.Inserted >= vm.Start && x.Inserted <= vm.End).Select(x => x.Amount).Sum();
            // supplier VAT
            var supplierTotalVat = db.SupplierInvoices.Where(x => x.OrganisationId == organisationId && x.Inserted >= vm.Start && x.Inserted <= vm.End).Select(x => x.VatAmount).Sum();
            // invoices
            var invoicesTotal = db.Invoices.Where(x => x.OrganisationId == organisationId && x.InvoiceDate >= vm.Start && x.InvoiceDate <= vm.End && x.Paid == true).Select(x => x.Amount).Sum();
            // invoices VAT
            var invoices = db.Invoices.Where(x => x.OrganisationId == organisationId && x.InvoiceDate >= vm.Start && x.InvoiceDate <= vm.End && x.Paid == true).ToList();

            var invoicesVateAmount = 0;

            foreach(var inVat in invoices)
            {

            }

            return result;
        }

        public Result SaveCustomerTransaction(CustomerTransaction ct)
        {
            var result = new Result();

            ct.Date = DateTime.Now;

            db.CustomerTransactions.Add(ct);
            var res = db.SaveChanges();

            if(res > 0)
            {
                result.Success = true;
                result.Message = "Customer transaction saved";
            }
            else
            {
                result.Success = false;
                result.Message = "Customer transaction failed to save";
            }

            return result;
        }

        public List<GeneralLedger> GetJournalEntriesAsGls(DateParameters vm, int organisationId)
        {
            var jeGls = new List<GeneralLedger>();

            var journals = db.Journals.Where(x => x.Date >= vm.Start && x.Date <= vm.End && x.OrganisationId == organisationId && x.IsActive == true).ToList();

            foreach(var journal in journals)
            {
                var entries = db.JournalEntries.Where(x => x.JournalId == journal.Id).ToList();

                foreach(var entry in entries)
                {
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == entry.AccountId).FirstOrDefault();

                    var jeGl = new GeneralLedger()
                    {
                        AccountReference = $"REF{journal.Number}",
                        AccountNumber = account.AccountNumber,
                        AccountName = $"{journal.Number} - {account.AccountName}",
                        AccountDescription = $"{journal.Number} - {entry.Narration}",
                        AccountDate = journal.Date,
                        Month = journal.Date.Month,
                        Year = journal.Date.Year,
                        Debit = entry.Debit,
                        Credit = entry.Credit,
                        OrganisationId = organisationId,
                    };

                    jeGls.Add(jeGl);
                }                
            }

            return jeGls;
        }

        public List<GeneralLedger> GetJournalEntriesAsGlsByYear(int year, int organisationId)
        {
            var jeGls = new List<GeneralLedger>();

            var journals = db.Journals.Where(x => x.Date.Year == year && x.OrganisationId == organisationId && x.IsActive == true).ToList();

            foreach (var journal in journals)
            {
                var entries = db.JournalEntries.Where(x => x.JournalId == journal.Id).ToList();

                foreach (var entry in entries)
                {
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == entry.AccountId).FirstOrDefault();

                    var jeGl = new GeneralLedger()
                    {
                        AccountNumber = account.AccountNumber,
                        AccountName = $"{journal.Number} - {account.AccountNumber} - {account.AccountName}",
                        AccountDescription = $"{journal.Number} - {entry.Narration}",
                        AccountDate = journal.Date,
                        Month = journal.Date.Month,
                        Year = journal.Date.Year,
                        Debit = entry.Debit,
                        Credit = entry.Credit,
                        OrganisationId = organisationId,
                    };

                    jeGls.Add(jeGl);
                }
            }

            return jeGls;
        }

        public List<GeneralLedger> GetJournalEntriesAsGlsByMonth(int year, int month, int organisationId)
        {
            var jeGls = new List<GeneralLedger>();

            var journals = db.Journals.Where(x => x.Date.Year == year && x.Date.Month == month && x.OrganisationId == organisationId && x.IsActive == true).ToList();

            foreach (var journal in journals)
            {
                var entries = db.JournalEntries.Where(x => x.JournalId == journal.Id).ToList();

                foreach (var entry in entries)
                {
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == entry.AccountId).FirstOrDefault();

                    var jeGl = new GeneralLedger()
                    {
                        AccountNumber = account.AccountNumber,
                        AccountName = $"{journal.Number} - {account.AccountNumber} - {account.AccountName}",
                        AccountDescription = $"{journal.Number} - {entry.Narration}",
                        AccountDate = journal.Date,
                        Month = journal.Date.Month,
                        Year = journal.Date.Year,
                        Debit = entry.Debit,
                        Credit = entry.Credit,
                        OrganisationId = organisationId,
                    };

                    jeGls.Add(jeGl);
                }
            }

            return jeGls;
        }

        public Result GetBalanceSheetAccounts(int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportBalanceSheetDetailViewModel>();

            var items = db.ReportBalanceSheetAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).ToList();

            foreach(var item in items)
            {
                var heading = db.ReportBalanceSheetAccounts.Where(x => x.Id == item.HeadingAccountId).FirstOrDefault();
                var subtotal = db.ReportBalanceSheetAccounts.Where(x => x.Id == item.SubtotalAccountId).FirstOrDefault();

                vm.Add(new ReportBalanceSheetDetailViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    HeadingAccountName = heading?.Name,
                    SubtotalAccountName = subtotal?.Name,
                    IsHeading = item.IsHeading,
                    IsSubtotal = item.IsSubtotal,
                    IsEmptySpace = item.IsEmptySpace,
                    Index = item.Index,
                });
            }

            result.obj = vm.OrderBy(x => x.Index);

            return result;
        }

        public Result GetIncomeStatementAccounts(int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportIncomeStatementDetailViewModel>();

            var items = db.ReportIncomeStatementAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).ToList();

            foreach (var item in items)
            {
                var heading = db.ReportIncomeStatementAccounts.Where(x => x.Id == item.HeadingAccountId).FirstOrDefault();
                var subtotal = db.ReportIncomeStatementAccounts.Where(x => x.Id == item.SubtotalAccountId).FirstOrDefault();

                vm.Add(new ReportIncomeStatementDetailViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    HeadingAccountName = heading?.Name,
                    SubtotalAccountName = subtotal?.Name,
                    OperatorTypeName = item.OperatorType != 0 && (int)item.OperatorType != -100 ? item.OperatorType.ToString() : string.Empty,
                    IsHeading = item.IsHeading,
                    IsSubtotal = item.IsSubtotal,
                    Index = item.Index,
                });
            }

            result.obj = vm.OrderBy(x => x.Index);

            return result;
        }

        public Result SaveBalanceSheetAccount(ReportBalanceSheetDetailViewModel vm, int organisationId)
        {
            var result = new Result();

            var lastItem = db.ReportBalanceSheetAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).OrderByDescending(x => x.Index).FirstOrDefault();

            var item = new ReportBalanceSheetAccount()
            {
                Name = vm.Name,
                HeadingAccountId = vm.HeadingAccountId,
                SubtotalAccountId = vm.SubtotalAccountId,
                IsHeading = vm.IsHeading,
                IsSubtotal = vm.IsSubtotal,
                IsEmptySpace = vm.IsEmptySpace,
                Index = lastItem != null ? lastItem.Index + 1 : 1,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                OrganisationId = organisationId
            };

            var res = db.ReportBalanceSheetAccounts.Add(item);

            var saveRes = db.SaveChanges();

            if(saveRes > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved account";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save account";
            }

            return result;
        }

        public Result SaveIncomeStatementAccount(ReportIncomeStatementDetailViewModel vm, int organisationId)
        {
            var result = new Result();

            var lastItem = db.ReportIncomeStatementAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).OrderByDescending(x => x.Index).FirstOrDefault();

            var item = new ReportIncomeStatementAccount()
            {
                Name = vm.Name,
                HeadingAccountId = vm.HeadingAccountId,
                SubtotalAccountId = vm.SubtotalAccountId,
                OperatorType = vm.OperatorType,
                IsHeading = vm.IsHeading,
                IsSubtotal = vm.IsSubtotal,
                Index = lastItem != null ? lastItem.Index + 1 : 1,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                IsEmptySpace = vm.IsEmptySpace,
                OrganisationId = organisationId
            };

            var res = db.ReportIncomeStatementAccounts.Add(item);

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved account";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save account";
            }

            return result;
        }

        public Result UpdateBalanceSheetAccount(ReportBalanceSheetDetailViewModel vm, int organisationId)
        {
            var result = new Result();

            var item = db.ReportBalanceSheetAccounts.Where(x => x.Id == vm.Id).FirstOrDefault();

            item.Name = vm.Name;
            item.HeadingAccountId = vm.HeadingAccountId;
            item.SubtotalAccountId = vm.SubtotalAccountId;
            item.IsHeading = vm.IsHeading;
            item.IsSubtotal = vm.IsSubtotal;
            item.IsEmptySpace = vm.IsEmptySpace;
            item.Updated = DateTime.Now;

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved account";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save account";
            }

            return result;
        }

        public Result UpdateIncomeStatementAccount(ReportIncomeStatementDetailViewModel vm, int organisationId)
        {
            var result = new Result();

            var item = db.ReportIncomeStatementAccounts.Where(x => x.Id == vm.Id).FirstOrDefault();

            item.Name = vm.Name;
            item.HeadingAccountId = vm.HeadingAccountId;
            item.SubtotalAccountId = vm.SubtotalAccountId;
            item.OperatorType = vm.OperatorType;
            item.IsHeading = vm.IsHeading;
            item.IsSubtotal = vm.IsSubtotal;
            item.IsEmptySpace = vm.IsEmptySpace;
            item.Updated = DateTime.Now;

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved account";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save account";
            }

            return result;
        }

        public List<VatOption> GetVatOptions(int organisationId)
        {
            return db.VatOptions.Where(x => x.OrganisationId == organisationId).ToList();
        }

        public List<TrialBalanceAccount> GetTrialBalanceAccounts(int organisationId)
        {
            return db.TrialBalanceAccounts.Where(x => x.OrganisationId == organisationId || x.IsGlobal == true).OrderBy(x => x.Index).ToList();
        }

        public ReportBalanceSheetViewModel GetBalanceSheetAccountList(int organisationId)
        {
            var vm = new ReportBalanceSheetViewModel();

            vm.HeadingAccounts = db.ReportBalanceSheetAccounts.Where(x => (x.OrganisationId == organisationId || x.IsGlobal) && x.IsHeading).ToList();
            vm.SubtotalAccounts = db.ReportBalanceSheetAccounts.Where(x => (x.OrganisationId == organisationId || x.IsGlobal) && x.IsSubtotal).ToList();

            return vm;
        }

        public ReportIncomeStatementViewModel GetIncomeStatementAccountList(int organisationId)
        {
            var vm = new ReportIncomeStatementViewModel();

            vm.HeadingAccounts = db.ReportIncomeStatementAccounts.Where(x => (x.OrganisationId == organisationId || x.IsGlobal == true) && x.IsHeading).ToList();
            vm.SubtotalAccounts = db.ReportIncomeStatementAccounts.Where(x => (x.OrganisationId == organisationId || x.IsGlobal == true) && x.IsSubtotal).ToList();

            return vm;
        }

        public Result GetBalanceSheetAccount(int id, int organisationId)
        {
            var result = new Result();

            var item = db.ReportBalanceSheetAccounts.Where(x => x.Id == id).FirstOrDefault();

            result.obj = item;

            return result;
        }

        public Result GetIncomeStatementAccount(int id, int organisationId)
        {
            var result = new Result();

            var item = db.ReportIncomeStatementAccounts.Where(x => x.Id == id).FirstOrDefault();

            result.obj = item;

            return result;
        }

        public Result MoveUpBalanceSheetAccount(int id, int organisationId)
        {
            var result = new Result();

            if (id != 0)
            {
                var item = db.ReportBalanceSheetAccounts.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();
                var prevItem = db.ReportBalanceSheetAccounts.Where(x => x.Index == item.Index - 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if (prevItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account up";
                    return result;
                }

                var currIndex = 0;
                var prevIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (prevItem != null)
                    prevIndex = prevItem.Index;

                item.Index = prevIndex;
                prevItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result MoveDownBalanceSheetAccount(int id, int organisationId)
        {
            var result = new Result();

            if (id != 0)
            {
                var item = db.ReportBalanceSheetAccounts.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();
                var nextItem = db.ReportBalanceSheetAccounts.Where(x => x.Index == item.Index + 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if (nextItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account down";
                    return result;
                }

                var currIndex = 0;
                var nextItemIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (nextItem != null)
                    nextItemIndex = nextItem.Index;

                item.Index = nextItemIndex;
                nextItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result MoveUpIncomeStatementAccount(int id, int organisationId)
        {
            var result = new Result();

            if (id != 0)
            {
                var item = db.ReportIncomeStatementAccounts.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();
                var prevItem = db.ReportIncomeStatementAccounts.Where(x => x.Index == item.Index - 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if (prevItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account up";
                    return result;
                }

                var currIndex = 0;
                var prevIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (prevItem != null)
                    prevIndex = prevItem.Index;

                item.Index = prevIndex;
                prevItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result MoveDownIncomeStatementAccount(int id, int organisationId)
        {
            var result = new Result();

            if (id != 0)
            {
                var item = db.ReportIncomeStatementAccounts.Where(x => x.Id == id && x.OrganisationId == organisationId).FirstOrDefault();
                var nextItem = db.ReportIncomeStatementAccounts.Where(x => x.Index == item.Index + 1 && x.OrganisationId == organisationId).FirstOrDefault();

                if (nextItem == null)
                {
                    result.Success = false;
                    result.Message = "Cannot move account down";
                    return result;
                }

                var currIndex = 0;
                var nextItemIndex = 0;

                if (item != null)
                    currIndex = item.Index;

                if (nextItem != null)
                    nextItemIndex = nextItem.Index;

                item.Index = nextItemIndex;
                nextItem.Index = currIndex;

                db.SaveChanges();
            }

            result.Success = true;

            return result;
        }

        public Result LinkBsAccount(int bsAccountId, int tbAccountId, int organisationId)
        {
            var result = new Result();

            var link = new ReportBalanceSheetAccountLink()
            {
                BsAccountId = bsAccountId,
                TbAccountId = tbAccountId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                OrganisationId = organisationId
            };

            db.ReportBalanceSheetAccountLinks.Add(link);

            var saveRes = db.SaveChanges();

            if(saveRes > 0)
            {
                result.Success = true;
                result.Message = "Saved link";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save link";
            }            

            return result;
        }

        public Result LinkIsAccount(int isAccountId, int tbAccountId, int organisationId)
        {
            var result = new Result();

            var link = new ReportIncomeStatementAccountLink()
            {
                IsAccountId = isAccountId,
                TbAccountId = tbAccountId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                OrganisationId = organisationId
            };

            db.ReportIncomeStatementAccountLinks.Add(link);

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Saved link";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save link";
            }

            return result;
        }

        public Result UnlinkBsAccount(int bsAccountId, int tbAccountId, int organisationId)
        {
            var result = new Result();

            var item = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == bsAccountId && x.TbAccountId == tbAccountId).FirstOrDefault();

            db.ReportBalanceSheetAccountLinks.Remove(item);

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Saved link";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save link";
            }

            return result;
        }

        public Result UnlinkIsAccount(int isAccountId, int tbAccountId, int organisationId)
        {
            var result = new Result();

            var item = db.ReportIncomeStatementAccountLinks.Where(x => x.IsAccountId == isAccountId && x.TbAccountId == tbAccountId).FirstOrDefault();

            db.ReportIncomeStatementAccountLinks.Remove(item);

            var saveRes = db.SaveChanges();

            if (saveRes > 0)
            {
                result.Success = true;
                result.Message = "Saved link";
            }
            else
            {
                result.Success = false;
                result.Message = "Could not save link";
            }

            return result;
        }

        public Result GetLinkedBsAccounts(int bsAccountId, int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportBalanceSheetLinkedAccounts>();

            var links = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == bsAccountId && x.OrganisationId == organisationId).ToList();

            foreach (var link in links)
            {
                var tbItem = db.TrialBalanceAccounts.Where(x => x.Id == link.TbAccountId).FirstOrDefault();

                vm.Add(new ReportBalanceSheetLinkedAccounts()
                {
                    Id = tbItem.Id,
                    Name = $"{tbItem.AccountNumber} - {tbItem.AccountName}"
                });
            }

            result.obj = vm;

            return result;
        }

        public Result GetUnlinkedBsAccounts(int bsAccountId, int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportBalanceSheetLinkedAccounts>();

            var tbAccounts = GetTrialBalanceAccounts(organisationId);

            foreach (var tbItem in tbAccounts)
            {
                var link = db.ReportBalanceSheetAccountLinks.Where(x => x.TbAccountId == tbItem.Id && x.OrganisationId == organisationId).FirstOrDefault();

                if(link == null)
                {
                    vm.Add(new ReportBalanceSheetLinkedAccounts()
                    {
                        Id = tbItem.Id,
                        Name = $"{tbItem.AccountNumber} - {tbItem.AccountName}"
                    });
                }                
            }

            result.obj = vm;

            return result;
        }

        public Result GetLinkedIsAccounts(int bsAccountId, int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportIncomeStatementLinkedAccounts>();

            var links = db.ReportIncomeStatementAccountLinks.Where(x => x.IsAccountId == bsAccountId && x.OrganisationId == organisationId).ToList();

            foreach (var link in links)
            {
                var tbItem = db.TrialBalanceAccounts.Where(x => x.Id == link.TbAccountId).FirstOrDefault();

                vm.Add(new ReportIncomeStatementLinkedAccounts()
                {
                    Id = tbItem.Id,
                    Name = $"{tbItem.AccountNumber} - {tbItem.AccountName}"
                });
            }

            result.obj = vm;

            return result;
        }

        public Result GetUnlinkedIsAccounts(int bsAccountId, int organisationId)
        {
            var result = new Result();

            var vm = new List<ReportIncomeStatementLinkedAccounts>();

            var tbAccounts = GetTrialBalanceAccounts(organisationId);

            foreach (var tbItem in tbAccounts)
            {
                var link = db.ReportIncomeStatementAccountLinks.Where(x => x.TbAccountId == tbItem.Id && x.OrganisationId == organisationId).FirstOrDefault();

                if (link == null)
                {
                    vm.Add(new ReportIncomeStatementLinkedAccounts()
                    {
                        Id = tbItem.Id,
                        Name = $"{tbItem.AccountNumber} - {tbItem.AccountName}"
                    });
                }
            }

            result.obj = vm;

            return result;
        }

        public BalanceSheetContainer RunBalanceSheetYearly(BalanceSheetViewModel vm, int organisation)
        {
            var container = new BalanceSheetContainer();

            var data = new List<BalanceSheet>();

            var startYear = vm.Start.Year;
            var endYear = vm.End.Year;
            var startDate = vm.Start;
            var endDate = vm.End;
            var sDate = vm.Start.Year;
            var eDate = vm.End.Year;

            var counter = 1;

            var accounts = db.ReportBalanceSheetAccounts.Where(x => x.OrganisationId == organisation || x.IsGlobal).OrderBy(x => x.Index).ToList();

            foreach(var account in accounts)
            {
                data.Add(new BalanceSheet()
                {
                    Id = account.Id,
                    Name = account.Name,
                    IsHeading = account.IsHeading,
                    IsSubtotal = account.IsSubtotal,
                    IsEmptySpace = account.IsEmptySpace,
                    SubtotalAccountId = account.SubtotalAccountId,
                    
                });
            }

            var accountsRequireBalance = data.Where(x => x.IsHeading == false && x.IsSubtotal == false).ToList();

            var subtotals = data.Where(x => x.IsSubtotal).ToList();

            var dummyBalances = new Dictionary<string, double>();
            var newStartDate = vm.Start;
            while (startYear <= endYear)
            {
                container.Coloumns.Add(startYear.ToString());

                dummyBalances.Add(startYear.ToString(), 0);

                var innerLoopStartDate = new DateTime(startYear, startDate.Month, 1); ;
                var innerLoopEndDate = endDate;

                if(startYear < endYear)
                {
                    innerLoopEndDate = new DateTime(startYear, 12, 1);
                }

                foreach (var account in accountsRequireBalance)
                {
                    var links = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == account.Id).ToList();

                    var balance = 0.0;
                                        
                    foreach (var link in links)
                    {
                        var tempStartDate = innerLoopStartDate;
                        var tempEndDate = innerLoopEndDate;

                        while(tempStartDate <= tempEndDate)
                        {
                            balance += GetTrialBalanceAccountBalanceByMonth(tempStartDate.Year, tempStartDate.Month, link.TbAccountId, organisation);
                            tempStartDate = tempStartDate.AddMonths(1);
                        }
                        
                    }

                    account.Balances.Add(startYear.ToString(), balance);
                }
                
                foreach (var subTot in subtotals)
                {
                    var subTotAccounts = data.Where(x => x.SubtotalAccountId == subTot.Id).SelectMany(x => x.Balances).ToList();

                    var yearBalances = subTotAccounts.Where(x => x.Key.Contains(startYear.ToString())).Select(x => x.Value);

                    subTot.Balances.Add(startYear.ToString(), yearBalances.Sum());
                }

                startYear += 1;
                counter++;
            }

            foreach(var acc in data.Where(x => x.IsHeading))
            {
                acc.Balances = dummyBalances;
            }

            container.Items = data;

            return container;
        }

        public BalanceSheetContainer RunBalanceSheetMonthly(BalanceSheetViewModel vm, int organisation)
        {
            var container = new BalanceSheetContainer();

            var data = new List<BalanceSheet>();

            var start = new DateTime(vm.Start.Year, vm.Start.Month, 1);
            var end = new DateTime(vm.End.Year, vm.End.Month, 1);

            var counter = 1;

            var accounts = db.ReportBalanceSheetAccounts.Where(x => x.OrganisationId == organisation || x.IsGlobal).OrderBy(x => x.Index).ToList();

            foreach (var account in accounts)
            {
                data.Add(new BalanceSheet()
                {
                    Id = account.Id,
                    Name = account.Name,
                    IsHeading = account.IsHeading,
                    IsSubtotal = account.IsSubtotal,
                    IsEmptySpace = account.IsEmptySpace,
                    SubtotalAccountId = account.SubtotalAccountId
                });
            }

            var accountsRequireBalance = data.Where(x => x.IsHeading == false && x.IsSubtotal == false).ToList();

            var subtotals = data.Where(x => x.IsSubtotal).ToList();

            var dummyBalances = new Dictionary<string, double>();

            while (start <= end)
            {
                container.Coloumns.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString());

                dummyBalances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), 0);

                foreach (var account in accountsRequireBalance)
                {
                    var links = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == account.Id).ToList();

                    var balance = 0.0;

                    foreach (var link in links)
                    {
                        balance += GetTrialBalanceAccountBalanceByMonth(start.Year, start.Month, link.TbAccountId, organisation);
                    }

                    account.Balances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), balance);
                }

                foreach (var subTot in subtotals)
                {
                    var subTotAccounts = data.Where(x => x.SubtotalAccountId == subTot.Id).SelectMany(x => x.Balances).ToList();

                    var yearBalances = subTotAccounts.Where(x => x.Key.Contains(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString())).Select(x => x.Value);

                    var balanceSum = yearBalances.Sum();

                    subTot.Balances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), balanceSum);
                }

                start = start.AddMonths(1);
                counter++;
            }

            foreach (var acc in data.Where(x => x.IsHeading))
            {
                acc.Balances = dummyBalances;
            }

            container.Items = data;

            return container;
        }

        public IncomeStatementContainer RunIncomeStatementYearly(IncomeStatementViewModel vm, int organisation)
        {
            var container = new IncomeStatementContainer();

            var data = new List<IncomeStatement>();

            var startYear = vm.Start.Year;
            var endYear = vm.End.Year;
            var startDate = vm.Start;
            var endDate = vm.End;
            var sDate = vm.Start.Year;
            var eDate = vm.End.Year;

            var counter = 1;

            var accounts = db.ReportIncomeStatementAccounts.Where(x => x.OrganisationId == organisation || x.IsGlobal).OrderBy(x => x.Index).ToList();

            foreach (var account in accounts)
            {
                data.Add(new IncomeStatement()
                {
                    Id = account.Id,
                    Name = account.Name,
                    IsHeading = account.IsHeading,
                    IsSubtotal = account.IsSubtotal,
                    IsEmptySpace = account.IsEmptySpace,
                    SubtotalAccountId = account.SubtotalAccountId,
                    OperatorType = account.OperatorType
                });
            }

            var accountsRequireBalance = data.Where(x => x.IsHeading == false && x.IsSubtotal == false).ToList();

            var subtotals = data.Where(x => x.IsSubtotal).ToList();

            var viewDummyBalances = new Dictionary<string, double>();

            var dummyBalances = new List<Balance>();

            var newStartDate = vm.Start;
            while (startYear <= endYear)
            {
                container.Coloumns.Add(startYear.ToString());

                viewDummyBalances.Add(startYear.ToString(), 0);

                dummyBalances.Add(new Balance() { Year = startYear.ToString(), OperatoryType = MathOperatorType.Add, Amount = 0 });

                var innerLoopStartDate = new DateTime(startYear, startDate.Month, 1); ;
                var innerLoopEndDate = endDate;

                if (startYear < endYear)
                {
                    innerLoopEndDate = new DateTime(startYear, 12, 1);
                }

                foreach (var account in accountsRequireBalance)
                {
                    var links = db.ReportIncomeStatementAccountLinks.Where(x => x.IsAccountId == account.Id).ToList();

                    var balance = 0.0;

                    foreach (var link in links)
                    {
                        var tempStartDate = innerLoopStartDate;
                        var tempEndDate = innerLoopEndDate;

                        while (tempStartDate <= tempEndDate)
                        {
                            balance += GetTrialBalanceAccountBalanceByMonth(tempStartDate.Year, tempStartDate.Month, link.TbAccountId, organisation);
                            tempStartDate = tempStartDate.AddMonths(1);
                        }

                    }

                    account.Balances.Add(new Balance()
                    {
                        Year = startYear.ToString(),
                        OperatoryType = account.OperatorType,
                        Amount = balance
                    });

                    account.ViewBalances.Add(startYear.ToString(), balance);
                }

                foreach (var subTot in subtotals)
                {
                    var subTotAccounts = data.Where(x => x.SubtotalAccountId == subTot.Id).SelectMany(x => x.Balances).Where(x => x.Year == startYear.ToString()).ToList();

                    var subTotalBalance = 0.0;

                    foreach (var subTotAccount in subTotAccounts)
                    {
                        if (subTotAccount.OperatoryType == MathOperatorType.Add)
                        {
                            subTotalBalance += subTotAccount.Amount;
                        }
                        else if(subTotAccount.OperatoryType == MathOperatorType.Subtract)
                        {
                            subTotalBalance -= subTotAccount.Amount;
                        }
                        else
                        {
                            subTotalBalance += subTotAccount.Amount;
                        }
                    }

                    subTot.ViewBalances.Add(startYear.ToString(), subTotalBalance);

                    subTot.Balances.Add(new Balance()
                    {
                        Year = startYear.ToString(),
                        OperatoryType = MathOperatorType.Add,
                        Amount = subTotalBalance
                    });
                }

                startYear += 1;
                counter++;
            }

            foreach (var acc in data.Where(x => x.IsHeading))
            {
                acc.Balances = dummyBalances;
                acc.ViewBalances = viewDummyBalances;
            }

            container.Items = data;

            return container;
        }

        public IncomeStatementContainer RunIncomeStatementMonthly(IncomeStatementViewModel vm, int organisation)
        {
            var container = new IncomeStatementContainer();

            var data = new List<IncomeStatement>();

            var start = new DateTime(vm.Start.Year, vm.Start.Month, 1);
            var end = new DateTime(vm.End.Year, vm.End.Month, 1);

            var counter = 1;

            var accounts = db.ReportIncomeStatementAccounts.Where(x => x.OrganisationId == organisation || x.IsGlobal).OrderBy(x => x.Index).ToList();

            foreach (var account in accounts)
            {
                data.Add(new IncomeStatement()
                {
                    Id = account.Id,
                    Name = account.Name,
                    IsHeading = account.IsHeading,
                    IsSubtotal = account.IsSubtotal,
                    IsEmptySpace = account.IsEmptySpace,
                    SubtotalAccountId = account.SubtotalAccountId,
                    OperatorType = account.OperatorType
                });
            }

            var accountsRequireBalance = data.Where(x => x.IsHeading == false && x.IsSubtotal == false).ToList();

            var subtotals = data.Where(x => x.IsSubtotal).ToList();

            var viewDummyBalances = new Dictionary<string, double>();

            var dummyBalances = new List<Balance>();

            while (start <= end)
            {
                container.Coloumns.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString());

                viewDummyBalances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), 0);

                dummyBalances.Add(new Balance() { Year = ((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), Amount = 0 });

                foreach (var account in accountsRequireBalance)
                {
                    var links = db.ReportIncomeStatementAccountLinks.Where(x => x.IsAccountId == account.Id).ToList();

                    var balance = 0.0;

                    foreach (var link in links)
                    {
                        balance += GetTrialBalanceAccountBalanceByMonth(start.Year, start.Month, link.TbAccountId, organisation);
                    }

                    account.ViewBalances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), balance);

                    account.Balances.Add(new Balance() { Year = ((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), Amount = balance });
                }

                foreach (var subTot in subtotals)
                {
                    var subTotAccounts = data.Where(x => x.SubtotalAccountId == subTot.Id).SelectMany(x => x.Balances).Where(x => x.Year == ((MonthShort)start.Month).ToString() + " - " + start.Year.ToString()).ToList();

                    var subTotalBalance = 0.0;

                    foreach (var subTotAccount in subTotAccounts)
                    {
                        if (subTotAccount.OperatoryType == MathOperatorType.Add)
                        {
                            subTotalBalance += subTotAccount.Amount;
                        }
                        else if (subTotAccount.OperatoryType == MathOperatorType.Subtract)
                        {
                            subTotalBalance -= subTotAccount.Amount;
                        }
                        else
                        {
                            subTotalBalance += subTotAccount.Amount;
                        }
                    }

                    subTot.ViewBalances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), subTotalBalance);

                    subTot.Balances.Add(new Balance()
                    {
                        Year = ((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(),
                        OperatoryType = MathOperatorType.Add,
                        Amount = subTotalBalance
                    });
                }

                start = start.AddMonths(1);
                counter++;
            }

            foreach (var acc in data.Where(x => x.IsHeading))
            {
                acc.Balances = dummyBalances;
                acc.ViewBalances = viewDummyBalances;
            }

            container.Items = data;

            return container;
        }

        public Result GetDebtorsTransactions(ReportParametersViewModel vm, int organisationId)
        {
            var result = new Result();

            var list = new List<DebtorsViewModel>();

            var startDate = vm.Start;
            var endDate = new DateTime(vm.End.Year, vm.End.Month, DateTime.DaysInMonth(vm.End.Year, vm.End.Month));

            var items = db.CustomerTransactions.Where(x => x.Date >= startDate && x.Date <= endDate && x.OrganisationId == organisationId).ToList();

            foreach (var item in items)
            {
                var customer = db.Customers.Where(x => x.Id == item.CustomerId).FirstOrDefault();

                list.Add(new DebtorsViewModel()
                {
                    DateStr = item.Date.HasValue ? item.Date.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty,
                    Description = item.Description,
                    Amount = item.Amount,
                    Customer = customer.CustomerName
                });
            }

            result.obj = list;

            return result;
        }

        public Result GetCreditorsTransactions(ReportParametersViewModel vm, int organisationId)
        {
            var result = new Result();

            var list = new List<CreditorsViewModel>();

            var startDate = vm.Start;
            var endDate = new DateTime(vm.End.Year, vm.End.Month, DateTime.DaysInMonth(vm.End.Year, vm.End.Month));

            var items = db.SupplierTransactions.Where(x => x.Date >= startDate && x.Date <= endDate && x.OrganisationId == organisationId).ToList();

            foreach (var item in items)
            {
                var supplier = db.Suppliers.Where(x => x.Id == item.SupplierId).FirstOrDefault();

                list.Add(new CreditorsViewModel()
                {
                    DateStr = item.Date.HasValue ? item.Date.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty,
                    Description = item.Description,
                    Amount = item.Amount,
                    Supplier = supplier.CompanyName
                });
            }

            result.obj = list;

            return result;
        }

        public Result GetVatTransactions(ReportParametersViewModel vm, int organisationId)
        {
            var result = new Result();

            var list = new List<VatReport>();

            var startDate = vm.Start;
            var endDate = new DateTime(vm.End.Year, vm.End.Month, DateTime.DaysInMonth(vm.End.Year, vm.End.Month));

            var items = db.VatTransactions.Where(x => x.Date >= startDate && x.Date <= endDate && x.OrganisationId == organisationId).ToList();

            foreach (var item in items)
            {
                var vat = db.VatOptions.Where(x => x.Id == item.TypeId).FirstOrDefault();

                if(vat != null)
                {
                    list.Add(new VatReport()
                    {
                        Date = item.Date.ToString("dd/MM/yyyy"),
                        Type = vat.Description,
                        Account = item.Account,
                        Description = item.Description,
                        Exclusive = item.Exclusive,
                        Inclusive = item.Inclusive,
                        TaxAmount = item.TaxAmount,
                    });
                }
            }

            result.obj = list;

            return result;
        }
        public Result SaveVatTransaction(VatTransaction vt, int organisationId)
        {
            var result = new Result();

            db.VatTransactions.Add(vt);

            var res = db.SaveChanges();

            if(res > 0)
            {
                result.Success = true;
                result.Message = "Successfully saved transaction";
            }
            else
            {
                result.Success = true;
                result.Message = "Could not save transaction";
            }

            return result;
        }

        //public BalanceSheetYearlyContainer RunIncomeStatementMonthly(IncomeStatementViewModel vm, int organisation)
        //{
        //    var container = new BalanceSheetYearlyContainer();

        //    var data = new List<BalanceSheetYearly>();

        //    var start = new DateTime(vm.Start.Year, vm.Start.Month, 1);
        //    var end = new DateTime(vm.End.Year, vm.End.Month, 1);

        //    var counter = 1;

        //    var accounts = db.ReportBalanceSheetAccounts.Where(x => x.OrganisationId == organisation || x.IsGlobal).OrderBy(x => x.Index).ToList();

        //    foreach (var account in accounts)
        //    {
        //        data.Add(new BalanceSheetYearly()
        //        {
        //            Id = account.Id,
        //            Name = account.Name,
        //            IsHeading = account.IsHeading,
        //            IsSubtotal = account.IsSubtotal,
        //            SubtotalAccountId = account.SubtotalAccountId
        //        });
        //    }

        //    var accountsRequireBalance = data.Where(x => x.IsHeading == false && x.IsSubtotal == false).ToList();

        //    var subtotals = data.Where(x => x.IsSubtotal).ToList();

        //    var dummyBalances = new Dictionary<string, double>();

        //    while (start <= end)
        //    {
        //        container.Coloumns.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString());

        //        dummyBalances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), 0);

        //        foreach (var account in accountsRequireBalance)
        //        {
        //            var links = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == account.Id).ToList();

        //            var balance = 0.0;

        //            foreach (var link in links)
        //            {
        //                balance += GetTrialBalanceAccountBalanceByMonth(start.Year, start.Month, link.TbAccountId, organisation);
        //            }

        //            account.Balances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), balance);
        //        }

        //        foreach (var subTot in subtotals)
        //        {
        //            var subTotAccounts = data.Where(x => x.SubtotalAccountId == subTot.Id).SelectMany(x => x.Balances).ToList();

        //            var yearBalances = subTotAccounts.Where(x => x.Key.Contains(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString())).Select(x => x.Value);

        //            var balanceSum = yearBalances.Sum();

        //            subTot.Balances.Add(((MonthShort)start.Month).ToString() + " - " + start.Year.ToString(), balanceSum);
        //        }

        //        start = start.AddMonths(1);
        //        counter++;
        //    }

        //    foreach (var acc in data.Where(x => x.IsHeading))
        //    {
        //        acc.Balances = dummyBalances;
        //    }

        //    container.Items = data;

        //    return container;
        //}


        //public Result GetLinkedIsAccounts()
        //{
        //    var result = new Result();

        //    var vm = new List<>();

        //    var links = db.ReportBalanceSheetAccountLinks.Where(x => x.BsAccountId == bsAccountId).ToList();

        //    foreach (var link in links)
        //    {
        //        var tbItem = db.TrialBalanceAccounts.Where(x => x.Id == link.TbAccountId).FirstOrDefault();

        //        vm.Add(new ReportBalanceSheetLinkedAccounts()
        //        {
        //            Name = $"{tbItem.AccountNumber} - {tbItem.AccountName}"
        //        });
        //    }

        //    result.obj = vm;

        //    return result;
        //}

        public double GetTrialBalanceAccountBalanceByYear(int year, int accountId, int organisationId)
        {
            double balance = 0.0;

            var tbItem = db.TrialBalanceAccounts.Where(x => x.Id == accountId).FirstOrDefault();

            var gls = db.GeneralLedgers.Where(x => x.AccountNumber == tbItem.AccountNumber && x.AccountDate.Year == year && x.OrganisationId == organisationId).ToList();

            var jeGls = GetJournalEntriesAsGlsByYear(year, organisationId);

            gls.AddRange(jeGls.Where(x => x.AccountNumber == tbItem.AccountNumber).ToList());

            foreach (var glAccount in gls)
            {
                balance += glAccount.Debit;
                balance -= glAccount.Credit;
            }

            return balance;
        }

        public double GetTrialBalanceAccountBalanceByMonth(int year, int month, int accountId, int organisationId)
        {
            double balance = 0.0;

            var tbItem = db.TrialBalanceAccounts.Where(x => x.Id == accountId).FirstOrDefault();

            var gls = db.GeneralLedgers.Where(x => x.AccountNumber == tbItem.AccountNumber && x.AccountDate.Year == year && x.AccountDate.Month == month && x.OrganisationId == organisationId).ToList();

            var jeGls = GetJournalEntriesAsGlsByMonth(year, month, organisationId);

            gls.AddRange(jeGls.Where(x => x.AccountNumber == tbItem.AccountNumber).ToList());

            foreach (var glAccount in gls)
            {
                balance += glAccount.Debit;
                balance -= glAccount.Credit;
            }

            return balance;
        }

        public string GetTrialBalanceAccountDetails(int id, int organisationId)
        {
            var returnStr = string.Empty;

            var item = db.TrialBalanceAccounts.Where(x => x.Id == id && (x.OrganisationId == organisationId || x.IsGlobal == true)).FirstOrDefault();

            if(item != null)
            {
                returnStr = $"{item.AccountNumber} - {item.AccountName}";
            }

            return returnStr;
        }

    }
}
