﻿using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(LoginRequest request)
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterEnterOTP()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterEnterPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterEnterProfile()
        {
            return View();
        }
    }
}
