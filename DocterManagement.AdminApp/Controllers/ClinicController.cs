using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Clinic;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class ClinicController :  BaseController
    {
        private readonly IClinicApiClient _clinicApiClient;
        private readonly IConfiguration _configuration;

        public ClinicController(IClinicApiClient clinicApiClient,
            IConfiguration configuration)
        {
            _clinicApiClient = clinicApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
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
