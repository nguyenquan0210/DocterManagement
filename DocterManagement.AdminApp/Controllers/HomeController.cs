using DoctorManagement.AdminApp.Models;
using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.ActiveUsers;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
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
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly IAnnualServiceFeeApiClient _annualServiceFeeApiClient;
        private readonly IUserApiClient _userApiClient;


        public HomeController(ILogger<HomeController> logger,
            IDoctorApiClient doctorApiClient, IUserApiClient userApiClient,
            IStatisticApiClient statisticApiClient, IAnnualServiceFeeApiClient annualServiceFeeApiClient)
        {
            _logger = logger;
            _doctorApiClient = doctorApiClient;
            _userApiClient = userApiClient;
            _statisticApiClient = statisticApiClient;
            _annualServiceFeeApiClient = annualServiceFeeApiClient;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Age = JsonConvert.SerializeObject(await Statistic("doctor"));
            ViewBag.User = JsonConvert.SerializeObject(await StatisticUser());
            var count = (await _doctorApiClient.GetAllUser("")).Data.Count();
            ViewBag.Count = SetCount(count);
            ViewBag.ActiveUserRoleDoctor = await StatisticActiveUser("doctor");
            ViewBag.ActiveUserRolePatient = await StatisticActiveUser("patient");
            ViewBag.StatisticServiceFee = await StatisticServiceFee();
            ViewBag.TopRateDoctor = (await _doctorApiClient.GetTopFavouriteDoctors()).Data.OrderByDescending(x=>x.Rating).ToList();
            return View();
        }
        public async Task<StatisticCountActiveUser> StatisticServiceFee()
        {
            var date = DateTime.Now;
            var requeststatictic = new GetAnnualServiceFeePagingRequest()
            {
                month = date.ToString("MM"),
                year = date.ToString("yyyy"),
                status = Data.Enums.StatusAppointment.complete,
            };
            var userMonthNow = (await _annualServiceFeeApiClient.GetServiceFeeStatiticMonth(requeststatictic)).Sum(x => x.amount * 1000000);
            requeststatictic.month = date.AddMonths(-1).ToString("MM");
            var userMonthBefor = (await _annualServiceFeeApiClient.GetServiceFeeStatiticMonth(requeststatictic)).Sum(x => x.amount * 1000000);
            var percent = 0;
            var change = "text-danger";
            
            if (userMonthNow >= userMonthBefor)
            {
                percent = (int) ((userMonthNow - userMonthBefor) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor));
                change = "text-success";
            }
            else
            {
                percent = (int)((userMonthBefor - userMonthNow) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor));
            }
            return new StatisticCountActiveUser()
            {
                countuserMonthNow = userMonthNow,
                countuserMonthBefor = userMonthBefor,
                percent = percent,
                change = change
            };
        }
        public async Task<StatisticCountActiveUser> StatisticActiveUser(string role)
        {
            var date = DateTime.Now;
            var requeststatictic = new GetHistoryActivePagingRequest()
            {
                month = date.ToString("MM"),
                year = date.ToString("yyyy"),
                role = role,
            };
            var userMonthNow = (await _statisticApiClient.GetServiceFeeStatiticMonth(requeststatictic)).Sum(x=>x.count);
            requeststatictic.month = date.AddMonths(-1).ToString("MM");
            var userMonthBefor = (await _statisticApiClient.GetServiceFeeStatiticMonth(requeststatictic)).Sum(x => x.count);
            var percent = 0;
            var change = "text-danger";
            if(userMonthNow >= userMonthBefor)
            {
                percent = (userMonthNow - userMonthBefor)*100 / (userMonthBefor==0?1: userMonthBefor) ;
                change = "text-success";
            }
            else
            {
                percent = (userMonthBefor - userMonthNow) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor);
            }
            return new StatisticCountActiveUser() {
                countuserMonthNow = userMonthNow,
                countuserMonthBefor = userMonthBefor,
                percent = percent,
                change = change
            };
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