using SafriSoftv1._3.Models.ViewModels;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class JournalController : BaseController
    {
        // GET: Journal
        public ActionResult Journals()
        {
            var vm = new JournalViewModel();

            var aSvc = new AccountingService();

            var organisationName = GetOrganisationName();

            var organisationId = BaseService.GetOrganisationId(organisationName);

            vm.TrialBalanceAccounts = aSvc.GetTrialBalanceAccounts(organisationId);

            return View(vm);
        }
    }
}