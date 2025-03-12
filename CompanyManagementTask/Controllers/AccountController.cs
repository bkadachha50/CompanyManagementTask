using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using CompanyTask.Models;
using CompanyTask.Extensions;

namespace CompanyTask.Controllers
{
    public class AccountController : Controller
    {
        private const string SessionUserKey = "LoggedInUser";
        private const string SessionUserList = "UserList";

        private bool IsUserLoggedIn() => HttpContext.Session.GetString(SessionUserKey) != null;

        [HttpGet]
        public IActionResult Register()
        {
            if (IsUserLoggedIn())
                return RedirectToAction("Index", "Company");
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var userList = HttpContext.Session.GetObjectFromJson<List<UserModel>>(SessionUserList) ?? new List<UserModel>();

                if (userList.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                model.Id = userList.Any() ? userList.Max(u => u.Id) + 1 : 1;
                userList.Add(model);

                HttpContext.Session.SetObjectAsJson(SessionUserList, userList);
                TempData["Success"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsUserLoggedIn())
                return RedirectToAction("Index", "Company");
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel model)
        {
            var userList = HttpContext.Session.GetObjectFromJson<List<UserModel>>(SessionUserList) ?? new List<UserModel>();
            var user = userList.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetString(SessionUserKey, user.Email);
                return RedirectToAction("Index", "Company");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionUserKey);
            return RedirectToAction("Login");
        }

        public IActionResult UserList()
        {
            var userList = HttpContext.Session.GetObjectFromJson<List<UserModel>>(SessionUserList) ?? new List<UserModel>();
            return View(userList);
        }
    }
}
