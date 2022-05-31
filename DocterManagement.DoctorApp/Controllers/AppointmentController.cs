using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;

        public AppointmentController(IAppointmentApiClient AppointmentApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _appointmentApiClient = AppointmentApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserNameDoctor = User.Identity.Name
            };
            var data = await _appointmentApiClient.GetAppointmentPagings(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateRequest request)
        {
            ViewBag.Location = await _locationApiClient.GetAllProvince(new Guid());
            if (!ModelState.IsValid)
                return View();

            var result = await _appointmentApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Đăt khám thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _appointmentApiClient.GetById(id);
            //var doctor = await _appointmentApiClient.Get
            if (result.IsSuccessed)
            {
                var Appointment = result.Data;

                //ViewBag.Status = SeletectStatus(Appointment.Status);

                var updateRequest = new AppointmentUpdateRequest()
                {
                    //Title = Appointment.Title,
                    Id = id,
                    //SortOrder = Appointment.SortOrder,
                    Status = Appointment.Status,
                    //Description = Appointment.Description
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppointmentUpdateRequest request)
        {

            //ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View();

            var result = await _appointmentApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> CreateMedicalRecord(Guid id)
        {
            var result = await _appointmentApiClient.GetById(id);
            //var doctor = await _appointmentApiClient.Get
            if (result.IsSuccessed)
            {
                ViewBag.Status = SeletectStatus(StatusIllness.Other);
                ViewBag.Appointment = result.Data;
                return View();
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> CreateMedicalRecord(MedicalRecordCreateRequest request)
        {
            ViewBag.Appointment = (await _appointmentApiClient.GetById(request.AppointmentId)).Data;
            ViewBag.Status = SeletectStatus(request.StatusIllness);
            if (!ModelState.IsValid)
                return View(request);
            var result = await _appointmentApiClient.CreateMedicalRecord(request);
            //var doctor = await _appointmentApiClient.Get
            if (result.IsSuccessed)
            {
                return RedirectToAction("DetailtAppointment", new {id = request.AppointmentId});
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> DetailtAppointment(Guid id)
        {
            var result = await _appointmentApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                //ViewBag.Status = appointmentData.Status == StatusAppointment.complete ? "Ngừng hoạt động" : appointmentData.Status == StatusAppointment.pending ? "Hoạt động" : "không hoạt động";
                
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        public List<SelectListItem> SeletectStatus(StatusIllness status)
        {
            List<SelectListItem> lstatus = new List<SelectListItem>()
            {
                new SelectListItem(text: "Nặng", value: StatusIllness.heavy.ToString()),
                new SelectListItem(text: "Vừa", value: StatusIllness.central.ToString()),
                new SelectListItem(text: "Nhẹ", value: StatusIllness.light.ToString()),
                new SelectListItem(text: "Khác", value: StatusIllness.Other.ToString())
            };
            var rs = lstatus.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = status.ToString() == x.Value
            }).ToList();
            return rs;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _appointmentApiClient.Delete(Id);
            return Json(new { response = result });
        }

        [HttpPost]
        public async Task<IActionResult> CanceledAppointment(AppointmentCancelRequest request)
        {
            if (!ModelState.IsValid) return RedirectToAction("DetailtAppointment", new { id = request.Id });
            var result = await _appointmentApiClient.CanceledAppointment(request);
            if (result.IsSuccessed)
            {
                return RedirectToAction("DetailtAppointment", new {id = request.Id});
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
