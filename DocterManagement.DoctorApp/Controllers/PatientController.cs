using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class PatientController : BaseController
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IMedicineApiClient _medicineApiClient;
        private readonly IServiceApiClient _serviceApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Patient";

        public PatientController(IAppointmentApiClient AppointmentApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IMedicineApiClient medicineApiClient, IServiceApiClient serviceApiClient, IDoctorApiClient doctorApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _appointmentApiClient = AppointmentApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _medicineApiClient = medicineApiClient;
            _serviceApiClient = serviceApiClient;
            _doctorApiClient = doctorApiClient;
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
        public async Task<IActionResult> Index(string keyword,  int pageIndex = 1, int pageSize = 10)
        {
          
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserNameDoctor = User.Identity.Name,
            };
            var data = await _appointmentApiClient.GetAppointmentPagingPatient(request);
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
        public async Task<IActionResult> Detailt(Guid Id, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var patient = await _doctorApiClient.GetByPatientId(Id);
            if (!patient.IsSuccessed) return RedirectToAction("Error", "Home");
            var request = new GetAppointmentPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserNameDoctor = User.Identity.Name,
                PatientId = Id,
            };
            var data = await _appointmentApiClient.GetAppointmentPagings(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            ViewBag.Keyword = keyword;
            ViewBag.Appointment = data.Data;

            return View(patient.Data);
        }
    }
}
