using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class AccountingController : Controller
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
    }
}