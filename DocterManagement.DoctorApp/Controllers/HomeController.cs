using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.DoctorApp.Models;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.System.ActiveUsers;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
            ViewBag.StatisticAppointmentAll = JsonConvert.SerializeObject(await StatisticAppointmentAll());
            ViewBag.StatisticAgePatient = JsonConvert.SerializeObject(await StatisticAgePatient());
            var count = (await _appointmentApiClient.GetAllAppointment(User.Identity.Name)).Data.Count();
            ViewBag.Count = SetCount(count);
            ViewBag.StatisticPatient = await StatisticActivePatient("patient");
            ViewBag.StatisticAppointment = await StatisticActivePatient("appointment");
            ViewBag.StatisticRevanue = await StatisticActivePatient("ravanue");
            ViewBag.TopPatientCompleAppointment = await TopPatientCompleAppointment();
            
            return View();
        }
        public async Task<List<AppointmentVm>> TopPatientCompleAppointment()
        {
            return (await _appointmentApiClient.GetAllAppointment(User.Identity.Name)).Data.Where(x=>x.Status == StatusAppointment.complete).DistinctBy(x => x.Patient.Id).ToList();
        }
        public async Task<List<StatisticActive>> StatisticAgePatient()
        {
            var patient = (await _appointmentApiClient.GetAllAppointment(User.Identity.Name));
            var date = DateTime.Now;
            List<StatisticActive> model = new List<StatisticActive>();
            for (int i = 10; i <= 55; i += 5)
            {
                model.Add(new StatisticActive
                {
                    date = i > 50 ? (i + 1) + "+" : (i + 1) + "-" + (i + 5),
                    qty = i > 50 ? patient.Data.Where(x => x.Patient.Dob <= date).DistinctBy(x=>x.Patient.Id).Count() : patient.Data.Where(x => x.Patient.Dob <= date && x.Patient.Dob > date.AddYears(i == 10 ? -10 : -5)).DistinctBy(x => x.Patient.Id).Count(),

                });
                date = date.AddYears(i == 10 ? -10 : -5);
            }
            return model;
        }
        public async Task<List<SelectListItem>> StatisticAppointmentAll()
        {
            var users = await _appointmentApiClient.GetAllAppointment(User.Identity.Name);
            List<SelectListItem> model = new List<SelectListItem>();
            List<StatusAppointment> status = new List<StatusAppointment>() { 
                StatusAppointment.complete, 
                StatusAppointment.pending,
                StatusAppointment.cancel,
                StatusAppointment.approved
            };
            foreach (var sttus in status)
            {
                var item = new SelectListItem()
                {
                    Text = sttus == StatusAppointment.complete?"đã khám": sttus == StatusAppointment.pending?"quá hạn": sttus == StatusAppointment.approved ? "chờ khám":"hủy khám",
                    Value = users.Data.Count(x => x.Status == sttus).ToString(),
                };
                model.Add(item);
            }
            return model;
        }
        public async Task<StatisticCountActiveUser> StatisticActivePatient(string check)
        {
            var date = DateTime.Now;
            var requeststatictic = new GetAppointmentPagingRequest()
            {
                month = date.ToString("MM"),
                year = date.ToString("yyyy"),
                status = StatusAppointment.complete,
                UserNameDoctor = User.Identity.Name
            };
            decimal userMonthNow = 0;
            decimal userMonthBefor = 0;
            switch (check)
            {
                case "patient":
                    userMonthNow = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.countpatient);
                    requeststatictic.month = date.AddMonths(-1).ToString("MM");
                    userMonthBefor = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.countpatient);
                    break;
                case "appointment":
                    userMonthNow = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.count);
                    requeststatictic.month = date.AddMonths(-1).ToString("MM");
                    userMonthBefor = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.count);
                    break;
                default:
                    userMonthNow = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.amount);
                    requeststatictic.month = date.AddMonths(-1).ToString("MM");
                    userMonthBefor = (await _appointmentApiClient.GetAppointmentStatiticMonth(requeststatictic)).Sum(x => x.amount);
                    break;
            }
            var percent = 0;
            var change = "text-danger";
            if (userMonthNow >= userMonthBefor)
            {
                percent = (int)((userMonthNow - userMonthBefor) * 100 / (userMonthBefor == 0 ? 1 : userMonthBefor));
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