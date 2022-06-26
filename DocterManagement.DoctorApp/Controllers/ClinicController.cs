using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ClinicController : BaseController
    {
        private readonly IClinicApiClient _clinicApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Clinic";
        public ClinicController(IClinicApiClient clinicApiClient, IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _clinicApiClient = clinicApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _locationApiClient = locationApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);

            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name,
                    Usertemporary = User.Identity.Name,
                    Type = "doctor",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var user = await _userApiClient.GetByUserName(User.Identity.Name);
            var result = await _clinicApiClient.GetById(user.Data.DoctorVm.GetClinic.Id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            //var doctor = await _ClinicApiClient.Get
            if (result.IsSuccessed)
            {
                var clinic = result.Data;
                ViewBag.District = await _locationApiClient.GetAllDistrict(clinic.LocationVm.District.Id);
                ViewBag.Location = await _locationApiClient.GetAllSubDistrict(clinic.LocationVm.Id, clinic.LocationVm.District.Id);
                ViewBag.Img = clinic.ImgLogo;
                ViewBag.Imgs = clinic.Images;
                ViewBag.Status = SeletectStatus(clinic.Status);
                //ViewBag.Speciality = await _ClinicApiClient.GetAllSpeciality(Clinic.DoctorVm.GetSpeciality.Id);
                var updateRequest = new ClinicUpdateRequest()
                {
                    Name = clinic.Name,
                    Id = clinic.Id,
                    Address = clinic.Address,
                    Status = clinic.Status,
                    Description = clinic.Description,
                    LocationId = clinic.LocationVm.Id,
                    DistrictId = clinic.LocationVm.District.Id,
                    Images = clinic.Images,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
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

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] ClinicUpdateRequest request)
        {
            ViewBag.District = await _locationApiClient.GetAllDistrict(request.DistrictId);
            ViewBag.Location = await _locationApiClient.GetAllSubDistrict(request.LocationId, request.DistrictId);
            ViewBag.Img = request.ImgLogo;
            ViewBag.Imgs = request.ImgClinics;
            ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View();

            var result = await _clinicApiClient.Update(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "Post",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Name + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Detailt");
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt()
        {
            var user = await _userApiClient.GetByUserName(User.Identity.Name);
            if(user.Data.DoctorVm.IsPrimary == false) return RedirectToAction("Error", "Home");
            var result = await _clinicApiClient.GetById(user.Data.DoctorVm.GetClinic.Id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Detailt",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                var clinicdata = result.Data;
                ViewBag.Img = clinicdata.ImgLogo;
                ViewBag.Imgs = clinicdata.Images;
                ViewBag.Status = clinicdata.Status == Status.NotActivate ? "Ngừng hoạt động" : clinicdata.Status == Status.Active ? "Hoạt động" : "không hoạt động";
               
                return View(clinicdata);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImg(Guid imgId)
        {
            var result = await _clinicApiClient.DeleteImg(imgId);
            return Json(new { response = result });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAllImg(Guid clinicId)
        {
            var result = await _clinicApiClient.DeleteAllImg(clinicId);
            return Json(new { response = result });
        }
    }
}
