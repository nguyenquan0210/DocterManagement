using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ClinicController : BaseController
    {
        private readonly IClinicApiClient _clinicApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly ILocationApiClient _locationApiClient;     

        public ClinicController(IClinicApiClient clinicApiClient, IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _clinicApiClient = clinicApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _locationApiClient = locationApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Update(string userName)
        {
            var user = await _userApiClient.GetByUserName(userName);
            var result = await _clinicApiClient.GetById(user.Data.DoctorVm.GetClinic.Id);
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
                    DistrictId = clinic.LocationVm.District.Id
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
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Name + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Update");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt(string userName)
        {
            var user = await _userApiClient.GetByUserName(userName);
            var result = await _clinicApiClient.GetById(user.Data.DoctorVm.GetClinic.Id);
            if (result.IsSuccessed)
            {
                var clinicdata = result.Data;
                ViewBag.Img = clinicdata.ImgLogo;
                ViewBag.Imgs = clinicdata.Images;
                ViewBag.Status = clinicdata.Status == Status.NotActivate ? "Ngừng hoạt động" : clinicdata.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var clinic = new ClinicVm()/*_mapper.Map<ClinicVm>(clinicdata);*/
                {
                    Name = clinicdata.Name,
                    Id = clinicdata.Id,
                    Description = clinicdata.Description,
                    Address = clinicdata.Address,
                    Status = clinicdata.Status, //== Status.Active ? true : false
                    DoctorVms = clinicdata.DoctorVms,
                    LocationVm = clinicdata.LocationVm,
                    No = clinicdata.No,
                };
                return View(clinic);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteImg(Guid imgId)
        {
            var result = await _clinicApiClient.DeleteImg(imgId);
            return Json(new { response = result });
        }
       
    }
}
