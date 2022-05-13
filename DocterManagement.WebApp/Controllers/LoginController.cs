using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoctorManagement.WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;

        public LoginController(IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            request.Check = "patient";
            var result = await _userApiClient.Authenticate(request);
            if (result.Data == null)
            {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            var userPrincipal = ValidateToken(result.Data);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };

            HttpContext.Session.SetString(SystemConstants.AppSettings.Token, result.Data);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterEnterPhoneRequest request)
        {
            if (!ModelState.IsValid) return View();
            var result = await _userApiClient.CheckPhone(request);
            if (result.Data == null)
            {
                ModelState.AddModelError("", result.Message);
                return View();
            }
            var registerPatientSession = new RegisterPatientSession()
            {
                PhoneNumber = request.PhoneNumber,
                NoOTP = result.Data,
                dateTime = DateTime.Now
            };
            ViewBag.RegisterPatientSession = registerPatientSession;
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));
            var RegisterEnterOTPRequest = new RegisterEnterOTPRequest()
            {
                PhoneNumber = request.PhoneNumber,
            };

            return RedirectToAction("RegisterEnterOTP", RegisterEnterOTPRequest);
        }
        [HttpPost]
        public async Task<IActionResult> ResetOtp(string phoneNumber)
        {
            var registerEnterPhone = new RegisterEnterPhoneRequest()
            {
                PhoneNumber = phoneNumber,
            };
            var result = await _userApiClient.CheckPhone(registerEnterPhone);
            if (result.Data == null)
            {
                return Json(0);
            }
            var registerPatientSession = new RegisterPatientSession()
            {
                PhoneNumber = phoneNumber,
                NoOTP = result.Data,
                dateTime = DateTime.Now
            };
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));

            return Json(1);
        }
        public IActionResult RegisterEnterOTP()
        {
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            ViewBag.RegisterPatientSession = currentOtp;
            var RegisterEnterOTPRequest = new RegisterEnterOTPRequest()
            {
                PhoneNumber = currentOtp.PhoneNumber,
            };
            return View(RegisterEnterOTPRequest);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEnterOTP(RegisterEnterOTPRequest request)
        {
            if (!ModelState.IsValid) return View("RegisterEnterOTP", request);
            var otp = request.Otp_0 + request.Otp_1 + request.Otp_2 + request.Otp_3 + request.Otp_4 + request.Otp_5;
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            var checktime = DateTime.Now - currentOtp.dateTime;
            if(currentOtp.NoOTP != otp)
            {
                ModelState.AddModelError("", "mã otp không đúng");
                return View();
            }else if(checktime.Minutes > 2)
            {
                ModelState.AddModelError("", "mã otp của bạn đã quá 2 phút.");
                return View();
            }
            ViewBag.RegisterPatientSession = currentOtp;

            return RedirectToAction("RegisterEnterPassword");
        }

        public IActionResult RegisterEnterPassword()
        {
                var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
                var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
                var registerEnterPasswordRequest = new RegisterEnterPasswordRequest()
                {
                    PhoneNumber = currentOtp.PhoneNumber,
                };
                return View(registerEnterPasswordRequest);
        }
        [HttpPost]
        public IActionResult RegisterEnterPassword(RegisterEnterPasswordRequest request)
        {
            if (!ModelState.IsValid) return View(ModelState);
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            var registerPatientSession = new RegisterPatientSession()
            {
                PhoneNumber = currentOtp.PhoneNumber,
                NoOTP = currentOtp.NoOTP,
                dateTime = currentOtp.dateTime,
                Password = request.Password,
            };
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));


            return RedirectToAction("RegisterEnterProfile");
        }
        public async Task<IActionResult> RegisterEnterProfile()
        {
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            ViewBag.District = new List<SelectListItem>();
            ViewBag.SubDistrict = new List<SelectListItem>();
            var registerEnterProfile = new RegisterEnterProfileRequest()
            {
                PhoneNumber = currentOtp.PhoneNumber,
                Password = currentOtp.Password,
                RelativePhone = currentOtp.PhoneNumber
            };
            return View(registerEnterProfile);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEnterProfile(RegisterEnterProfileRequest request)
        {
            if (!ModelState.IsValid) { return View(ModelState); }
            var result = await _userApiClient.RegisterPatient(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin thành công";
                TempData["AlertType"] = "alert-success";
                var login = new LoginRequest()
                {
                    UserName = request.PhoneNumber,
                    Password = request.Password,
                    Check = "patient",
                    RememberMe = false
                };
                var rs = await _userApiClient.Authenticate(login);
                if (rs.Data == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var userPrincipal = ValidateToken(rs.Data);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = false
                };

                HttpContext.Session.SetString(SystemConstants.AppSettings.Token, rs.Data);
                await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            userPrincipal,
                            authProperties);
                return RedirectToAction("Index", "Home");
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
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}
