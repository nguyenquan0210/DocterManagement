using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IServiceApiClient _serviceApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Service";

        public ServiceController(IServiceApiClient serviceApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IUserApiClient userApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _serviceApiClient = serviceApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _userApiClient = userApiClient;
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Create",
                MethodName = "Get",
                ExtraProperties = "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Create",
                MethodName = "Post",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{id: " + id + "}",
            };
            await HistoryActive(historyactive);
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
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.ServiceName + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            
            return View(request);
        }

        public async Task<IActionResult> DetailtService(Guid id)
        {
            var result = await _serviceApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DetailtMedicine",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{id: " + id + "}",
            };
            await HistoryActive(historyactive);
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
