using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;

        public UserController(IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
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
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = new List<SelectListItem>();
            ViewBag.SubDistrict = new List<SelectListItem>();
            return View();
        }

        [HttpPost]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(ManageRegisterRequest request)
        {
            ViewBag.Clinic = await _userApiClient.GetAllClinic(request.ClinicId);
            //ViewBag.Speciality = await _userApiClient.GetAllSpeciality(request.SpecialityId);
            ViewBag.Province = await _locationApiClient.GetAllProvince(request.ProvinceId);
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(request.DistrictId, request.ProvinceId==null?new Guid():request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(request.SubDistrictId,request.DistrictId == null ? new Guid() : request.DistrictId);
            //ViewBag.Gender = SeletectGender(request.Gender.ToString());
            if (!ModelState.IsValid)
                return View();
            
            var result = await _userApiClient.RegisterDocter(request);

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
        public async Task<IActionResult> GetSubDistrict(Guid DistrictId)
        {
            if (!string.IsNullOrWhiteSpace(DistrictId.ToString()))
            {
                var district = await _locationApiClient.GetAllSubDistrict(null, DistrictId);
                return Json(district);
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> GetDistrict(Guid ProvinceId)
        {
            if (!string.IsNullOrWhiteSpace(ProvinceId.ToString()))
            {
                var district = await _locationApiClient.CityGetAllDistrict(null, ProvinceId);
                return Json(district);
            }
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            //var doctor = await _userApiClient.Get
            if (result.IsSuccessed)
            {
                var user = result.Data;
                //ViewBag.Gender = SeletectGender(user.Gender.ToString());
                ViewBag.Status = SeletectStatus(user.Status);
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Clinic = await _userApiClient.GetAllClinic(user.DoctorVm.GetClinic.Id);
                ViewBag.Imgs = user.DoctorVm.Galleries;
                //ViewBag.Speciality = await _userApiClient.GetAllSpeciality(new Guid());
                ViewBag.SetChoices = JsonConvert.SerializeObject(await SeletectSpecialities(user.DoctorVm.GetSpecialities));
                ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
                ViewBag.District = await _locationApiClient.CityGetAllDistrict(user.DoctorVm.Location.District.Id, user.DoctorVm.Location.District.Province.Id);
                ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(user.DoctorVm.Location.Id, user.DoctorVm.Location.District.Id);
                var updateRequest = new UserUpdateRequest()
                {
                    Dob = user.DoctorVm.Dob,
                    //Email = user.Email,
                    FirstName = user.DoctorVm.FirstName,
                    LastName = user.DoctorVm.LastName,
                    //PhoneNumber = user.PhoneNumber,
                    Gender = user.DoctorVm.Gender,
                    Id = id,
                    Address = user.DoctorVm.Address,
                    Status = user.Status,
                    ClinicId = user.DoctorVm.GetClinic.Id,
                    //img = user.DoctorVm.Img,
                    Description = user.DoctorVm.Intro,
                    Services = user.DoctorVm.Services,
                    Slug = user.DoctorVm.Slug.Replace("-"+user.DoctorVm.No,""),
                    Educations = user.DoctorVm.Educations,
                    Prefix = user.DoctorVm.Prefix,
                    MapUrl = user.DoctorVm.MapUrl,
                    Booking = user.DoctorVm.Booking,
                    IsPrimary = user.DoctorVm.IsPrimary,
                    Prizes = user.DoctorVm.Prizes,
                    ProvinceId = user.DoctorVm.Location.District.Province.Id,
                    DistrictId = user.DoctorVm.Location.District.Id,
                    SubDistrictId = user.DoctorVm.Location.Id,
                    Note = user.DoctorVm.Note,
                    TimeWorking = user.DoctorVm.TimeWorking,
                    GetGalleries = user.DoctorVm.Galleries
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }
        public async Task<List<SetChoicesVm>> SeletectSpecialities(List<GetSpecialityVm> specialities)
        {
            var chose = await _userApiClient.GetAllSpeciality(new Guid());
            var rs = new List<SetChoicesVm>();
            foreach (var spe in specialities)
            {

                var chose_spe = new SetChoicesVm()
                {
                    selected = true,
                    label = spe.Title,
                    value = spe.Id.ToString()
                };
                rs.Add(chose_spe);
                chose = chose.Where(x => x.Value != spe.Id.ToString()).ToList();
            }
            foreach(var spe in chose)
            {
                var chose_spe = new SetChoicesVm()
                {
                    selected = false,
                    label = spe.Text,
                    value = spe.Value
                };
                rs.Add(chose_spe);
            }
            return rs;
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            //ViewBag.Img = request.img;
            ViewBag.Imgs = request.GetGalleries;
            //ViewBag.Gender = SeletectGender(request.Gender.ToString());
            ViewBag.Status = SeletectStatus(request.Status);
            ViewBag.Clinic = await _userApiClient.GetAllClinic(request.ClinicId);
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(), request.ProvinceId);
            ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(), request.DistrictId);
            var getallspecialities = await _userApiClient.GetAllSpeciality(new Guid());
            var specialities = new List<GetSpecialityVm>();
            foreach (var spe in getallspecialities)
            {
                var speciality = new GetSpecialityVm()
                {
                    Id = new Guid(spe.Value),
                    IsDeleted = false,
                    Title = spe.Text
                };
                specialities.Add(speciality);
            }
            
            ViewBag.SetChoices = JsonConvert.SerializeObject(await SeletectSpecialities(specialities));
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
                ViewBag.Gender = user.DoctorVm.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.DoctorVm.Dob.ToShortDateString();
                ViewBag.Status = user.Status == Status.NotActivate ? "Ngừng hoạt động" : user.Status == Status.Active ? "Hoạt động": "không hoạt động";
                ViewBag.Specialities = user.DoctorVm.GetSpecialities;
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
                ViewBag.Img = user.PatientVm.Img;
                ViewBag.Gender = user.PatientVm.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.PatientVm.Dob.ToShortDateString();
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
        public async Task<IActionResult> DeleteImg(Guid imgId)
        {
            var result = await _userApiClient.DeleteImg(imgId);
            return Json(new { response = result });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAllImg(Guid doctorId)
        {
            var result = await _userApiClient.DeleteAllImg(doctorId);
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
