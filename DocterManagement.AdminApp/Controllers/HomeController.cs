using DoctorManagement.AdminApp.Models;
using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.Statistic;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DoctorManagement.AdminApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly IUserApiClient _userApiClient;


        public HomeController(ILogger<HomeController> logger,
            IDoctorApiClient doctorApiClient, IUserApiClient userApiClient)
        {
            _logger = logger;
            _doctorApiClient = doctorApiClient;
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Age = JsonConvert.SerializeObject(await Statistic("doctor"));
            ViewBag.User = JsonConvert.SerializeObject(await StatisticUser());
            var count = (await _doctorApiClient.GetAllUser("")).Data.Count();
             switch (count)
            {
                case >= 1000 and < 1000000:
                    ViewBag.Count = count / 1000 + "K";
                    break;
                case >= 1000000 and < 1000000000:
                    ViewBag.Count = count / 1000000 + "M";
                    break;
                case >= 1000000000:
                    ViewBag.Count = count / 1000000000 + "B";
                    break;
                default:
                    ViewBag.Count = count;
                    break;
            }
            return View();
        }
        public async Task<List<StatisticActive>> Statistic(string role)
        {
            var patient = (await _doctorApiClient.GetAllUser(role));
            var date = DateTime.Now;
            List<StatisticActive> model = new List<StatisticActive>();
            for (int i = 25; i <= 55; i += 5)
            {
                model.Add(new StatisticActive
                {
                    date = i>50? (i+1)+"+": (i+1) + "-" + (i+5),
                    qty = i>50? patient.Data.Where(x => x.DoctorVm.Dob <= date).Count() : patient.Data.Where(x => x.DoctorVm.Dob <= date && x.DoctorVm.Dob > date.AddYears(i == 25 ? -25 : -5)).Count(),
                   
                });
                date = date.AddYears(i==25?-25:-5);
            }
            return model;
        }
        public async Task<List<SelectListItem>> StatisticUser()
        {
            var users = await _doctorApiClient.GetAllUser("");
            List<SelectListItem> model = new List<SelectListItem>();
            var roles = await _userApiClient.GetAllRole();
            foreach(var role in roles)
            {
                var item = new SelectListItem()
                {
                    Text = role.Name,
                    Value = users.Data.Count(x => x.GetRole.Name == role.Name).ToString(),
                };
                model.Add(item);
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