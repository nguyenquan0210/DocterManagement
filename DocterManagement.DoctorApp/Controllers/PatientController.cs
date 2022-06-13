using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Appointment;
using Microsoft.AspNetCore.Mvc;

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

        public PatientController(IAppointmentApiClient AppointmentApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IMedicineApiClient medicineApiClient, IServiceApiClient serviceApiClient, IDoctorApiClient doctorApiClient)
        {
            _appointmentApiClient = AppointmentApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _medicineApiClient = medicineApiClient;
            _serviceApiClient = serviceApiClient;
            _doctorApiClient = doctorApiClient;
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
            ViewBag.Keyword = keyword;
            ViewBag.Appointment = data.Data;

            return View(patient.Data);
        }
    }
}
