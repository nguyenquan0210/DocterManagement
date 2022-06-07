using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IClinicApiClient _clinicApiClient;
        public ClinicController(IClinicApiClient clinicApiClient)
        {
            _clinicApiClient = clinicApiClient;
        }
        public async Task<IActionResult> Index(Guid Id)
        {
            var result = await _clinicApiClient.GetById(Id);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message==null? result.ValidationErrors[1] : result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return RedirectToAction("Index", "Home");
            }
            return View(result.Data);
        }
    }
}
