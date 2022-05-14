using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly ISpecialityApiClient _specialityApiClient;

        public DoctorController(ILogger<HomeController> logger, IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            ISpecialityApiClient specialityApiClient)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _doctorApiClient = doctorApiClient;
            _specialityApiClient = specialityApiClient;
        }
        public async Task<IActionResult> Index(Guid Id)
        {

            var doctor = await _doctorApiClient.GetById(Id);
            if (!doctor.IsSuccessed)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(doctor.Data);
        }
    }
}
