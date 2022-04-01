using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ApiResult<bool>> AddRoleUser(RequestRoleUser request)
        {
            var user = await _userManager.FindByIdAsync(request.IdUser.ToString());

            var role = await _roleManager.FindByIdAsync(request.IdRole.ToString());

            if (await _userManager.IsInRoleAsync(user, role.Name) == false)
            {
                var result = await _userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    return new ApiSuccessResult<bool>();
                }
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
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

        public async Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            var checkpass = await _userManager.CheckPasswordAsync(user, request.currentPassword);
            if (!checkpass)
            {
                return new ApiErrorResult<bool>("Mật khẩu không đúng.");
            }
            var result = await _userManager.ChangePasswordAsync(user, request.currentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Đổi mật khẩu không thành công!");
        }

        public async Task<int> Delete(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            int check = 0;
            var role = await _roleManager.FindByNameAsync("staff");
            if (user == null) return check;
            if (user.Status == Status.Active)
            {
                user.Status = Status.InActive;
                var reultupdate = await _userManager.UpdateAsync(user);
                check = 1;
            }
            else
            {
                var reult = await _userManager.DeleteAsync(user);
                if (reult.Succeeded)
                {
                    /*if (user.Img != null)
                    {
                        await _storageService.DeleteFileAsync(user.Img);
                    }*/
                    if (await _userManager.IsInRoleAsync(user, role.Id.ToString()) == true)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Id.ToString());
                    }
                    check = 2;
                }
            }
            return check;

        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                Gender = user.Gender,
                Dob = user.Dob,
                Id = user.Id,
                UserName = user.UserName,
                Status = user.Status,
                Roles = roles
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public async Task<ApiResult<UserVm>> GetByUserName(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("Tài khoản không tồn tại");
            }
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                Dob = user.Dob,
                Id = user.Id,
                UserName = user.UserName,
                Gender = user.Gender
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public List<UserVm> GetNewUser()
        {
            var query = _userManager.GetUsersInRoleAsync("patient").Result;
            var data = query.Select(x => new UserVm()
            {
                Date = x.Date
            }).ToList();

            return data;
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersAllPaging(GetUserPagingRequest request)
        {
            var query = _userManager.Users;

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName.Contains(request.Keyword)
                 || x.PhoneNumber.Contains(request.Keyword));
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    Gender = x.Gender,
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status
                }).ToListAsync();

            //4. Select and projection
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        public ApiResult<PagedResult<UserVm>> GetUsersPaging(GetUserPagingRequest request)
        {
            var query = _userManager.GetUsersInRoleAsync(request.RoleName).Result;

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = (IList<AppUsers>)query.Where(x => x.UserName.Contains(request.Keyword)
                 || x.PhoneNumber.Contains(request.Keyword));
            }
            //3. Paging
            int totalRow = query.Count;

            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    Name = x.Name,
                    Id = x.Id,
                    Gender = x.Gender,
                    Status = x.Status
                }).ToList();

            //4. Select and projection
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        public async Task<ApiResult<bool>> ManageRegister(ManageRegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var role = await _roleManager.FindByNameAsync(request.NameRole);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }

            user = new AppUsers()
            {
                Dob = request.Dob,
                Email = request.Email,
                Name = request.Name,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Status = Status.Active,
                Date = DateTime.Now

            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    var rs = await _userManager.AddToRoleAsync(user, role.Name);

                    if (rs.Succeeded)
                    {
                        return new ApiSuccessResult<bool>();
                    }
                }
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }

        public async Task<ApiResult<bool>> Register(PublicRegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var role = await _roleManager.FindByNameAsync(request.NameRole);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            user = new AppUsers()
            {
                Dob = DateTime.Now.AddYears(-12),
                Name = request.Name,
                UserName = request.UserName,
                Status = Status.Active,
                Date = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    var rs = await _userManager.AddToRoleAsync(user, role.Name);

                    if (rs.Succeeded)
                    {
                        return new ApiSuccessResult<bool>();
                    }
                }
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            }
            var removedRoles = request.Roles.Where(x => x.Selected == false).Select(x => x.Name).ToList();
            foreach (var roleName in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            var addedRoles = request.Roles.Where(x => x.Selected).Select(x => x.Name).ToList();
            foreach (var roleName in addedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            /*if (request.ThumbnailImage != null)
            {
                if (user.Img != null)
                {
                    await _storageService.DeleteFileAsync(user.Img);
                }

                user.Img = await this.SaveFile(request.ThumbnailImage);

            }*/

            user.Dob = request.Dob;
            user.Email = request.Email;
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status ? Status.Active : Status.InActive;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }
    }
}
