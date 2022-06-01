using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Service;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceApiClient _serviceApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IUserApiClient _userApiClient;

        public ServiceController(IServiceApiClient serviceApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IUserApiClient userApiClient)
        {
            _serviceApiClient = serviceApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {

            var request = new GetServicePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
            var data = await _serviceApiClient.GetAllPaging(request);
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
            var user = await _userApiClient.GetByUserName(User.Identity.Name);
            ViewBag.ParentId = user.Data.Id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ServiceCreateRequest request)
        {
            var user = await _userApiClient.GetByUserName(User.Identity.Name);
            ViewBag.ParentId = user.Data.Id;
            if (!ModelState.IsValid)
                return View();

            var result = await _serviceApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.ServiceName + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _serviceApiClient.GetById(id);

            if (result.IsSuccessed)
            {
                var Service = result.Data;

                var updateRequest = new ServiceUpdateRequest()
                {
                    ServiceName = Service.ServiceName,
                    Id = id,
                    IsDeleted = Service.IsDeleted,
                    Description = Service.Description,
                    Price = Service.Price,
                    Unit = Service.Unit
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(ServiceUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _serviceApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.ServiceName + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            
            return View(request);
        }

        public async Task<IActionResult> DetailtService(Guid id)
        {
            var result = await _serviceApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _serviceApiClient.Delete(Id);
            return Json(new { response = result });
        }

    }
}
