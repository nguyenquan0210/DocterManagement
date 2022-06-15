using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Contact;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Statistic;
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
        private readonly IContactApiClient _contactApiClient;
        private readonly IPostApiClient _postApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Home";

        public HomeController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IClinicApiClient clinicApiClient, IContactApiClient contactApiClient,
            IPostApiClient postApiClient, IMasterDataApiClient masterDataApiClient, IStatisticApiClient statisticApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _clinicApiClient = clinicApiClient;
            _contactApiClient = contactApiClient;
            _postApiClient = postApiClient;
            _masterDataApiClient = masterDataApiClient;
            _statisticApiClient = statisticApiClient;
        }

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString(SystemConstants.Patient, JsonConvert.SerializeObject(new PatientVm()));
            
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "GET",
                ExtraProperties = "",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();

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
        public async Task<IActionResult> Doctor()
        {
            var result = await _doctorApiClient.GetTopFavouriteDoctors();
            ViewBag.DoctorTops = result.Data.OrderByDescending(x => x.View).Take(4).ToList();
            ViewBag.GetAllSpeciality = (await _specialityApiClient.GetAllSpeciality()).Data.ToList();
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Doctor",
                MethodName = "GET",
                ExtraProperties = "",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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

            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Clinic",
                MethodName = "GET",
                ExtraProperties = data.IsSuccessed?"success":"error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".FilterDoctorHome",
                MethodName = "GET",
                ExtraProperties = doctor.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);

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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".FilterDoctorHomeJson",
                MethodName = "GET",
                ExtraProperties = doctor.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return Json(doctor);
        }
        public async Task<IActionResult> Post()
        {
            var request = new GetPostPagingRequest()
            {
                PageIndex = 1,
                PageSize = 12,
            };
            ViewBag.Menus = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "Category").ToList();
            var data = await _postApiClient.GetAllPaging(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Post",
                MethodName = "GET",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View(data.Data);
        }
        public async Task<IActionResult> Contact()
        {
            ViewBag.Information = (await _masterDataApiClient.GetById()).Data;
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Contact",
                MethodName = "GET",
                ExtraProperties = "",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(ContactCreateRequest request)
        {
            if(!ModelState.IsValid) return View(request);
            var session = HttpContext.Session.GetString(SystemConstants.Contact);
            var currentContact = session==null?new ContactCreateRequest(): JsonConvert.DeserializeObject<ContactCreateRequest>(session);
            currentContact.container_post = currentContact.container_post + 1;
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Contact",
                MethodName = "POST",
                ExtraProperties = "",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (currentContact.PhoneNumber != null && currentContact.container_post>2)
            {
                if(currentContact.YourMessage == request.YourMessage)
                {
                    TempData["AlertMessage"] = "Bạn đã gửi nội dung này lần 2 rồi!!!";
                    
                }else
                {
                        TempData["AlertMessage"] = "Xin lỗi bạn không thể gửi liên hệ quá 2 lần trong khoảng thời gian ngắn!!!";
                   
                }
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
            }
            else
            {

                var result = await _contactApiClient.CreateContact(request);
                if (result.IsSuccessed)
                {
                    TempData["AlertMessage"] = "Bạn đã gửi liên hệ thành công.";
                    TempData["AlertType"] = "success";
                    TempData["AlertId"] = "successToast";
                    request.container_post = currentContact.container_post ;
                    HttpContext.Session.SetString(SystemConstants.Contact, JsonConvert.SerializeObject(request));
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AlertMessage"] = "" /*result.Message*/;
                    TempData["AlertType"] = "error";
                    TempData["AlertId"] = "errorToast";
                }
            }
           

            return View(request);
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