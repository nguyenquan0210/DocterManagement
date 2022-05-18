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
        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 15)
        {
            var request = new GetClinicPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _clinicApiClient.GetClinicPagings(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
    }
}
