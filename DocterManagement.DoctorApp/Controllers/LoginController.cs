using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.System.Statistic;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Login";

        public LoginController(IUserApiClient userApiClient,
            IConfiguration configuration, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);

            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name,
                    Usertemporary = User.Identity.Name,
                    Type = "doctor",
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);
            request.Check = "doctor";
            var result = await _userApiClient.Authenticate(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Post",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("", result.Message);
                return View(request);
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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            request.Role = "doctor";
            var rs = await _userApiClient.ForgotPassword(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".ForgotPassword",
                MethodName = "Post",
                ExtraProperties = rs.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string uid, string token)
        {
            ResetPasswordRequest resetPasswordModel = new ResetPasswordRequest
            {
                Token = token,
                UserId = uid
            };
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".ResetPassword",
                MethodName = "Get",
                ExtraProperties ="success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            return View(resetPasswordModel);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var rs = await _userApiClient.ResetPassword(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".ResetPassword",
                MethodName = "Post",
                ExtraProperties = rs.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> LoginClient(string token)
        {
            var userPrincipal = ValidateToken(token);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };

            HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token);

            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
            return RedirectToAction("Index", "Home");
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
