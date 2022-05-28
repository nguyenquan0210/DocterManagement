using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Drawing;

namespace DoctorManagement.AdminApp.Controllers
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
        public async Task<IActionResult> ServiceFeePaging(string keyword, string rolename, int pageIndex = 1, int pageSize = 10)
        {
            /* if (ViewBag.Role != null)
             {
                 rolename = ViewBag.Role;
             }
             if (rolename == null)
             {
                 rolename = "all";
             }
             ViewBag.rolename = SeletectRole(rolename);*/

            var request = new GetAnnualServiceFeePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                //RoleName = rolename
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
        [HttpPost]
        public async Task<IActionResult> CanceledServiceFee(AnnualServiceFeeCancelRequest request)
        {
                if(!ModelState.IsValid) return RedirectToAction("DetailtServiceFee", new {Id = request.Id});
                var result = await _annualServiceFeeApiClient.CanceledServiceFee(request);
                if (result.IsSuccessed)
                {
                    return RedirectToAction("ServiceFeePaging");
                }
                return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ApprovedServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.ApprovedServiceFee(id);
            if (result.IsSuccessed)
            {
                return RedirectToAction("ServiceFeePaging");
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
                var service = new AnnualServiceFeePaymentRequest()
                {
                    Id = result.Data.Id,
                };
                return View(service);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> PaymentServiceFee(AnnualServiceFeePaymentRequest request)
        {
            var GetById = await _annualServiceFeeApiClient.GetById(request.Id);
            ViewBag.ServiceFee = GetById.Data;
            if (!ModelState.IsValid) return View(request);
            var result = await _annualServiceFeeApiClient.PaymentServiceFee(request);
            if (result.IsSuccessed)
            {
                return RedirectToAction("DetailtServiceFee" , new {Id = request.Id});
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> PaymentServiceFeePDF(Guid Id)
        {
            var result = await _annualServiceFeeApiClient.GetById(Id);
            if (result.IsSuccessed)
            {
                var pdf = new ViewAsPdf()
                {
                    PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 20 },
                    Model = result.Data,
                    ViewName = "~/Views/ServiceFee/PaymentServiceFeePDF.cshtml",
                    IsGrayScale = true,
                };
                return pdf;
                //return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
           
        }
    }
}
