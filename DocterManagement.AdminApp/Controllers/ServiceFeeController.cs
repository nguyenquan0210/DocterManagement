using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
        public async Task<IActionResult> ServiceFeePaging(string keyword, StatusAppointment? status, string day, string month,string year, string check, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetAnnualServiceFeePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                day = day,
                month = month,
                year = year,
                status = status
            };
            if (check == "year"|| check==null)
            {
                request.year = year == null ? DateTime.Now.ToString("yyyy") : year;
                request.month = null;
                request.day = null;
                
                ViewBag.Statitic = JsonConvert.SerializeObject(await _annualServiceFeeApiClient.GetServiceFeeStatiticYear(request));
            }
            else if(check == "month")
            {
                request.day = null;
                request.month = month == null ? DateTime.Now.ToString("MM") : month;
                request.year = year == null ? DateTime.Now.ToString("yyyy") : year;
                ViewBag.Statitic = JsonConvert.SerializeObject(await _annualServiceFeeApiClient.GetServiceFeeStatiticMonth(request));
            }
            else
            {
                request.day = day == null ? DateTime.Now.ToString("dd") : day;
                request.month = month == null ? DateTime.Now.ToString("MM") : month;
                request.year = year == null ? DateTime.Now.ToString("yyyy") : year;
                ViewBag.Statitic = JsonConvert.SerializeObject(await _annualServiceFeeApiClient.GetServiceFeeStatiticDay(request));
            }
            var data = await _annualServiceFeeApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.Day = request.day == null ? DateTime.Now.ToString("dd") : request.day;
            ViewBag.Month = request.month == null ? DateTime.Now.ToString("MM") : request.month;
            ViewBag.Year = request.year;
            ViewBag.Status = request.status;
            ViewBag.Check = check;
            ViewBag.LStatus = SeletectStatus(request.status.ToString());
            ViewBag.Days = SeletectDay(request.day == null ? DateTime.Now.ToString("dd") : request.day);
            ViewBag.Months = SeletectMonth(request.month == null ? DateTime.Now.ToString("MM") : request.month);
            ViewBag.Years = SeletectYear(request.year);
            return View(data.Data);
        }
        public List<SelectListItem> SeletectStatus(string? id)
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
               new SelectListItem(text: "Chờ xử lý", value: StatusAppointment.pending.ToString()),
                new SelectListItem(text: "Chờ phê duyệt", value: StatusAppointment.approved.ToString()),
                new SelectListItem(text: "Hoàn thành", value: StatusAppointment.complete.ToString()),
                new SelectListItem(text: "Hủy bỏ", value: StatusAppointment.cancel.ToString()),
                new SelectListItem(text: "Tất cả", value: "")
            };
            var rs = gender.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = id == x.Value
            }).ToList();
            return rs;
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
