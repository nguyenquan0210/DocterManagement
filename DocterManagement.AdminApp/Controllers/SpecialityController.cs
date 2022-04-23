using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Speciality;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class SpecialityController : BaseController
    {
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;

        public SpecialityController(ISpecialityApiClient specialityApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _specialityApiClient = specialityApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetSpecialityPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _specialityApiClient.GetSpecialityPagings(request);
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
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create( SpecialityCreateRequest request)
        {
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());
            if (!ModelState.IsValid)
                return View();

            var result = await _specialityApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Title + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _specialityApiClient.GetById(id);
            //var doctor = await _specialityApiClient.Get
            if (result.IsSuccessed)
            {
                var speciality = result.Data;
               
                ViewBag.Status = SeletectStatus(speciality.Status);
                
                var updateRequest = new SpecialityUpdateRequest()
                {
                    Title = speciality.Title,
                    Id = id,
                    SortOrder = speciality.SortOrder,
                    Status = speciality.Status,
                    Description = speciality.Description
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(SpecialityUpdateRequest request)
        {
           
            ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View();

            var result = await _specialityApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Title + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt(Guid id)
        {
            var result = await _specialityApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var specialityData = result.Data;
                ViewBag.Status = specialityData.Status == Status.NotActivate ? "Ngừng hoạt động" : specialityData.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var Speciality = new SpecialityVm()/*_mapper.Map<SpecialityVm>(Specialitydata);*/
                {
                    Title = specialityData.Title,
                    Id = id,
                    No = specialityData.No,
                    Description = specialityData.Description,
                    Status = specialityData.Status, //== Status.Active ? true : false
                    SortOrder = specialityData.SortOrder
                };
                return View(Speciality);
            }
            return RedirectToAction("Error", "Home");
        }
       
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _specialityApiClient.Delete(Id);
            return Json(new { response = result });
        }
    }
}
