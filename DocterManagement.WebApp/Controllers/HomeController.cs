using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
using DoctorManagement.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DoctorManagement.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IClinicApiClient _clinicApiClient;

        public HomeController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IClinicApiClient clinicApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _clinicApiClient = clinicApiClient;
        }
        
        public IActionResult Index()
        {
            HttpContext.Session.SetString(SystemConstants.Patient, JsonConvert.SerializeObject(new PatientVm()));
            return View();

        }
        public async Task<IActionResult> Doctor()
        {
            var result = await _doctorApiClient.GetTopFavouriteDoctors();
            ViewBag.DoctorTops = result.Data.OrderByDescending(x => x.View).Take(4).ToList();
            ViewBag.GetAllSpeciality = (await _specialityApiClient.GetAllSpeciality()).Data.ToList();
            return View();
        }
        public async Task<IActionResult> Clinic(string keyword, int pageIndex = 1, int pageSize = 15)
        {
            var request = new GetClinicPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _clinicApiClient.GetClinicPagings(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        public IActionResult Hospital()
        {
            return View();
        }
        public async Task<IActionResult> FilterDoctorHome(string keyword, string searchspeciality, int pageIndex = 1, int pageSize = 20)
        {
            //ViewBag.GetAllSpeciality = (await _specialityApiClient.GetAllSpeciality()).Data.ToList();
            ViewBag.SearchSpeciality = searchspeciality;
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = "doctor",
                searchSpeciality = searchspeciality,
                checkclient = true,
            };
            ViewBag.Keyword = keyword;
            var doctor = await _userApiClient.GetUsersPagings(request);
            var requestClinic = new GetClinicPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = 100
            };
            ViewBag.Clinics = (await _clinicApiClient.GetClinicPagings(requestClinic)).Data.Items;
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            
            return View(doctor.Data);
        }
        public async Task<IActionResult> FilterDoctorHomeJson(string keyword, string searchSpeciality, int pageIndex = 1, int pageSize = 20)
        {
            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = "doctor",
                searchSpeciality = searchSpeciality,
                checkclient = true
            };

            var doctor = await _userApiClient.GetUsersPagings(request);
            return Json(doctor);
        }
        public IActionResult Contact()
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
        }/*
        Khách hàng phải khám sàng lọc và test nhanh Covid-19 trước khi tiến hành thăm
                                        khám với Bác sĩ và thực hiện các liệu trình chăm sóc da(Giá test:
                                        150.000đ/lần).
                                        -Các trường hợp được MIỄN test nhanh Covid:
                                        ✅ Trường hợp 1: người đã khỏi bệnh Covid(F0 khỏi bệnh) trên 14 ngày có giấy xác
                                        nhận của cơ quan y tế trong thời hạn 6 tháng.
                                        ✅ Trường hợp 2: người có giấy xét nghiệm âm tính với SARS-CoV2 có hiệu lực trong
                                        thời hạn 72 giờ(3 ngày) và không có các triệu chứng nghi ngờ mắc Covid 19.
                                        ✅ Trường hợp 3: người đã tiêm 2 mũi vaccin ngừa Covid 19 (đã đủ 14 ngày kể từ
                                        lúc tiêm mũi 2).*/
    }
}