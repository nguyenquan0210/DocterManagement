using AutoMapper;
using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    public class ClinicController :  BaseController
    {
        private readonly IClinicApiClient _clinicApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;

        public ClinicController(IClinicApiClient clinicApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _clinicApiClient = clinicApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.District = await _locationApiClient.GetAllDistrict(new Guid());
            ViewBag.Location = new List<SelectListItem>();
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ClinicCreateRequest request)
        {
            ViewBag.District = await _locationApiClient.GetAllDistrict(request.DistrictId);
            ViewBag.Location = await _locationApiClient.GetAllSubDistrict(request.LocationId,request.DistrictId);

            if (!ModelState.IsValid)
                return View(request);

            var result = await _clinicApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Name + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }
           
          
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _clinicApiClient.GetById(id);
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
                    Id = id,
                    Address = clinic.Address,
                    Status = clinic.Status,
                    Description = clinic.Description,
                    LocationId = clinic.LocationVm.Id,
                    DistrictId = clinic.LocationVm.District.Id,
                    Images = clinic.Images,
                    MapUrl = clinic.MapUrl
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
            ViewBag.Imgs = request.Images;
            ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View(request);

            var result = await _clinicApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Name + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

          
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt(Guid id)
        {
            var result = await _clinicApiClient.GetById(id);
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
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _clinicApiClient.Delete(Id);
            return Json(new { response = result });
        }
    }
}
