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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] SpecialityCreateRequest request)
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
            
            if (result.IsSuccessed)
            {
                var speciality = result.Data;
               
                ViewBag.Image = speciality.Img;
                
                var updateRequest = new SpecialityUpdateRequest()
                {
                    Title = speciality.Title,
                    Id = id,
                    SortOrder = speciality.SortOrder,
                    IsDeleted = speciality.IsDeleted,
                    Description = speciality.Description,
                    
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] SpecialityUpdateRequest request)
        {
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
        
        public async Task<IActionResult> DetailtSpeciality(Guid id)
        {
            var result = await _specialityApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
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
