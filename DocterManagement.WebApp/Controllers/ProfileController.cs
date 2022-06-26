using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Statistic;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IClinicApiClient _clinicalApiClient;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Profile";
        public ProfileController(IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IScheduleApiClient scheduleApiClient, IAppointmentApiClient appointmentApiClient,
            IClinicApiClient clinicApiClient, ILocationApiClient locationApiClient, IStatisticApiClient statisticApiClient)
        {
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _scheduleApiClient = scheduleApiClient;
            _appointmentApiClient = appointmentApiClient;
            _clinicalApiClient = clinicApiClient;
            _locationApiClient = locationApiClient;
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
                    Type = user == null ? "passersby" : "patient",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "GET",
                ExtraProperties = "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        public async Task<IActionResult> Account()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data.FirstOrDefault(x=>x.IsPrimary);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Account",
                MethodName = "GET",
                ExtraProperties = "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Account(ChangePasswordRequest request)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data.FirstOrDefault(x => x.IsPrimary);

            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.ChangePassword(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Account",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed? "success":"error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin thành công";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";
                return RedirectToAction("Index");
            }
            
            TempData["AlertMessage"] = result.ValidationErrors[1];
            TempData["AlertType"] = "error";
            TempData["AlertId"] = "errorToast";
            return View(request);
        }
        public async Task<IActionResult> UpdateInfo(Guid Id)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            
            var result = await _doctorApiClient.GetByPatientId(Id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".UpdateInfo",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                var data = result.Data;
                var patient = new UpdatePatientInfoRequest()
                {
                    Id = data.Id,
                    LocationId = data.Location.Id,
                    Address = data.Address,
                    Dob = data.Dob,
                    Gender = data.Gender,
                    Name = data.Name,
                    EthnicId = data.Ethnics.Id,
                    Identitycard = data.Identitycard,
                    RelativePhone = data.RelativePhone,
                    RelativeName = data.RelativeName,
                    RelativeEmail = data.RelativeEmail,
                    DistrictId = data.Location.District.Id,
                    ProvinceId = data.Location.District.Province.Id,
                };
                ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(), patient.ProvinceId);
                ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(), patient.DistrictId);
                ViewBag.RelativeName = patient.RelativeName;
                return View(patient);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UpdatePatientInfoRequest request)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(), request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(), request.DistrictId);
            if (!ModelState.IsValid) return View(request);
            var result = await _doctorApiClient.UpdateInfo(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".UpdateInfo",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }
            TempData["AlertMessage"] = "Thay đổi thông tin thành công";
            TempData["AlertType"] = "success";
            TempData["AlertId"] = "successToast";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddInfo()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = new List<SelectListItem>();
            ViewBag.SubDistrict = new List<SelectListItem>();
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".AddInfo",
                MethodName = "GET",
                ExtraProperties = "success" ,
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddInfo(AddPatientInfoRequest request)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(),request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(),request.DistrictId);
            if (!ModelState.IsValid) return View(request);
            var result = await _doctorApiClient.AddInfo(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".UpdateInfo",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }
            else
            {
                TempData["AlertMessage"] = "Hủy lịch khám thành công.";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";

            }
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetSubDistrict(Guid DistrictId)
        {
            if (!string.IsNullOrWhiteSpace(DistrictId.ToString()))
            {
                var district = await _locationApiClient.GetAllSubDistrict(null, DistrictId);
                return Json(district);
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> GetDistrict(Guid ProvinceId)
        {
            if (!string.IsNullOrWhiteSpace(ProvinceId.ToString()))
            {
                var district = await _locationApiClient.CityGetAllDistrict(null, ProvinceId);
                return Json(district);
            }
            return null;
        }
        public async Task<IActionResult> Appointment(string keyword,Guid? Id, StatusAppointment? status, int pageIndex = 1, int pageSize = 15)
        {
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name,
                status = status
            };
            var appointments = await _appointmentApiClient.GetAppointmentPagings(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Appointment",
                MethodName = "GET",
                ExtraProperties = appointments.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            ViewBag.Appointments = appointments.Data.Items;
            ViewBag.Keyword = keyword;
            ViewBag.Status = request.status.ToString();
            ViewBag.LStatus = SeletectStatus(request.status.ToString());
            var appointment = new AppointmentVm();
            if (Id!= null)
            {
                 appointment = (await _appointmentApiClient.GetById(Id.Value)).Data;
            }
            else{
                var first = appointments.Data.Items.FirstOrDefault();
                if(first == null) return View();
                 appointment = (await _appointmentApiClient.GetById(first.Id)).Data;
            }
            
            return View(appointment);
        }
        public List<SelectListItem> SeletectStatus(string? id)
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                new SelectListItem(text: "Quá hạn", value: StatusAppointment.pending.ToString()),
                new SelectListItem(text: "Chờ khám", value: StatusAppointment.approved.ToString()),
                new SelectListItem(text: "Đã khám", value: StatusAppointment.complete.ToString()),
                new SelectListItem(text: "Hủy bỏ", value: StatusAppointment.cancel.ToString()),
                new SelectListItem(text: "Tất cả", value: "")
            };
            var rs = gender.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = id == x.Value
            }).ToList();
            return rs;
        }
        [HttpGet]
        public IActionResult RatingTitle(int rating)
        {
            var ratings = ListRatingTitle().Where(x=>x.Value == rating.ToString());

            return Json(ratings);
        }
        public List<SelectListItem> ListRatingTitle()
        {
            var ratings = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Chất lượng khám rất kém", Value="0" },
                new SelectListItem() { Text = "Độ thân thiện rất kém", Value="0" },
                new SelectListItem() { Text = "Phục vụ bệnh nhân rất kém", Value="0" },
                new SelectListItem() { Text = "Rất không đáng tiền", Value="0" },
                new SelectListItem() { Text = "Thời gian làm việc rất chậm", Value="0" },
                new SelectListItem() { Text = "Chất lượng khám kém", Value="1" },
                new SelectListItem() { Text = "Độ thân thiện kém", Value="1" },
                new SelectListItem() { Text = "Phục vụ bệnh nhân kém", Value="1" },
                new SelectListItem() { Text = "Không đáng tiền", Value="1" },
                new SelectListItem() { Text = "Thời gian làm việc chậm", Value="1" },
                new SelectListItem() { Text = "Chất lượng khám tạm được", Value="2" },
                new SelectListItem() { Text = "Độ thân thiện tạm được", Value="2" },
                new SelectListItem() { Text = "Phục vụ bệnh nhân tạm được", Value="2" },
                new SelectListItem() { Text = "Tạm chấp nhận đồng tiền", Value="2" },
                new SelectListItem() { Text = "Thời gian làm việc tạm được", Value="2" },
                new SelectListItem() { Text = "Chất lượng khám tốt", Value="3" },
                new SelectListItem() { Text = "Bác sĩ thân thiện", Value="3" },
                new SelectListItem() { Text = "Phục vụ bệnh nhân tốt", Value="3" },
                new SelectListItem() { Text = "Đáng đồng tiền", Value="3" },
                new SelectListItem() { Text = "Thời gian làm việc nhanh", Value="3" },
                new SelectListItem() { Text = "Chất lượng khám tuyệt vời", Value="4" },
                new SelectListItem() { Text = "Bác sĩ rất thân thiện", Value="4" },
                new SelectListItem() { Text = "Phục vụ bệnh nhân rất tốt", Value="4" },
                new SelectListItem() { Text = "Rất đáng tiền", Value="4" },
                new SelectListItem() { Text = "Thời gian làm việc rất nhanh", Value="4" },
            };
            return ratings;
        }
        [HttpGet]
        public async Task<IActionResult> CanceledAppointment(Guid Id)
        {
            var cancelRequest = new AppointmentCancelRequest()
            {
                Id = Id,
                CancelReason = "Bận đột xuất",
                Checked = "patient"
            };
            var result = await _appointmentApiClient.CanceledAppointment(cancelRequest);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".CanceledAppointment",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(cancelRequest),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
            }
            else
            {
                TempData["AlertMessage"] = "Hủy lịch khám thành công.";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";

            }
            return RedirectToAction("Appointment");
        }
        [HttpPost]
        public async Task<IActionResult> AddRate(RateCreateRequest request)
        {
            if (!ModelState.IsValid) { return RedirectToAction("Appointment", new {id=request.AppointmentId}); }
            var result = await _appointmentApiClient.AddRate(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".AddRate",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
            }
            else
            {
                TempData["AlertMessage"] = "Đáng giá thành công.";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";

            }
            return RedirectToAction("Appointment", new { id = request.AppointmentId });
        }
    }
}
