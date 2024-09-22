using Microsoft.Ajax.Utilities;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SafriSoftv1._3.Services
{
    public class JournalService: BaseService
    {
        public Result GetJournals(int organisationId)
        {
            var result = new Result();

            var vm = new List<JournalListViewModel>();
            
            var items = db.Journals.Where(x => x.OrganisationId == organisationId).ToList();

            foreach (var item in items)
            {
                var entries = db.JournalEntries.Where(x => x.JournalId == item.Id).ToList();

                var debits = entries.Sum(x => x.Debit);
                var credits = entries.Sum(x => x.Credit);

                var balance = debits + (credits * -1);

                var createdBy = dbSafriSoftApp.Users.Where(x => x.Id == item.CreatedBy).FirstOrDefault();

                var activatedBy = dbSafriSoftApp.Users.Where(x => x.Id == item.ActivatedBy).FirstOrDefault();

                vm.Add(new JournalListViewModel()
                {
                    Id = item.Id,
                    Number = item.Number,
                    Description = item.Description,
                    DateStr = item.Date.ToString("dd/MM/yyyy"),
                    Balance = balance,
                    IsActive = item.IsActive,
                    CreatedDateStr = item.Inserted.ToString("dd/MM/yyyy"),
                    CreatedBy = createdBy != null ? $"{createdBy.FirstName} {createdBy.LastName}" : string.Empty,
                    ActivatedDateStr = item.ActivatedDate.HasValue ? item.ActivatedDate.GetValueOrDefault().ToString("dd/MM/yyyy") : string.Empty,
                    ActivatedBy = activatedBy != null ? $"{activatedBy.FirstName} {activatedBy.LastName}" : string.Empty,
                    //Entries = entries,
                });
            }

            result.obj = vm;

            return result;
        }

        public Result GetJournalEntries(int jnlId, int organisationId)
        {
            var result = new Result();

            var vm = new List<JournalEntryViewModel>();

            var jnl = db.Journals.Where(x => x.Id == jnlId && x.OrganisationId == organisationId).FirstOrDefault();

            var entries = db.JournalEntries.Where(x => x.JournalId == jnlId && x.OrganisationId == organisationId).ToList();

            foreach(var entry in entries)
            {
                vm.Add(new JournalEntryViewModel()
                {
                    JournalNumber = jnl.Number,
                    DateStr = jnl.Date.ToString("yyyy-MM-dd"),
                    Description = jnl.Description,
                    Credit = entry.Credit,
                    Debit = entry.Debit,
                    AccountId = entry.AccountId,
                    Narration = entry.Narration,
                });
            }

            result.obj = vm;

            return result;
        }

        public Result SaveJournal(JournalDetailViewModel vm,int organisationId, string userId)
        {
            var result = new Result();

            var journal = new Journal
            {
                Date = vm.Date,
                Number = GenerateJnlNumber(organisationId),
                Description = vm.Description,
                OrganisationId = organisationId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                CreatedBy = userId,
                UpdatedBy = string.Empty,
            };

            var res = db.Journals.Add(journal);

            var resSave = db.SaveChanges();

            if(resSave > 0)
            {
                foreach(var entry in vm.Entries)
                {
                    var newEntry = new JournalEntry()
                    {
                        AccountId = entry.AccountId,
                        Credit = entry.Credit,
                        Debit = entry.Debit,
                        Narration = entry.Narration,
                        OrganisationId = organisationId,
                        Inserted = DateTime.Now,
                        Updated = DateTime.Now,
                        JournalId = res.Id,
                    };

                    var resEntry = db.JournalEntries.Add(newEntry);
                    var resEntrySave = db.SaveChanges();
                }
            }

            return result;
        }

        public Result UpdateJournal(JournalDetailViewModel vm, int organisationId, string userId)
        {
            var result = new Result();

            var jnl = db.Journals.Where(x => x.Id == vm.Id && x.OrganisationId == organisationId).FirstOrDefault();

            if(jnl != null)
            {
                var journal = new Journal
                {
                    Id = vm.Id,
                    Date = vm.Date,
                    Description = vm.Description,
                    Number = jnl.Number,
                    OrganisationId = organisationId,
                    Updated = DateTime.Now,
                    Inserted = jnl.Inserted,
                    CreatedBy = jnl.CreatedBy,
                    UpdatedBy = jnl.UpdatedBy,
                };

                db.Entry(jnl).State = EntityState.Detached;

                db.Entry(journal).State = EntityState.Modified;

                var resSave = db.SaveChanges();

                if (resSave > 0)
                {
                    //remove old entries
                    var oldEntries = db.JournalEntries.Where(x => x.JournalId == vm.Id && x.OrganisationId == organisationId).ToList();

                    db.JournalEntries.RemoveRange(oldEntries);

                    db.SaveChanges();

                    foreach (var entry in vm.Entries)
                    {
                        var newEntry = new JournalEntry()
                        {
                            AccountId = entry.AccountId,
                            Credit = entry.Credit,
                            Debit = entry.Debit,
                            Narration = entry.Narration,
                            OrganisationId = organisationId,
                            Inserted = DateTime.Now,
                            Updated = DateTime.Now,
                            JournalId = vm.Id,
                        };

                        var resEntry = db.JournalEntries.Add(newEntry);
                        var resEntrySave = db.SaveChanges();
                    }
                }
            }

            return result;
        }

        public Result ActivateJournal(int jnlId, int organisationId, string userId)
        {
            var result = new Result();

            var item = db.Journals.Where(x => x.Id == jnlId && x.OrganisationId == organisationId).FirstOrDefault();

            if (item != null)
            {
                item.IsActive = true;
                item.ActivatedBy = userId;
                item.ActivatedDate = DateTime.Now;

                db.SaveChanges();
            }

            return result;
        }

        public Result DeactivateJournal(int jnlId, int organisationId, string userId)
        {
            var result = new Result();

            var item = db.Journals.Where(x => x.Id == jnlId && x.OrganisationId == organisationId).FirstOrDefault();

            if (item != null)
            {
                item.IsActive = false;
                db.SaveChanges();
            }

            return result;
        }

        public string GenerateJnlNumber(int organisationId)
        {
            var jnlNumber = string.Empty;

            var lastJournal = db.Journals.Where(x => x.OrganisationId == organisationId).OrderByDescending(x => x.Id).FirstOrDefault();

            if(lastJournal == null)
            {
                jnlNumber = $"Jnl-1";
            }
            else
            {
                var num = Convert.ToInt32(lastJournal.Number.Split('-').LastOrDefault()) + 1;
                jnlNumber = $"Jnl-{num}";
            }

            return jnlNumber.ToUpper();
        }

    }
}