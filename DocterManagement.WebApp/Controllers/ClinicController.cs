using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.WebApp.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IClinicApiClient _clinicApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Clinic";

        public ClinicController(IClinicApiClient clinicApiClient, IStatisticApiClient statisticApiClient)
        {
            _clinicApiClient = clinicApiClient;
            _statisticApiClient = statisticApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? usertemporary = null;
            string? user = null;
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);
                usertemporary = currentHistory.Usertemporary;
                user = currentHistory.User;
            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name == null ? user : User.Identity.Name,
                    Usertemporary = (usertemporary == null && User.Identity.Name == null) ? ("patient" + new Random().Next(10000000, 99999999) + new Random().Next(10000000, 99999999)) : (usertemporary == null ? User.Identity.Name : usertemporary),
                    Type = user == null ? "passersby" : "patient",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        public async Task<IActionResult> Index(Guid Id)
        {
            var result = await _clinicApiClient.GetById(Id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed?"success":"error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message==null? result.ValidationErrors[1] : result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return RedirectToAction("Index", "Home");
            }
            result.Data.RatingText = SetCount(int.Parse(result.Data.RatingText));
            return View(result.Data);
        }
        public string SetCount(int count)
        {
            switch (count)
            {
                case >= 1000 and < 1000000:
                    return count / 1000 + "K";
                case >= 1000000 and < 1000000000:
                    return count / 1000000 + "M";
                case >= 1000000000:
                    return count / 1000000000 + "B";
                default:
                    return count.ToString();
            }
        }
    }
}
