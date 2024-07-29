using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class AccountingController : BaseController
    {
        // GET: Accounting
        public ActionResult TrialBalanceAccounts()
        {
            return View();
        }

        public ActionResult GeneralLedger()
        {
            return View();
        }

        public ActionResult TrialBalanceReport()
        {
            return View();
        }

        public ActionResult BalanceSheetAccounts()
        {
            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            var aSvc = new AccountingService();

            var vm = aSvc.GetBalanceSheetAccountList(organisationId);

            return View(vm);
        }

        public ActionResult IncomeStatementAccounts()
        {
            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            var aSvc = new AccountingService();

            var vm = aSvc.GetIncomeStatementAccountList(organisationId);

            return View(vm);
        }
    }
}