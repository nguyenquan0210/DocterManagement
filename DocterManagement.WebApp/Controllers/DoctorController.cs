using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;

        public DoctorController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IScheduleApiClient scheduleApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _scheduleApiClient = scheduleApiClient;
        }
        public async Task<IActionResult> Index(Guid Id)
        {
            var doctor = await _doctorApiClient.GetById(Id);
            ViewBag.GetScheduleDoctor = (await _scheduleApiClient.GetScheduleDoctor(Id)).Data.ToList();
            if (!doctor.IsSuccessed)
            {
                return RedirectToAction("Index", "Home");
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
            return View(doctor.Data);
        }
       
    }
}
