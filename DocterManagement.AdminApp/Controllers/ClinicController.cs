using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc;

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
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());

            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ClinicCreateRequest request)
        {
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());
            if (!ModelState.IsValid)
                return View();

            var result = await _clinicApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới người dùng thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }
    }
}
