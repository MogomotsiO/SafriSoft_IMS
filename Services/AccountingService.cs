using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                accountNumber = existingTb.AccountNumber + 1;

            var tb = new TrialBalanceAccount()
            {
                Id = existingTb != null ? existingTb.Id : 0, // new or existing
                AccountNumber = accountNumber,
                AccountName = vm.AccountName,
                Index = existingTb != null ? existingTb.Index + 1 : 1,
                OrganisationId = organisationId
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

            var items = db.TrialBalanceAccounts.ToList().OrderBy(x => x.Index);

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

            var mappedGlItems = db.TrialBalanceGeneralLedgerMappings.Where(x => x.TrialBalanceAccountId == id && x.OrganisationId == organisationId).ToList();

            var items = new List<MappedGlAccounts>();

            foreach (var mappedItem in mappedGlItems)
            {
                var item = db.GeneralLedgers.Where(x => x.Id == mappedItem.GeneralLedgerId).FirstOrDefault();

                if(item != null)
                {
                    items.Add(new MappedGlAccounts()
                    {
                        Id = item.Id,
                        AccountNumber = item.AccountNumber,
                        AccountName = item.AccountName
                    });
                }
            }

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
            //var existingGl = db.GeneralLedgers.Where(x => x.AccountNumber == vm.AccountNumber && x.Month == vm.Month && x.Year == vm.Year && x.OrganisationId == organisationId).FirstOrDefault();

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

                db.GeneralLedgers.Add(gl);
                id = db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Could not save Gl {vm.AccountName} - {ex.Message}";
            }            

            //if (existingGl == null)
            //{
            //    gl.Id = 0;
            //    gl.AccountNumber = vm.AccountNumber;
            //    gl.AccountName = vm.AccountName;
            //    gl.Month = vm.Month;
            //    gl.Year = vm.Year;
            //    gl.Amount = vm.Amount;
            //    gl.OrganisationId = organisationId;
            //}
            //else
            //{
            //    existingGl.AccountName = vm.AccountName;
            //    existingGl.AccountNumber = vm.AccountNumber;
            //    existingGl.Month = vm.Month;
            //    existingGl.Year = vm.Year;
            //    existingGl.Amount = vm.Amount;
            //}


            //var id = 0;

            //if (existingGl == null)
            //{
            //    db.GeneralLedgers.Add(gl);
            //    id = db.SaveChanges();
            //}
            //else
            //{
            //    db.SaveChanges();
            //    id = existingGl.Id;
            //}            

            if (id > 0) //created
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

        public Result GetGeneralLedgerAccounts(DateParameters vm, int organisationId)
        {
            var result = new Result();

            var items = db.GeneralLedgers.Where(x => x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).OrderBy(x => x.AccountDate).ThenBy(x => x.AccountNumber).ToList();

            if (items.Count() > 0)
            {
                result.Success = true;
                result.Message = "Successfully retrieved Accounts";
                result.obj = items;
            }
            else
            {
                result.Success = false;
                result.obj = new List<GeneralLedger>();
            }

            return result;
        }

        public Result GetTrialBalanceReport(DateParameters vm, int organisationId)
        {
            var result = new Result();

            var glAccountsForPeriod = db.GeneralLedgers.Where(x => x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).ToList();

            if (glAccountsForPeriod.Count() == 0)
            {
                result.Success = false;
                result.Message = "No data for period";
                result.obj = new List<TrialBalanceContainer>();
                return result;
            }

            var trialBalanceMappingsForOrganisation = db.TrialBalanceGeneralLedgerMappings.Where(x => x.OrganisationId == organisationId).ToList();

            if (trialBalanceMappingsForOrganisation.Count() == 0)
            {
                result.Success = false;
                result.Message = "No mappings for organisation";
                result.obj = new List<TrialBalanceContainer>();
                return result;
            }

            var trialBalanceItems = db.TrialBalanceAccounts.Where(x => x.OrganisationId == organisationId).ToList();

            var items = new List<TrialBalanceContainer>();

            var trialBalanceTotal = 0.00;

            foreach (var tbItem in trialBalanceItems)
            {

                var trialBalanceMappings = db.TrialBalanceGeneralLedgerMappings.Where(x => x.TrialBalanceAccountId == tbItem.Id && x.OrganisationId == organisationId);

                if(trialBalanceMappings.Count() > 0)
                {
                    var total = 0.00;

                    var glAccounts = new List<GeneralLedger>();

                    foreach (var mapping in trialBalanceMappings)
                    {
                        var glAccount = db.GeneralLedgers.Where(x => x.Id == mapping.GeneralLedgerId && x.AccountDate >= vm.Start && x.AccountDate <= vm.End && x.OrganisationId == organisationId).FirstOrDefault();

                        if (glAccount != null)
                        {
                            total += glAccount.Debit;
                            total -= glAccount.Credit;

                            glAccounts.Add(glAccount);
                        }
                    }

                    trialBalanceTotal += total;

                    items.Add(new TrialBalanceContainer()
                    {
                        AccountNumber = tbItem.AccountNumber,
                        AccountName = tbItem.AccountName,
                        Index = tbItem.Index,
                        Period = $"{vm.Start.ToString("dd MMMM yyyy")} - {vm.End.ToString("dd MMMM yyyy")}",
                        Total = Math.Round(total, 2),
                        TrialBalanceItems = glAccounts
                    });
                }

            }

            items.Add(new TrialBalanceContainer()
            {
                Total = trialBalanceTotal,
                Index = 1000000
            });

            result.Success = true;
            result.obj = items.OrderBy(x => x.Index);

            return result;
        }
    }
}
