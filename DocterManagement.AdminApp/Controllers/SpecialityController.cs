using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class SpecialityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
