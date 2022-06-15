using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Statistic;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.WebApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IPostApiClient _postApiClient;
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Doctor";

        public DoctorController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IPostApiClient postApiClient, IScheduleApiClient scheduleApiClient,
            IAppointmentApiClient appointmentApiClient, IStatisticApiClient statisticApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _scheduleApiClient = scheduleApiClient;
            _postApiClient = postApiClient;
            _appointmentApiClient = appointmentApiClient;
            _statisticApiClient = statisticApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? usertemporary = null;
            string? user = null;
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);
                usertemporary = currentHistory.Usertemporary;
                user = currentHistory.User;
            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name == null ? user : User.Identity.Name,
                    Usertemporary = (usertemporary == null && User.Identity.Name == null) ? ("patient" + new Random().Next(10000000, 99999999) + new Random().Next(10000000, 99999999)) : (usertemporary == null ? User.Identity.Name : usertemporary),
                    Type = user == null ? "patientlogout" : "patient",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        public async Task<IActionResult> Index(Guid Id, int pageIndex = 1, int pageSize = 10)
        {
            var doctor = await _doctorApiClient.GetById(Id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "GET",
                ExtraProperties = doctor.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            var session = HttpContext.Session.GetString(SystemConstants.CheckPostInfo);
            var currentContact =  session==null?false:JsonConvert.DeserializeObject<bool>(session);
            ViewBag.CheckPostInfo = currentContact;
            ViewBag.GetScheduleDoctor = (await _scheduleApiClient.GetScheduleDoctor(Id)).Data.ToList();
            var requestRating = new GetAppointmentPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserNameDoctor = doctor.Data.User.UserName,
            };
            ViewBag.Ratings = (await _appointmentApiClient.GetAppointmentPagingRating(requestRating)).Data;
            if (!doctor.IsSuccessed)
            {
                return RedirectToAction("Index", "Home");
            }
            var request = new GetPostPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TopicId = Id
            };
             
            ViewBag.Posts = (await _postApiClient.GetAllPaging(request)).Data;
            if (pageIndex > 1 )
            {
                ViewBag.CheckPostInfo = true;
                HttpContext.Session.SetString(SystemConstants.CheckPostInfo, JsonConvert.SerializeObject(true));
            }
            else if (session == "null")
            {
                ViewBag.CheckPostInfo = false;
                HttpContext.Session.SetString(SystemConstants.CheckPostInfo, JsonConvert.SerializeObject(false));
            }
            return View(doctor.Data);
        }
        public async Task<IActionResult> DoctorSpeciality(Guid Id, string keyword, string searchSpeciality, int pageIndex = 1, int pageSize = 20)
        {
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = "doctor",
                SpecialityId = Id,
                searchSpeciality = searchSpeciality
            };

            var doctor = await _userApiClient.GetUsersPagings(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DoctorSpeciality",
                MethodName = "GET",
                ExtraProperties = doctor.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            ViewBag.GetAllSpeciality = (await _specialityApiClient.GetAllSpeciality()).Data.ToList();
            ViewBag.SpecialityId = Id;
            ViewBag.SearchSpeciality = searchSpeciality;
            if (!doctor.IsSuccessed)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(doctor.Data);
        }
        public async Task<IActionResult> DoctorSpecialityJson(Guid Id, string keyword, string searchSpeciality, int pageIndex = 1, int pageSize = 20)
        {
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = "doctor",
                SpecialityId = Id,
                searchSpeciality = searchSpeciality
            };

            var doctor = await _userApiClient.GetUsersPagings(request);

            return Json(doctor);
        }
        public async Task<IActionResult> DoctorSpecialityDemo(Guid specialityid, string keyword, string searchspeciality, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.GetAllSpeciality = (await _specialityApiClient.GetAllSpeciality()).Data.ToList();
            ViewBag.SpecialityId = specialityid;
            ViewBag.SearchSpeciality = searchspeciality;
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = "doctor",
                SpecialityId = specialityid,
                searchSpeciality = searchspeciality
            };

            var doctor = await _userApiClient.GetUsersPagings(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DoctorSpeciality",
                MethodName = "GET",
                ExtraProperties = doctor.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View(doctor.Data);
        }
       
    }
}
