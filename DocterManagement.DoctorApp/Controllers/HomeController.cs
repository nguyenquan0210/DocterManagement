using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.DoctorApp.Models;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.System.ActiveUsers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppointmentApiClient _appointmentApiClient;
 
        public HomeController(ILogger<HomeController> logger, IAppointmentApiClient appointmentApiClient)
        {
            _logger = logger;
            _appointmentApiClient = appointmentApiClient;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.StatisticPatient = await StatisticActivePatient();
            return View();
        }
        public async Task<StatisticCountActiveUser> StatisticActivePatient()
        {
            var date = DateTime.Now;
            var requeststatictic = new GetAppointmentPagingRequest()
            {
                month = date.ToString("MM"),
                year = date.ToString("yyyy"),
                status = StatusAppointment.complete,
                UserNameDoctor = User.Identity.Name
            };
            var userMonthNow = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.count);
            requeststatictic.month = date.AddMonths(-1).ToString("MM");
            var userMonthBefor = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.count);
            var percent = 0;
            var change = "text-danger";
            if (userMonthNow >= userMonthBefor)
            {
                percent = (userMonthNow - userMonthBefor) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor);
                change = "text-success";
            }
            else
            {
                percent = (userMonthBefor - userMonthNow) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor);
            }
            return new StatisticCountActiveUser()
            {
                countuserMonthNow = userMonthNow,
                countuserMonthBefor = userMonthBefor,
                percent = percent,
                change = change
            };
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