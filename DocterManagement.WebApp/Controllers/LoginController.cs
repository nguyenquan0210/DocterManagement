using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Statistic;
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
        private readonly IDoctorApiClient _doctorApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Login";
        public LoginController(IUserApiClient userApiClient, IDoctorApiClient doctorApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient, IStatisticApiClient statisticApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _doctorApiClient = doctorApiClient;
            _statisticApiClient = statisticApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? usertemporary = null;
            string? user = null;
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);
                usertemporary = currentHistory.Usertemporary;
                user = currentHistory.User;
            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name == null ? user : User.Identity.Name,
                    Usertemporary = (usertemporary == null && User.Identity.Name == null) ? ("patient" + new Random().Next(10000000, 99999999) + new Random().Next(10000000, 99999999)) : (usertemporary == null ? User.Identity.Name : usertemporary),
                    Type = user == null ? "patientlogout" : "patient",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Get",
                ExtraProperties = "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            request.Check = "patient";
            var result = await _userApiClient.Authenticate(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }
            
                TempData["AlertMessage"] = "Đăng nhập thành công.";
                TempData["AlertType"] = "success";
                TempData["AlertId"] = "successToast";

            
            var userPrincipal = ValidateToken(result.Data);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };
           /* var patients = (await _doctorApiClient.GetPatientProfile(request.UserName)).Data;
            var patient = patients.FirstOrDefault(x => x.IsPrimary == true);
            HttpContext.Session.SetString(SystemConstants.Patient, JsonConvert.SerializeObject(patient));*/


            HttpContext.Session.SetString(SystemConstants.AppSettings.Token, result.Data);
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Register()
        {
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Register",
                MethodName = "Get",
                ExtraProperties ="success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterEnterPhoneRequest request)
        {
            if (!ModelState.IsValid) return View();
            var result = await _userApiClient.CheckPhone(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Register",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                TempData["AlertMessage"] = result.Message;
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }

            TempData["AlertMessage"] = "Gửi mã xác nhận thành công.";
            TempData["AlertType"] = "success";
            TempData["AlertId"] = "successToast";
            var registerPatientSession = new RegisterPatientSession()
            {
                Email = request.Email,
                NoOTP = result.Data,
                dateTime = DateTime.Now
            };
            ViewBag.RegisterPatientSession = registerPatientSession;
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));
            var RegisterEnterOTPRequest = new RegisterEnterOTPRequest()
            {
                Email = request.Email,
            };

            return RedirectToAction("RegisterEnterOTP", RegisterEnterOTPRequest);
        }
        [HttpPost]
        public async Task<IActionResult> ResetOtp(string Email)
        {
            var registerEnterPhone = new RegisterEnterPhoneRequest()
            {
                Email = Email,
            };
            var result = await _userApiClient.CheckPhone(registerEnterPhone);
            if (result.Data == null)
            {
                return Json(0);
            }
            var registerPatientSession = new RegisterPatientSession()
            {
                Email = Email,
                NoOTP = result.Data,
                dateTime = DateTime.Now
            };
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));

            return Json(1);
        }
        public async Task<IActionResult> RegisterEnterOTP()
        {
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            ViewBag.RegisterPatientSession = currentOtp;
            var RegisterEnterOTPRequest = new RegisterEnterOTPRequest()
            {
                Email = currentOtp.Email,
            };
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterOTP",
                MethodName = "Get",
                ExtraProperties ="success",
                Parameters = JsonConvert.SerializeObject(RegisterEnterOTPRequest),
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterOTP",
                MethodName = "POST",
                ExtraProperties = "success" ,
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (currentOtp.NoOTP != otp)
            {
                TempData["AlertMessage"] = "mã xác nhân không đúng";
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }else if(checktime.Minutes > 2)
            {
                TempData["AlertMessage"] = "mã xác nhân của bạn đã quá 2 phút";
                TempData["AlertType"] = "error";
                TempData["AlertId"] = "errorToast";
                return View(request);
            }
            TempData["AlertMessage"] = "Nhập mã xác nhận thành công.";
            TempData["AlertType"] = "success";
            TempData["AlertId"] = "successToast";
            ViewBag.RegisterPatientSession = currentOtp;

            return RedirectToAction("RegisterEnterPassword");
        }

        public async Task<IActionResult> RegisterEnterPassword()
        {
                var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
                var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
                var registerEnterPasswordRequest = new RegisterEnterPasswordRequest()
                {
                    Email = currentOtp.Email,
                };
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterPassword",
                MethodName = "GET",
                ExtraProperties = "success" ,
                Parameters = JsonConvert.SerializeObject(registerEnterPasswordRequest),
            };
            await HistoryActive(historyactive);
            return View(registerEnterPasswordRequest);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEnterPassword(RegisterEnterPasswordRequest request)
        {
            if (!ModelState.IsValid) return View(ModelState);
            var session = HttpContext.Session.GetString(SystemConstants.OtpSession);
            var currentOtp = JsonConvert.DeserializeObject<RegisterPatientSession>(session);
            var registerPatientSession = new RegisterPatientSession()
            {
                Email = currentOtp.Email,
                NoOTP = currentOtp.NoOTP,
                dateTime = currentOtp.dateTime,
                Password = request.Password,
            };
            HttpContext.Session.SetString(SystemConstants.OtpSession, JsonConvert.SerializeObject(registerPatientSession));
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterPassword",
                MethodName = "POST",
                ExtraProperties = "success",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);

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
                Email = currentOtp.Email,
                Password = currentOtp.Password,
            };
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterProfile",
                MethodName = "GET",
                ExtraProperties = "success" ,
                Parameters = JsonConvert.SerializeObject(registerEnterProfile),
            };
            await HistoryActive(historyactive);
            return View(registerEnterProfile);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEnterProfile(RegisterEnterProfileRequest request)
        {
            ViewBag.Ethnics = await _userApiClient.GetAllEthnicGroup();
            ViewBag.Province = await _locationApiClient.GetAllProvince(new Guid());
            if(request.ProvinceId != new Guid())
            {
                ViewBag.District = await _locationApiClient.CityGetAllDistrict(new Guid(), request.ProvinceId);
                if (request.DistrictId != new Guid())
                {
                    ViewBag.SubDistrict = await _locationApiClient.GetAllSubDistrict(new Guid(), request.DistrictId);
                }
            }
            
            if (!ModelState.IsValid) { return View(request); }
            var result = await _userApiClient.RegisterPatient(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".RegisterEnterProfile",
                MethodName = "POST",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin thành công";
                TempData["AlertType"] = "alert-success";
                var userName = request.Email.Split("@")[0];
                var login = new LoginRequest()
                {
                    UserName = userName,
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

                var patient = (await _doctorApiClient.GetPatientProfile(userName)).Data;
                HttpContext.Session.SetString(SystemConstants.Patient, JsonConvert.SerializeObject(patient.FirstOrDefault(x=>x.IsPrimary==true)));

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            request.Role = "patient";
            var rs = await _userApiClient.ForgotPassword(request);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(request);
        }
        [HttpGet]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordRequest resetPasswordModel = new ResetPasswordRequest
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var rs = await _userApiClient.ResetPassword(request);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(request);
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
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Home");
        }
    }
}
