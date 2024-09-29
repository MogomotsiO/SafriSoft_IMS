using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SafriSoftv1._3.Services
{
    public class ExpensesService : BaseService
    {
        public ExpensesViewModel GetExpensesViewModel(int orgId)
        {
            var vm = new ExpensesViewModel();

            var aSvc = new AccountingService();

            vm.categories = db.Settings.Where(x => x.SettingType == Models.SystemModels.SettingType.ExpenseCategories && x.OrganisationId == orgId).ToList();

            vm.Accounts = aSvc.GetTrialBalanceAccounts(orgId);

            vm.VatOptions = db.VatOptions.Where(x => x.OrganisationId == orgId).ToList();

            return vm;
        }

        public List<ExpensesViewModelItems> GetExpenses(int orgId, DateParameters dateVm)
        {
            var vm = new List<ExpensesViewModelItems>();

            var items = db.Expenses.Where(x => x.OrganisationId == orgId && x.Date >= dateVm.Start && x.Date <= dateVm.End).ToList();

            foreach (var item in items)
            {
                var category = db.Settings.Where(x => x.Id == item.CategoryId).FirstOrDefault();
                
                var account = db.TrialBalanceAccounts.Where(x => x.Id == item.AccountId).FirstOrDefault();
                
                var bankAccount = db.TrialBalanceAccounts.Where(x => x.Id == item.BankAccountId).FirstOrDefault();

                var vat = db.VatOptions.Where(x => x.Id == item.VatOptionId).FirstOrDefault();

                var vatAccount = db.TrialBalanceAccounts.Where(x => x.Id == vat.TaxAccountId).FirstOrDefault();

                vm.Add(new ExpensesViewModelItems
                {
                    Date = item.Date.ToString("dd/MM/yyyy"),
                    Amount = item.Amount,
                    Name = item.Name,
                    Category = category.Name,
                    Type = item.Type.ToString(),
                    AccountName = $"{account.AccountNumber} - {account.AccountName}",
                    VatAccountName = $"{vatAccount.AccountNumber} - {vatAccount.AccountName}",
                    BankAccountName = $"{bankAccount.AccountNumber} - {bankAccount.AccountName}",
                    VatAmount = item.Amount * (vat.Percentage / 100),
                });
            }

            return vm;
        }

        public Result SaveExpense(ExpensesViewModelItem vm, int orgId)
        {
            var result = new Result();

            var aSvc = new AccountingService();

            var item = new Expense
            {
                Date = vm.Date,
                Name = vm.Name,
                Amount = vm.Amount,
                CategoryId = vm.CategoryId,
                AccountId = vm.AccountId,
                VatAccountId = vm.VatAccountId,
                VatOptionId = vm.VatOptionId,
                BankAccountId = vm.BankAccountId,
                OrganisationId = orgId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                Type = vm.Type,
            };

            db.Expenses.Add(item);

            var res = db.SaveChanges();

            if(res > 0)
            {
                if(vm.AccountId != -100)
                {
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == vm.AccountId).FirstOrDefault();

                    if(account != null)
                    {
                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - EXP",
                            AccountName = $"{account.AccountName}",
                            AccountNumber = account.AccountNumber,
                            Description = $"EXP - {vm.Name}",
                            Debit = vm.Amount > 0 ? vm.Amount : 0,
                            Credit = vm.Amount < 0 ? vm.Amount : 0,
                            Date = vm.Date,
                            Month = vm.Date.Month,
                            Year = vm.Date.Year,
                        };

                        var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);
                    }

                    
                }

                if (vm.VatOptionId != -100)
                {
                    var vatOption = db.VatOptions.Where(x => x.Id == vm.VatOptionId).FirstOrDefault();

                    if(vatOption != null)
                    {
                        var vatAccount = db.TrialBalanceAccounts.Where(x => x.Id == vatOption.TaxAccountId).FirstOrDefault();

                        if (vatAccount != null)
                        {
                            var vatAmount = vm.Amount * (vatOption.Percentage / 100);

                            var gl = new GlAccountViewModel
                            {
                                AccountReference = "REF(ID) - EXP",
                                AccountName = $"{vatAccount.AccountName}",
                                AccountNumber = vatAccount.AccountNumber,
                                Description = $"EXP - {vm.Name}",
                                Debit = vatAmount > 0 ? vatAmount : 0,
                                Credit = vatAmount < 0 ? vatAmount : 0,
                                Date = vm.Date,
                                Month = vm.Date.Month,
                                Year = vm.Date.Year,
                            };

                            var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);

                            if(glRes.Success == true)
                            {
                                var vt = new VatTransaction()
                                {
                                    Date = vm.Date,
                                    TypeId = vatOption.Id,
                                    Account = vatAccount.AccountNumber,
                                    Description = $"EXP - {vm.Name}",
                                    Exclusive = vm.Amount,
                                    Inclusive = vm.Amount + vatAmount,
                                    TaxAmount = vatAmount,
                                    Inserted = DateTime.Now,
                                    Updated = DateTime.Now,
                                    OrganisationId = orgId,
                                };

                                var vtRes = aSvc.SaveVatTransaction(vt, orgId);
                            }
                        }
                    }
                    
                }

                if (vm.BankAccountId != -100)
                {
                    var bankAccount = db.TrialBalanceAccounts.Where(x => x.Id == vm.BankAccountId).FirstOrDefault();

                    var vatOption = db.VatOptions.Where(x => x.Id == vm.VatOptionId).FirstOrDefault();

                    var totalAmount = vm.Amount + (vm.Amount * (vatOption.Percentage / 100));
                    
                    if (bankAccount != null)
                    {
                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - EXP",
                            AccountName = $"{bankAccount.AccountName}",
                            AccountNumber = bankAccount.AccountNumber,
                            Description = $"EXP - {vm.Name}",
                            Debit = 0,
                            Credit = totalAmount,
                            Date = vm.Date,
                            Month = vm.Date.Month,
                            Year = vm.Date.Year,
                        };

                        var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);
                    }
                }

                result.Success = true;
                result.Message = "Successfully saved";

            }
            else
            {
                result.Success = false;
                result.Message = "Could not save the expense details";
            }



            return result;
        }

        public Result SaveIncome(ExpensesViewModelItem vm, int orgId)
        {
            var result = new Result();

            var aSvc = new AccountingService();

            var item = new Expense
            {
                Date = vm.Date,
                Name = vm.Name,
                Amount = vm.Amount,
                CategoryId = vm.CategoryId,
                AccountId = vm.AccountId,
                VatAccountId = vm.VatAccountId,
                VatOptionId = vm.VatOptionId,
                BankAccountId = vm.BankAccountId,
                OrganisationId = orgId,
                Inserted = DateTime.Now,
                Updated = DateTime.Now,
                Type = vm.Type,
            };

            db.Expenses.Add(item);

            var res = db.SaveChanges();

            if (res > 0)
            {
                if (vm.AccountId != -100)
                {
                    var account = db.TrialBalanceAccounts.Where(x => x.Id == vm.AccountId).FirstOrDefault();

                    if (account != null)
                    {
                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - INC",
                            AccountName = $"{account.AccountName}",
                            AccountNumber = account.AccountNumber,
                            Description = $"INC - {vm.Name}",
                            Debit = 0,
                            Credit = vm.Amount,
                            Date = vm.Date,
                            Month = vm.Date.Month,
                            Year = vm.Date.Year,
                        };

                        var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);
                    }


                }

                if (vm.VatOptionId != -100)
                {
                    var vatOption = db.VatOptions.Where(x => x.Id == vm.VatOptionId).FirstOrDefault();

                    if (vatOption != null)
                    {
                        var vatAccount = db.TrialBalanceAccounts.Where(x => x.Id == vatOption.TaxAccountId).FirstOrDefault();

                        if (vatAccount != null)
                        {
                            var vatAmount = vm.Amount * (vatOption.Percentage / 100);

                            var gl = new GlAccountViewModel
                            {
                                AccountReference = "REF(ID) - INC",
                                AccountName = $"{vatAccount.AccountName}",
                                AccountNumber = vatAccount.AccountNumber,
                                Description = $"INC - {vm.Name}",
                                Debit = 0,
                                Credit = vatAmount,
                                Date = vm.Date,
                                Month = vm.Date.Month,
                                Year = vm.Date.Year,
                            };

                            var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);

                            if (glRes.Success == true)
                            {
                                var vt = new VatTransaction()
                                {
                                    Date = vm.Date,
                                    TypeId = vatOption.Id,
                                    Account = vatAccount.AccountNumber,
                                    Description = $"INC - {vm.Name}",
                                    Exclusive = vm.Amount,
                                    Inclusive = vm.Amount + vatAmount,
                                    TaxAmount = vatAmount,
                                    Inserted = DateTime.Now,
                                    Updated = DateTime.Now,
                                    OrganisationId = orgId,
                                };

                                var vtRes = aSvc.SaveVatTransaction(vt, orgId);
                            }
                        }
                    }

                }

                if (vm.BankAccountId != -100)
                {
                    var bankAccount = db.TrialBalanceAccounts.Where(x => x.Id == vm.BankAccountId).FirstOrDefault();

                    var vatOption = db.VatOptions.Where(x => x.Id == vm.VatOptionId).FirstOrDefault();

                    var totalAmount = vm.Amount + (vm.Amount * (vatOption.Percentage / 100));

                    if (bankAccount != null)
                    {
                        var gl = new GlAccountViewModel
                        {
                            AccountReference = "REF(ID) - INC",
                            AccountName = $"{bankAccount.AccountName}",
                            AccountNumber = bankAccount.AccountNumber,
                            Description = $"INC - {vm.Name}",
                            Debit = totalAmount,
                            Credit = 0,
                            Date = vm.Date,
                            Month = vm.Date.Month,
                            Year = vm.Date.Year,
                        };

                        var glRes = aSvc.CreateUpdateGlAccount(gl, orgId);
                    }
                }

                result.Success = true;
                result.Message = "Successfully saved";

            }
            else
            {
                result.Success = false;
                result.Message = "Could not save the expense details";
            }

            return result;
        }
    }
}