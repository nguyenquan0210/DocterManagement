using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    //[Authorize]
    public class BookingController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IAppointmentApiClient _appointmentApiClient;
        public BookingController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient, IScheduleApiClient scheduleApiClient, IAppointmentApiClient appointmentApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
            _scheduleApiClient = scheduleApiClient;
            _appointmentApiClient = appointmentApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BookingDoctorSetDate()
        {
            return View();
        }
        public async Task<IActionResult> BookingDoctorSetPatient(Guid doctorid, Guid scheduleid)
        {
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            ViewBag.Doctor = (await _doctorApiClient.GetById(doctorid)).Data;
            var getScheduleDoctor = (await _scheduleApiClient.GetScheduleDoctor(doctorid)).Data.ToList();
            ViewBag.GetScheduleDoctor = getScheduleDoctor;
            var slot = await _scheduleApiClient.GetByScheduleSlotId(scheduleid);
            if(slot.IsSuccessed)
            {
                ViewBag.Date = slot.Data.Schedule.CheckInDate;
                ViewBag.TimeSpan = slot.Data.FromTime.ToString().Substring(0, 5) + "-" + slot.Data.ToTime.ToString().Substring(0, 5);
            }
           
            var appointmentCreate = new AppointmentCreateRequest()
            {
                DoctorId = doctorid,
                SchedulesSlotId = scheduleid 
            };
            return View(appointmentCreate);
        }
        [HttpPost]
        public async Task<IActionResult> BookingDoctorSetPatient(AppointmentCreateRequest request)
        {
            
            ViewBag.Patient = (await _doctorApiClient.GetPatientProfile("0373951042")).Data;
            ViewBag.Doctor = (await _doctorApiClient.GetById(request.DoctorId)).Data;
            var getScheduleDoctor = (await _scheduleApiClient.GetScheduleDoctor(request.DoctorId)).Data.ToList();
            ViewBag.GetScheduleDoctor = getScheduleDoctor;
            var slot = await _scheduleApiClient.GetByScheduleSlotId(request.SchedulesSlotId);
            if (slot.IsSuccessed)
            {
                ViewBag.Date = slot.Data.Schedule.CheckInDate;
                ViewBag.TimeSpan = slot.Data.FromTime.ToString().Substring(0, 5) + "-" + slot.Data.ToTime.ToString().Substring(0, 5);
            }
            if (!ModelState.IsValid) View(request);
            var result = await _appointmentApiClient.Create(request);
            if (result.IsSuccessed)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(request);
        }
        public IActionResult BookingClinicSetDoctor()
        {
            return View();
        }
        public IActionResult BookingClinicSetDate()
        {
            return View();
        }
        public IActionResult BookingClinicSetTime()
        {
            return View();
        }
        public IActionResult BookingClinicSetPatient()
        {
            return View();
        }
    }
}
