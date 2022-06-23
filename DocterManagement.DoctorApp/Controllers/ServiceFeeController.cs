using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ServiceFeeController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IAnnualServiceFeeApiClient _annualServiceFeeApiClient;

        public ServiceFeeController(IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IAnnualServiceFeeApiClient annualServiceFeeApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _annualServiceFeeApiClient = annualServiceFeeApiClient;
        }
        public async Task<IActionResult> Index(string keyword , int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetAnnualServiceFeePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserName = User.Identity.Name
            };
         
            var data = await _annualServiceFeeApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;

            return View(data.Data);
        }
        public async Task<IActionResult> DetailtServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> PaymentServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.GetById(id);

            if (result.IsSuccessed)
            {
                ViewBag.ServiceFee = result.Data;
                var service = new AnnualServiceFeePaymentDoctorRequest()
                {
                    Id = result.Data.Id,
                    NeedToPay = result.Data.NeedToPay,
                    Contingency = result.Data.Contingency,
                };
                return View(service);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PaymentServiceFee([FromForm] AnnualServiceFeePaymentDoctorRequest request)
        {
            var GetById = await _annualServiceFeeApiClient.GetById(request.Id);
            ViewBag.ServiceFee = GetById.Data;
            if (!ModelState.IsValid) return View(request);
            var result = await _annualServiceFeeApiClient.PaymentServiceFeeDoctor(request);
            if (result.IsSuccessed)
            {
                return RedirectToAction("DetailtServiceFee", new { Id = request.Id });
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
