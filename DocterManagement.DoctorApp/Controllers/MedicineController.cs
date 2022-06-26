using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Medicine;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class MedicineController : BaseController
    {
        private readonly IMedicineApiClient _medicineApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Medicine";

        public MedicineController(IMedicineApiClient medicineApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IUserApiClient userApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _medicineApiClient = medicineApiClient;
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

            var request = new GetMedicinePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
            var data = await _medicineApiClient.GetAllPaging(request);
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
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
            ViewBag.ParentId = result.Data.DoctorVm.GetClinic.Id == new Guid() ? result.Data.DoctorVm.UserId : result.Data.DoctorVm.GetClinic.Id;
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Create",
                MethodName = "Get",
                ExtraProperties =  "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Name + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

           
            return View(request);
        }

        public async Task<IActionResult> DetailtMedicine(Guid id)
        {
            var result = await _medicineApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DetailtMedicine",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _medicineApiClient.Delete(Id);
            return Json(new { response = result });
        }

    }
}
