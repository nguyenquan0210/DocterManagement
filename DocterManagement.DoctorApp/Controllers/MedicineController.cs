using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Medicine;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class MedicineController : BaseController
    {
        private readonly IMedicineApiClient _medicineApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IUserApiClient _userApiClient;

        public MedicineController(IMedicineApiClient medicineApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IUserApiClient userApiClient)
        {
            _medicineApiClient = medicineApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient; 
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {

            var request = new GetMedicinePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
            var data = await _medicineApiClient.GetAllPaging(request);
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
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
            ViewBag.ParentId = result.Data.DoctorVm.GetClinic.Id == new Guid() ? result.Data.DoctorVm.UserId : result.Data.DoctorVm.GetClinic.Id;
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] MedicineCreateRequest request)
        {
            var user = await _userApiClient.GetByUserName(User.Identity.Name);
            ViewBag.ParentId = user.Data.DoctorVm.GetClinic.Id == new Guid() ? user.Data.DoctorVm.UserId : user.Data.DoctorVm.GetClinic.Id;
            if (!ModelState.IsValid)
                return View();

            var result = await _medicineApiClient.Create(request);

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
            var result = await _medicineApiClient.GetById(id);

            if (result.IsSuccessed)
            {
                var Medicine = result.Data;

                ViewBag.Image = Medicine.Image;

                var updateRequest = new MedicineUpdateRequest()
                {
                    Name = Medicine.Name,
                    Id = id,
                    IsDeleted = Medicine.IsDeleted,
                    Description = Medicine.Description,
                    Price = Medicine.Price,
                    Unit = Medicine.Unit,
                    ImageText = Medicine.Image,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] MedicineUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _medicineApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Name + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

           
            return View(request);
        }

        public async Task<IActionResult> DetailtMedicine(Guid id)
        {
            var result = await _medicineApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _medicineApiClient.Delete(Id);
            return Json(new { response = result });
        }

    }
}
