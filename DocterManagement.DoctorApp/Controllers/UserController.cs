using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.DoctorApp.Controllers
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


        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            //var doctor = await _userApiClient.Get
            if (result.IsSuccessed)
            {
                var user = result.Data;
                ViewBag.Gender = SeletectGender(user.Gender.ToString());
                ViewBag.Img = user.DoctorVm.Img;
                var updateRequest = new UserUpdateRequest()
                {
                    Dob = user.Dob,
                    Email = user.Email,
                    FirstName = user.DoctorVm.FirstName,
                    LastName = user.DoctorVm.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Id = id,
                    Address = user.DoctorVm.Address,
                    Status = user.Status,
                    //SpecialityId = user.DoctorVm.GetSpeciality.Id,
                    ClinicId = user.DoctorVm.GetClinic.Id,
                    //img = user.DoctorVm.Img,
                    Description = user.DoctorVm.Intro
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            //ViewBag.Img = request.img;
            ViewBag.Gender = SeletectGender(request.Gender.ToString());
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
    }
}
