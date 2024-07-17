using Newtonsoft.Json;
using SafriSoftv1._3.Models.Data;
using SafriSoftv1._3.Models.ViewModels;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SafriSoftv1._3.Controllers
{
    public class SettingController : BaseController
    {
        // GET: Setting
        public ActionResult Settings()
        {
            var settingsVm = new SettingViewModel();

            var aSvc = new AccountingService();

            var accounts = aSvc.GetTrialBalanceAccounts().obj;

            var json = JsonConvert.SerializeObject(accounts);

            settingsVm.Accounts = JsonConvert.DeserializeObject<List<TrialBalanceAccount>>(json);

            return View(settingsVm);
        }
    }
}