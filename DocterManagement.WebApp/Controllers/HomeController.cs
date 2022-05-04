﻿using DoctorManagement.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoctorManagement.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Doctor()
        {
            return View();
        }
        public IActionResult Clinic()
        {
            return View();
        }
        public IActionResult Hospital()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}