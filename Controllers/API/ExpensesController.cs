﻿using SafriSoftv1._3.Models;
using SafriSoftv1._3.Models.ViewModels;
using SafriSoftv1._3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SafriSoftv1._3.Controllers.API
{
    [RoutePrefix("api/Expenses")]
    public class ExpensesController : BaseApiController
    {
        [HttpPost, Route("GetExpenses")]
        public async Task<IHttpActionResult> GetExpenses(DateParameters vm)
        {
            var result = new Result();            

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var eSvc = new ExpensesService();

                var res = eSvc.GetExpenses(organisationId, vm);

                result.obj = res;
                result.Success = true;
                result.Message = "Expenses";
                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }

        [HttpPost, Route("SaveExpense")]
        public async Task<IHttpActionResult> SaveExpense(ExpensesViewModelItem vm)
        {
            var result = new Result();

            try
            {
                var organisationName = GetOrganisationName();
                var organisationId = BaseService.GetOrganisationId(organisationName);

                var eSvc = new ExpensesService();

                if(vm.Type == Models.SystemModels.NominalAccountType.Expense)
                {
                    result = eSvc.SaveExpense(vm, organisationId);
                }
                else
                {
                    result = eSvc.SaveIncome(vm, organisationId);
                }
                

                return Json(result);
            }
            catch (Exception Ex)
            {
                result.Success = false;
                result.Message = Ex.Message;
                return Json(result);
            }
        }
    }
}