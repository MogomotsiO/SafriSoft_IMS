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
    public class ExpensesController : BaseController
    {
        // GET: Expenses
        public ActionResult Expenses()
        {
            var organisationName = GetOrganisationName();
            var organisationId = BaseService.GetOrganisationId(organisationName);

            var eSvc = new ExpensesService();

            var vm = eSvc.GetExpensesViewModel(organisationId);

            return View(vm);
        }
    }
}