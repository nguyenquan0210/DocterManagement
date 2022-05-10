﻿using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
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
        public async Task<IActionResult> Index()
        {
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Clinic = await _userApiClient.GetAllClinic(user.DoctorVm.GetClinic.Id);
                ViewBag.Imgs = user.DoctorVm.Galleries;
                ViewBag.SetChoices = JsonConvert.SerializeObject(await SeletectSpecialities(user.DoctorVm.GetSpecialities));
                ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
                ViewBag.District = await _locationApiClient.CityGetAllDistrict(user.DoctorVm.Location.District.Id, user.DoctorVm.Location.District.Province.Id);
                ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(user.DoctorVm.Location.Id, user.DoctorVm.Location.District.Id);
                var updateRequest = new DocterRequestAll()
                {
                    Dob = user.DoctorVm.Dob,
                    FirstName = user.DoctorVm.FirstName,
                    LastName = user.DoctorVm.LastName,
                    Gender = user.DoctorVm.Gender,
                    Id = user.Id,
                    Address = user.DoctorVm.Address,
                    ClinicId = user.DoctorVm.GetClinic.Id,
                    //img = user.DoctorVm.Img,
                    Description = user.DoctorVm.Intro,
                    Services = user.DoctorVm.Services,
                    Slug = user.DoctorVm.Slug.Replace("-" + user.DoctorVm.No, ""),
                    Educations = user.DoctorVm.Educations,
                    Prefix = user.DoctorVm.Prefix,
                    MapUrl = user.DoctorVm.MapUrl,
                    Booking = user.DoctorVm.Booking,
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
        [HttpPost]
        public async Task<IActionResult> Index(DoctorUpdateProfile request)
        {
            ViewBag.Imgs = request.GetGalleries;
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
            var result = await _userApiClient.DoctorUpdateProfile(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin thành công";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";
                return RedirectToAction("Index");
            }
          
            if (result.ValidationErrors[0] == "warning")
            {
                TempData["AlertMessage"] = result.ValidationErrors[1];
                TempData["AlertType"] = "warning";
                TempData["AlertId"] = "warningToast";
                return RedirectToAction("Index");
            }
            TempData["AlertMessage"] = result.ValidationErrors[1];
            TempData["AlertType"] = "danger";
            TempData["AlertId"] = "dangerToast";
            
    
           
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Clinic = await _userApiClient.GetAllClinic(user.DoctorVm.GetClinic.Id);
                ViewBag.Imgs = user.DoctorVm.Galleries;
                ViewBag.SetChoices = JsonConvert.SerializeObject(await SeletectSpecialities(user.DoctorVm.GetSpecialities));
                ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
                ViewBag.District = await _locationApiClient.CityGetAllDistrict(user.DoctorVm.Location.District.Id, user.DoctorVm.Location.District.Province.Id);
                ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(user.DoctorVm.Location.Id, user.DoctorVm.Location.District.Id);
                var updateRequest = new DoctorUpdateRequest()
                {
                    Dob = user.DoctorVm.Dob,
                    FirstName = user.DoctorVm.FirstName,
                    LastName = user.DoctorVm.LastName,
                    Gender = user.DoctorVm.Gender,
                    Id = user.Id,
                    Address = user.DoctorVm.Address,
                    ClinicId = user.DoctorVm.GetClinic.Id,
                    //img = user.DoctorVm.Img,
                    Description = user.DoctorVm.Intro,
                    Services = user.DoctorVm.Services,
                    Slug = user.DoctorVm.Slug.Replace("-" + user.DoctorVm.No, ""),
                    Educations = user.DoctorVm.Educations,
                    Prefix = user.DoctorVm.Prefix,
                    MapUrl = user.DoctorVm.MapUrl,
                    Booking = user.DoctorVm.Booking,
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
            foreach (var spe in chose)
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
        public async Task<IActionResult> Update([FromForm] DoctorUpdateRequest request)
        {
            ViewBag.Imgs = request.GetGalleries;
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

            var result = await _userApiClient.DoctorUpdateRequest(request.Id, request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("DetailtDoctor", new { userName = User.Identity.Name});
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
        public async Task<IActionResult> DetailtDoctor(string userName)
        {
            var result = await _userApiClient.GetByUserName(userName);
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Img = user.DoctorVm.Img;
                ViewBag.Gender = user.Gender == Gender.Male ? "Nam" : "Nữ";
                ViewBag.Dob = user.Dob.ToShortDateString();
                ViewBag.Status = user.Status == Status.NotActivate ? "Ngừng hoạt động" : user.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var updateRequest = new UserVm()
                {
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
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


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            request.UserName = User.Identity.Name;
            var result = await _userApiClient.ChangePassword(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Đổi mật khẩu thành công";
                TempData["AlertType"] = "alert-success";
                return View();
            }
            TempData["AlertMessage"] = "Đổi mật khẩu bị lỗi";
            TempData["AlertType"] = "alert-warning";
            return View(request);
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
