﻿using HealthcarePortal.Models;
using HealthcarePortal.Models.Data;
using HealthcarePortal.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HealthcarePortal.Controllers
{
    [Authorize(Roles = "Sales")]
    public class ProposalController : Controller
    {
        private readonly HealthcarePortalContext _db = new HealthcarePortalContext();

        public ActionResult Create()
        {
            var viewModel = new ProposalViewModel
            {
                AdminUser = _db.Users.Include(x => x.Roles).ToList().Where(x => Roles.IsUserInRole(x.Email, "Admin"))
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployee(ProposalViewModel viewModel)
        {
            int noOfEmployee = viewModel.Proposal.NumberOfEmployee;
            TempData["Proposal"] = viewModel.Proposal;
            var employees = new List<Employee>(Enumerable.Repeat(new Employee(), noOfEmployee));

            return View(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectPlan(IEnumerable<Employee> employees)
        {
            TempData["Employees"] = employees;

            var viewModel = new SelectPlanViewModel
            {
                Plans = _db.Plans.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProposal(SelectPlanViewModel viewModel)
        {
            var proposal = TempData["Proposal"] as Proposal;
            var employees = TempData["Employees"] as IEnumerable<Employee>;
            int planId = viewModel.Plan.Id;

            proposal.PlanId = planId;
            proposal.Employees = employees as ICollection<Employee>;

            _db.Proposals.Add(proposal);
            foreach (var employee in proposal.Employees)
            {
                _db.Employees.Add(employee);
            }
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}