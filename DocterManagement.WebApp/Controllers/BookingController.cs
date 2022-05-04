using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult BookingDoctorSetDate()
        {
            return View();
        }
        public IActionResult BookingDoctorSetPatient()
        {
            return View();
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
