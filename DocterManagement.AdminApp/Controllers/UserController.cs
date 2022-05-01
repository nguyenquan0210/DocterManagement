using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        /* private readonly IRoleApiClient _roleApiClient;*/

        public UserController(IUserApiClient userApiClient,
            /*IRoleApiClient roleApiClient,*/
            IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            /*_roleApiClient = roleApiClient;*/
        }

        public async Task<IActionResult> Index(string keyword, string rolename, int pageIndex = 1, int pageSize = 10)
        {
            if (ViewBag.Role != null)
            {
                rolename = ViewBag.Role;
            }
            if (rolename == null)
            {
                rolename = "all";
            }
            ViewBag.rolename = SeletectRole(rolename);

            var request = new GetUserPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                RoleName = rolename
            };
            var data = await _userApiClient.GetUsersPagings(request);
            ViewBag.Keyword = keyword;
            if (rolename != null)
                ViewBag.Role = rolename;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        public List<SelectListItem> SeletectRole(string str)
        {
            List<SelectListItem> role = new List<SelectListItem>()
            {
                new SelectListItem(text: "Bác sĩ", value: "doctor"),
                new SelectListItem(text: "Bệnh nhân", value: "patient"),
                new SelectListItem(text: "Tất cả", value: "all"),
                new SelectListItem(text: "Quản trị", value: "admin")
            };
           
            var rs = role.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = str == x.Value
            }).ToList();
            return rs;
        }
        public async Task<IActionResult> ListRole()
        {
            var data = await _userApiClient.GetAllRole();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clinic = await _userApiClient.GetAllClinic(new Guid());
            ViewBag.Speciality = await _userApiClient.GetAllSpeciality(new Guid());
            ViewBag.Gender = SeletectGender("0");
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ManageRegisterRequest request)
        {
            ViewBag.Clinic = await _userApiClient.GetAllClinic(request.ClinicId);
            ViewBag.Speciality = await _userApiClient.GetAllSpeciality(request.SpecialityId);
            ViewBag.Gender = SeletectGender(request.Gender.ToString());
            if (!ModelState.IsValid)
                return View();
            
            var result = await _userApiClient.RegisterUser(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới người dùng thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            //var doctor = await _userApiClient.Get
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Gender = SeletectGender(user.Gender.ToString());
                ViewBag.Status = SeletectStatus(user.Status);
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Clinic = await _userApiClient.GetAllClinic(user.DoctorVm.GetClinic.Id);
                ViewBag.Speciality = await _userApiClient.GetAllSpeciality(user.DoctorVm.GetSpeciality.Id);
                var updateRequest = new UserUpdateRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Id = id,
                    Address = user.DoctorVm.Address,
                    Status = user.Status ,
                    SpecialityId = user.DoctorVm.GetSpeciality.Id,
                    ClinicId = user.DoctorVm.GetClinic.Id,
                    img = user.DoctorVm.Img,
                    Description = user.DoctorVm.Description
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            ViewBag.Img = request.img;
            ViewBag.Gender = SeletectGender(request.Gender.ToString());
            ViewBag.Status = SeletectStatus(request.Status);
            ViewBag.Clinic = await _userApiClient.GetAllClinic(request.ClinicId);
            ViewBag.Speciality = await _userApiClient.GetAllSpeciality(request.SpecialityId);
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.UpdateDoctor(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin người dùng thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        public List<SelectListItem> SeletectGender(string id)
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                new SelectListItem(text: "Nam", value: "0"),
                new SelectListItem(text: "Nữ", value: "1")
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
        public async Task<IActionResult> UpdateAdmin(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            //var doctor = await _userApiClient.Get
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Gender = SeletectGender(user.Gender.ToString());
                ViewBag.Status = SeletectStatus(user.Status);
                var updateRequest = new UserUpdateAdminRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Id = id,
                    Status = user.Status 
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(UserUpdateAdminRequest request)
        {
            ViewBag.Gender = SeletectGender(request.Gender.ToString());
            ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View();
            
            var result = await _userApiClient.UpdateAdmin(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin người dùng thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> UpdatePatient(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            
            //var doctor = await _userApiClient.Get
           
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Gender = SeletectGender(user.Gender.ToString());
                ViewBag.Status = SeletectStatus(user.Status);
                ViewBag.Img = user.PatientVm.Img;
                var updateRequest = new UserUpdatePatientRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Id = id,
                    Address = user.PatientVm.Address,
                    Status = user.Status 
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePatient([FromForm] UserUpdatePatientRequest request)
        {
            ViewBag.Img = request.img;
            ViewBag.Gender = SeletectGender(request.Gender.ToString());
            ViewBag.Status = SeletectStatus(request.Status);
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.UpdatePatient(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin người dùng thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> DetailtDoctor(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Gender = user.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.Dob.ToShortDateString();
                ViewBag.Status = user.Status == Status.NotActivate ? "Ngừng hoạt động" : user.Status == Status.Active ? "Hoạt động": "không hoạt động";
                var updateRequest = new UserVm()
                {
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Id = id,
                    Address = user.Address,
                    Img = user.Img,
                    UserName = user.UserName,
                    GetRole = user.GetRole,
                    DoctorVm = user.DoctorVm,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }
        /*public List<SelectListItem> SeletectStatus(Status status)
        {
            List<SelectListItem> lstatus = new List<SelectListItem>()
            {
                new SelectListItem(text: "Ngừng hoạt động", value: Status.NotActivate.ToString()),
                new SelectListItem(text: "Hoạt động", value: Status.Active.ToString()),
                new SelectListItem(text: "Không hoạt động", value: Status.InActive.ToString())
            };
            var rs = lstatus.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = status.ToString() == x.Value
            }).ToList();
            return rs;
        }*/
        [HttpGet]
        public async Task<IActionResult> DetailtPatient(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Gender = user.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.Dob.ToShortDateString();
                ViewBag.Status = user.Status == Status.NotActivate ? "Ngừng hoạt động" : user.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var updateRequest = new UserVm()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Id = id,
                    Address = user.Address,
                    Img = user.Img,
                    UserName = user.UserName,
                    Status = user.Status //== Status.Active ? true : false
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Detailt(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Gender = user.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.Dob.ToShortDateString();
                ViewBag.Status = user.Status == Status.NotActivate ? "Ngừng hoạt động" : user.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var updateRequest = new UserVm()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Id = id,
                    Address = user.Address,
                    Img = user.Img,
                    UserName = user.UserName,
                    Status = user.Status //== Status.Active ? true : false
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid idUser)
        {
            var result = await _userApiClient.Delete(idUser);
            return Json(new { response = result });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
    }
}
