using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CompanyTask.Models;
using CompanyTask.Extensions;
using CompanyManagementTask.Models;

namespace CompanyTask.Controllers
{
    public class CompanyController : Controller
    {
        private const string SessionUserKey = "LoggedInUser";
        private const string SessionCompanyList = "CompanyList";

        private bool IsUserLoggedIn() => HttpContext.Session.GetString(SessionUserKey) != null;

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
            companies = companies.Where(c => !c.IsDeleted).ToList();
            return View(companies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(CompanyModel model)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
                model.Id = companies.Any() ? companies.Max(c => c.Id) + 1 : 1;
                companies.Add(model);
                HttpContext.Session.SetObjectAsJson(SessionCompanyList, companies);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
            var company = companies.FirstOrDefault(c => c.Id == id);
            if (company == null)
                return NotFound();
            return View(company);
        }

        [HttpPost]
        public IActionResult Edit(CompanyModel model)
        {
            if (!IsUserLoggedIn())  
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
                var company = companies.FirstOrDefault(c => c.Id == model.Id);
                if (company == null)
                    return NotFound();

                company.CompanyName = model.CompanyName;
                company.Startdate = model.Startdate;
                company.IsActive = model.IsActive;
                HttpContext.Session.SetObjectAsJson(SessionCompanyList, companies);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
            var company = companies.FirstOrDefault(c => c.Id == id);

            if (company == null)
                return NotFound();

            company.IsDeleted = true;
            company.IsActive = false;
            HttpContext.Session.SetObjectAsJson(SessionCompanyList, companies);

            return RedirectToAction("Index");
        }

        public IActionResult DeletedCompany()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = HttpContext.Session.GetObjectFromJson<List<CompanyModel>>(SessionCompanyList) ?? new List<CompanyModel>();
            companies = companies.Where(c => c.IsDeleted).ToList();
            return View(companies);
        }
    }
}