using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IMedicineApiClient _medicineApiClient;
        private readonly IServiceApiClient _serviceApiClient;

        public AppointmentController(IAppointmentApiClient AppointmentApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IMedicineApiClient medicineApiClient, IServiceApiClient serviceApiClient)
        {
            _appointmentApiClient = AppointmentApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _medicineApiClient = medicineApiClient;
            _serviceApiClient = serviceApiClient;
        }

        public async Task<IActionResult> Index(string keyword, StatusAppointment? status,string? check, int pageIndex = 1, int pageSize = 10)
        {
            if(check== null&&keyword==null)
            {
                status = StatusAppointment.approved;
            }
            var request = new GetAppointmentPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                UserNameDoctor = User.Identity.Name,
                status =  status
            }; 
            var data = await _appointmentApiClient.GetAppointmentPagings(request);
            ViewBag.Keyword = keyword;
            ViewBag.Status = request.status.ToString();
            ViewBag.LStatus = SeletectStatus(request.status.ToString());

            return View(data.Data);
        }
        public List<SelectListItem> SeletectStatus(string? id)
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                new SelectListItem(text: "Quá hạn", value: StatusAppointment.pending.ToString()),
                new SelectListItem(text: "Chờ khám", value: StatusAppointment.approved.ToString()),
                new SelectListItem(text: "Đã khám", value: StatusAppointment.complete.ToString()),
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
        public async Task<List<SelectListItem>> SeletectMedicine(string? id)
        {
            var medicine = await _medicineApiClient.GetAll(User.Identity.Name);
            var rs = medicine.Data.Select(x=> new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = id == x.Id.ToString()
            }).ToList(); 
            
            return rs;
        }
        public async Task<List<SelectListItem>> SeletectService(string? id)
        {
            var service = await _serviceApiClient.GetAll(User.Identity.Name);
            var rs = service.Data.Select(x => new SelectListItem()
            {
                Text = x.ServiceName,
                Value = x.Id.ToString(),
                Selected = id == x.Id.ToString()
            }).ToList();

            return rs;
        }
        [HttpGet]
        public async Task<IActionResult> CreateMedicalRecord(Guid id)
        {
            var session = HttpContext.Session.GetString(SystemConstants.Medicine);
            var currentMedicine = JsonConvert.DeserializeObject<List<MedicineCreate>>(session == null ? JsonConvert.SerializeObject(new List<MedicineCreate>()) : session);
            currentMedicine = currentMedicine.Where(x => x.AppointmentId == id).ToList();
            ViewBag.Medicine = currentMedicine;
            session = HttpContext.Session.GetString(SystemConstants.Service);
            var currentService = JsonConvert.DeserializeObject<List<ServiceCreate>>(session == null ? JsonConvert.SerializeObject(new List<ServiceCreate>()) : session);
            currentService = currentService.Where(x => x.AppointmentId == id).ToList();
            ViewBag.Service = currentService;
            var result = await _appointmentApiClient.GetById(id);
            ViewBag.MedicineSeleted = await SeletectMedicine("");
            ViewBag.ServiceSeleted = await SeletectService("");
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
            //HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));
            var session = HttpContext.Session.GetString(SystemConstants.Medicine);
            var currentMedicine = JsonConvert.DeserializeObject<List<MedicineCreate>>(session == null ? JsonConvert.SerializeObject(new List<MedicineCreate>()) : session);
            currentMedicine = currentMedicine.Where(x => x.AppointmentId == request.AppointmentId).ToList();
            ViewBag.Medicine = currentMedicine;
            session = HttpContext.Session.GetString(SystemConstants.Service);
            var currentService = JsonConvert.DeserializeObject<List<ServiceCreate>>(session == null ? JsonConvert.SerializeObject(new List<ServiceCreate>()) : session);
            currentService = currentService.Where(x => x.AppointmentId == request.AppointmentId).ToList();
            ViewBag.Service = currentService;
            ViewBag.MedicineSeleted = await SeletectMedicine("");
            ViewBag.ServiceSeleted = await SeletectService("");
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
        [HttpPost]
        public async Task<IActionResult> AddMedicine(MedicineCreate create)
        {
            if (ModelState.IsValid)
            {
                var rsmedicine = (await _medicineApiClient.GetById(create.MedicineId)).Data;
                var session = HttpContext.Session.GetString(SystemConstants.Medicine);
                var currentMedicine = JsonConvert.DeserializeObject<List<MedicineCreate>>(session == null ? JsonConvert.SerializeObject(new List<MedicineCreate>()) : session);
                var checkMedicine = currentMedicine.FirstOrDefault(x=>x.MedicineId == create.MedicineId);
                if (checkMedicine != null) currentMedicine = currentMedicine.Where(x => x.MedicineId != create.MedicineId).ToList();
                var medicine = new MedicineCreate()
                {
                    Afternoon = create.Afternoon,
                    MedicineId = rsmedicine.Id,
                    Morning = create.Morning,
                    Name = rsmedicine.Name,
                    Night = create.Night,
                    Noon = create.Noon,
                    Price = rsmedicine.Price,
                    TotalAmount = rsmedicine.Price * create.Qty,
                    TotalAmountString = (rsmedicine.Price * create.Qty).ToString("#,###,### vnđ"),
                    Qty = create.Qty,
                    Unit = rsmedicine.Unit,
                    Use = create.Use,
                    AppointmentId = create.AppointmentId,
                };
                currentMedicine.Add(medicine);
                currentMedicine = currentMedicine.Where(x => x.AppointmentId == create.AppointmentId).ToList();
                ViewBag.Medicine = currentMedicine;
                HttpContext.Session.SetString(SystemConstants.Medicine, JsonConvert.SerializeObject(currentMedicine));
                return Json(currentMedicine);
            }
            return null;
        }
        [HttpPost]
        public async Task<IActionResult> AddService(ServiceCreate create)
        {
            if (ModelState.IsValid)
            {
                var rsService = (await _serviceApiClient.GetById(create.ServiceId)).Data;
                var session = HttpContext.Session.GetString(SystemConstants.Service);
                var currentService = JsonConvert.DeserializeObject<List<ServiceCreate>>(session == null ? JsonConvert.SerializeObject(new List<ServiceCreate>()) : session);
                var checkService = currentService.FirstOrDefault(x => x.ServiceId == create.ServiceId);
                if (checkService != null) currentService = currentService.Where(x => x.ServiceId != create.ServiceId).ToList();
                var Service = new ServiceCreate()
                {
                    ServiceId = rsService.Id,
                    Name = rsService.ServiceName,
                    Price = rsService.Price,
                    TotalAmount = rsService.Price * create.Qty,
                    TotalAmountString = (rsService.Price * create.Qty).ToString("#,###,### vnđ"),
                    Qty = create.Qty,
                    Unit = rsService.Unit,
                    AppointmentId = create.AppointmentId,
                };
                currentService.Add(Service);
                currentService = currentService.Where(x => x.AppointmentId == create.AppointmentId).ToList();
                ViewBag.Service = currentService;
                HttpContext.Session.SetString(SystemConstants.Service, JsonConvert.SerializeObject(currentService));
                return Json(currentService);
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> RemoveMedicine(Guid Id)
        {
            
                var session = HttpContext.Session.GetString(SystemConstants.Medicine);
                var currentMedicine = JsonConvert.DeserializeObject<List<MedicineCreate>>(session == null ? JsonConvert.SerializeObject(new List<MedicineCreate>()) : session);
                var checkMedicine = currentMedicine.FirstOrDefault(x => x.MedicineId == Id);
                if (checkMedicine != null) currentMedicine = currentMedicine.Where(x => x.MedicineId != Id).ToList();
                
                ViewBag.Medicine = currentMedicine;
                HttpContext.Session.SetString(SystemConstants.Medicine, JsonConvert.SerializeObject(currentMedicine));
                return Json(true);
            
        }
        [HttpGet]
        public async Task<IActionResult> RemoveService(Guid Id)
        {
                var session = HttpContext.Session.GetString(SystemConstants.Service);
                var currentService = JsonConvert.DeserializeObject<List<ServiceCreate>>(session == null ? JsonConvert.SerializeObject(new List<ServiceCreate>()) : session);
                var checkService = currentService.FirstOrDefault(x => x.ServiceId == Id);
                if (checkService != null) currentService = currentService.Where(x => x.ServiceId != Id).ToList();
                ViewBag.Service = currentService;
                HttpContext.Session.SetString(SystemConstants.Service, JsonConvert.SerializeObject(currentService));
                return Json(true);
           
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
        [HttpGet]
        public async Task<IActionResult> PrintAppointment(Guid id)
        {
            var result = await _appointmentApiClient.GetById(id);
            if (result.IsSuccessed)
            {
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
