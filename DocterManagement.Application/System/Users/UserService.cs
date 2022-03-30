using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly SignInManager<AppUsers> _signInManager;
        private readonly RoleManager<AppRoles> _roleManager;
        private readonly IConfiguration _config;
        //private readonly IStorageService _storageService;
        public UserService(UserManager<AppUsers> userManager,
            SignInManager<AppUsers> signInManager,
            RoleManager<AppRoles> roleManager,
            IConfiguration config
            //IStorageService storageService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            //_storageService = storageService;
        }

        public async Task<ApiResult<string>> Authencate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return new ApiErrorResult<string>("Tài khoản không tồn tại");

            if (user.Status == Status.InActive) return new ApiErrorResult<string>("Tài khoản bị khóa");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<string>("Đăng nhập không đúng.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var checkrole = roles.Where(x => x.ToUpper() == "ADMIN");

            if (checkrole.Count() == 0 && request.Check) return new ApiErrorResult<string>("Chỉ nhận quản trị viên.");

            var claims = new[]
            {
                //new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.Name),
                new Claim(ClaimTypes.Role, string.Join(";",roles)),
                new Claim(ClaimTypes.Name, request.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }

    }
}
