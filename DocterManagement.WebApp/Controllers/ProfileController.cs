using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.System.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.WebApp.Controllers
{
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
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
        
            return View();
        }
        public async Task<IActionResult> UpdateInfo(Guid Id)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
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
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(), request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(), request.DistrictId);
            if (!ModelState.IsValid) return View(request);
            var result = await _doctorApiClient.UpdateInfo(request);
            if (!result.IsSuccessed) return View(request);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddInfo()
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = new List<SelectListItem>();
            ViewBag.SubDistrict = new List<SelectListItem>();
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddInfo(AddPatientInfoRequest request)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(),request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(),request.DistrictId);
            if (!ModelState.IsValid) return View(request);
            var result = await _doctorApiClient.AddInfo(request);
            if(!result.IsSuccessed) return View(request);
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
        public async Task<IActionResult> Appointment(string keyword, int pageIndex = 1, int pageSize = 15)
        {
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
            var data = await _appointmentApiClient.GetAppointmentPagings(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
    }
}
