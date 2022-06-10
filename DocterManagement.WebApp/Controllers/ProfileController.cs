using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public ProfileController(IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IScheduleApiClient scheduleApiClient, IAppointmentApiClient appointmentApiClient,
            IClinicApiClient clinicApiClient, ILocationApiClient locationApiClient)
        {
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _scheduleApiClient = scheduleApiClient;
            _appointmentApiClient = appointmentApiClient;
            _clinicalApiClient = clinicApiClient;
            _locationApiClient = locationApiClient;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data;
        
            return View();
        }
        public async Task<IActionResult> Account()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data.FirstOrDefault(x=>x.IsPrimary);

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Account(ChangePasswordRequest request)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile(User.Identity.Name)).Data.FirstOrDefault(x => x.IsPrimary);

            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.ChangePassword(request);
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
        public async Task<IActionResult> Appointment(string keyword,Guid? Id, int pageIndex = 1, int pageSize = 15)
        {
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
            var appointments = await _appointmentApiClient.GetAppointmentPagings(request);
            ViewBag.Appointments = appointments.Data.Items;
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
            ViewBag.Keyword = keyword;
            return View(appointment);
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
                Checked = "pantient"
            };
            var result = await _appointmentApiClient.CanceledAppointment(cancelRequest);
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
