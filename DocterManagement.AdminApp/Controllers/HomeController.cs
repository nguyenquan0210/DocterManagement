using DoctorManagement.AdminApp.Models;
using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.Statistic;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DoctorManagement.AdminApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDoctorApiClient _doctorApiClient;


        public HomeController(ILogger<HomeController> logger,
            IDoctorApiClient doctorApiClient)
        {
            _logger = logger;
            _doctorApiClient = doctorApiClient;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Age = JsonConvert.SerializeObject(await Statistic("patient"));
            return View();
        }
        public async Task<List<StatisticActive>> Statistic(string role)
        {
            var patient = (await _doctorApiClient.GetAllUser(role));
            var date = DateTime.Now;
            List<StatisticActive> model = new List<StatisticActive>();
            for (int i = 10; i <= 45; i += 5)
            {
                model.Add(new StatisticActive
                {
                    date = i>40? (i+1)+"+": (i+1) + "-" + (i+5),
                    qty = i>40? patient.Data.Where(x => x.PatientVm.Dob <= date).Count() : patient.Data.Where(x => x.PatientVm.Dob <= date && x.PatientVm.Dob > date.AddYears(i == 10 ? -10 : -5)).Count(),
                   
                });
                date = date.AddYears(i==10?-10:-5);
            }
            return model;
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