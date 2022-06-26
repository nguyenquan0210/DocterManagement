using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ServiceFeeController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IAnnualServiceFeeApiClient _annualServiceFeeApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Service";

        public ServiceFeeController(IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IAnnualServiceFeeApiClient annualServiceFeeApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _annualServiceFeeApiClient = annualServiceFeeApiClient;
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
        public async Task<IActionResult> Index(string keyword , int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetAnnualServiceFeePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
         
            var data = await _annualServiceFeeApiClient.GetAllPaging(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            ViewBag.Keyword = keyword;

            return View(data.Data);
        }
        public async Task<IActionResult> DetailtServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DetailtServiceFee",
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
        [HttpGet]
        public async Task<IActionResult> PaymentServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".PaymentServiceFee",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{id: " + id + "}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                ViewBag.ServiceFee = result.Data;
                var service = new AnnualServiceFeePaymentDoctorRequest()
                {
                    Id = result.Data.Id,
                    NeedToPay = result.Data.NeedToPay,
                    Contingency = result.Data.Contingency,
                };
                return View(service);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PaymentServiceFee([FromForm] AnnualServiceFeePaymentDoctorRequest request)
        {
            var GetById = await _annualServiceFeeApiClient.GetById(request.Id);
            ViewBag.ServiceFee = GetById.Data;
            if (!ModelState.IsValid) return View(request);
            var result = await _annualServiceFeeApiClient.PaymentServiceFeeDoctor(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".PaymentServiceFee",
                MethodName = "Post",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                return RedirectToAction("DetailtServiceFee", new { Id = request.Id });
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
