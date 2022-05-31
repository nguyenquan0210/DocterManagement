using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Models;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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
        private readonly DoctorManageDbContext _context; 
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;
        private readonly IEmailService _emailService;
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private const string GALLERY_CONTENT_FOLDER_NAME = "gallery-content"; 
        public UserService(UserManager<AppUsers> userManager,
            SignInManager<AppUsers> signInManager,
            RoleManager<AppRoles> roleManager,
            IConfiguration config,
            DoctorManageDbContext context,
            IEmailService emailService,
            IStorageService storageService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
            _emailService = emailService;
            _storageService = storageService;
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

            var roles = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            if(request.Check.ToUpper() == "ADMIN")
            {
                if (roles.Name.ToUpper() != "ADMIN" ) return new ApiErrorResult<string>("Chỉ nhận quản trị viên.");
            }
            if (request.Check.ToUpper() == "DOCTOR")
            {
                if (roles.Name.ToUpper() != "DOCTOR") return new ApiErrorResult<string>("Chỉ nhận tài khoản bác sĩ.");
            }
            if (request.Check.ToUpper() == "PATIENT")
            {
                if (roles.Name.ToUpper() != "PATIENT") return new ApiErrorResult<string>("Chỉ nhận tài khoản bệnh nhân.");
            }
          
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email),
                //new Claim(ClaimTypes.GivenName,user.Name),
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
        public async Task<ApiResult<string>> CheckPhone(RegisterEnterPhoneRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user != null) return new ApiErrorResult<string>("Tài khoản đã tồn tại");
            var otp = "";
            for (var i = 0; i < 6; i++)
            {
                 otp = otp + new Random().Next(0, 9).ToString();
            }
            return new ApiSuccessResult<string>(otp);
        }
        public async Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return new ApiErrorResult<bool>(new string[] { "warning", "Tài khoản không tồn tại" });
            var checkpass = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!checkpass)
            {
                return new ApiErrorResult<bool>(new string[] {"warning","Mật khẩu không đúng."});
            }
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                await SendEmailChangePassword(user);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>(new string[] { "warning", "Đổi mật khẩu không thành công!" });
        }
        private async Task SendEmailChangePassword(AppUsers user)
        {
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName)
                }
            };
            await _emailService.SendEmailChangePassword(options);
        }
        public async Task<ApiResult<int>> IsBooking(Guid Id)
        {
            var doctor = await _context.Doctors.FindAsync(Id);
            int check = 0;
            //var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            if (doctor == null) return new ApiSuccessResult<int>(check);
            if (doctor.Booking)
            {
                doctor.Booking = false;
                check = 1;
            }
            else
            {
                doctor.Booking = true;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);

        }
        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            var doctor = await _context.Doctors.FindAsync(user.Id);
            int check = 0;
            //var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            if (user == null) return new ApiSuccessResult<int>(check);
            if (user.Status == Status.Active)
            {
                user.Status = Status.InActive;
                await _userManager.UpdateAsync(user);
                check = 1;
            }
            else if (user.Status == Status.InActive)
            {
                user.Status = Status.NotActivate;
                await _userManager.UpdateAsync(user);
                check = 2;
            }
            else if (user.Status == Status.NotActivate)
            {
                var reult = await _userManager.DeleteAsync(user);
                if (reult.Succeeded)
                {
                    if (doctor.Img != null)
                    {
                        await _storageService.DeleteFileAsyncs(doctor.Img, USER_CONTENT_FOLDER_NAME);
                    }
                    if (await _userManager.IsInRoleAsync(user,user.RoleId.ToString()) == true)
                    {
                        await _userManager.RemoveFromRoleAsync(user, user.RoleId.ToString());
                    }
                    check = 2;
                }
            }
            /*else
            {
                var reult = await _userManager.DeleteAsync(user);
                if (reult.Succeeded)
                {
                    *//*if (user.Img != null)
                    {
                        await _storageService.DeleteFileAsync(user.Img);
                    }*//*
                    if (await _userManager.IsInRoleAsync(user, role.Id.ToString()) == true)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.Id.ToString());
                    }
                    check = 2;
                }
            }*/
            return new ApiSuccessResult<int>(check);

        }
        public async Task<ApiResult<int>> DeleteImg(Guid Id)
        {
            var image = await _context.Galleries.FindAsync(Id);
            int check = 0;
            if (image == null) return new ApiSuccessResult<int>(check);

            await _storageService.DeleteFileAsyncs(image.Img, GALLERY_CONTENT_FOLDER_NAME);
            _context.Galleries.Remove(image);
            check = 2;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check); 
        }
        public async Task<ApiResult<int>> DeleteAllImg(Guid Id)
        {
            var images = _context.Galleries.Where(x => x.DoctorId == Id).ToList();
            int check = 0;
            if (images.Count == 0) return new ApiSuccessResult<int>(check);
            foreach (var image in images)
            {
                var removeImg = await _context.Galleries.FindAsync(image.Id);
                await _storageService.DeleteFileAsyncs(removeImg.Img, GALLERY_CONTENT_FOLDER_NAME);
                _context.Galleries.Remove(removeImg);
            }
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                check = 2;
            }
            return new ApiSuccessResult<int>(check);
        }
        public async Task<ApiResult<List<RoleVm>>> GetAllRole()
        {
            var result = _context.AppRoles;
            var data = await result.Select(x => new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();
            return new ApiSuccessResult<List<RoleVm>>(data);
        }

        public async Task<ApiResult<List<RoleVm>>> GetAllRoleData()
        {
            var result = _context.AppRoles;
            var data = await result.Select(x => new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();
            return new ApiSuccessResult<List<RoleVm>>(data);
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());
            
            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var location = new Locations();
            var district = new Locations();
            var province = new Locations();
            var doctor = await _context.Doctors.FindAsync(user.Id);
            var clinic = await _context.Clinics.FindAsync(doctor != null ? doctor.ClinicId : new Guid());
            
            var specialities = from s in _context.ServicesSpecialities
                             join spe in _context.Specialities on s.SpecialityId equals spe.Id where s.IsDelete ==false
                             where s.DoctorId == user.Id
                             select new {s,spe};
            var rates = from r in _context.Rates
                        join a in _context.Appointments on r.AppointmentId equals a.Id
                        where a.DoctorId == user.Id
                        select new {r,a};

            var galleries = _context.Galleries.Where(x => x.DoctorId == user.Id);
          
            var patient = await _context.Patients.FindAsync(user.Id);
            var ethnic = await _context.Ethnics.FindAsync(patient != null ? patient.EthnicId : new Guid());
            if(doctor != null)
            {
                location = await _context.Locations.FindAsync(doctor.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            else if (patient != null)
            {
                location = await _context.Locations.FindAsync(patient.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            var fulladdreess =  location != null ? location.Name + ", " + district.Name + ", " + province.Name : null;
            //var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                UserName = user.UserName,
                Status = user.Status,
                Name = doctor != null ? doctor.LastName+ " "+doctor.FirstName : patient != null ? patient.Name : null,
                Address = doctor != null ? doctor.Address + ", " + fulladdreess : patient != null ? patient.Address + ", " + fulladdreess : null ,
                GetRole = new GetRoleVm()
                {
                    Id = role.Id,
                    Name = role.Name
                },
                DoctorVm = doctor != null ? new DoctorVm()
                {
                    UserId =doctor.UserId,
                    FirstName = doctor.FirstName,
                    Intro = doctor.Intro,
                    Address = doctor.Address,
                    Img = doctor.Img,
                    No = doctor.No,
                    Services = doctor.Services,
                    Slug = doctor.Slug,
                    Booking = doctor.Booking,
                    BeforeBookingDay = doctor.BeforeBookingDay,
                    Dob = doctor.Dob,
                    Educations = doctor.Educations,
                    Gender = doctor.Gender,
                    LastName = doctor.LastName,
                    IsPrimary = doctor.IsPrimary,
                    MapUrl = doctor.MapUrl,
                    Note = doctor.Note,
                    Prefix = doctor.Prefix,
                    Prizes = doctor.Prizes,
                    View = doctor.View,
                    TimeWorking = doctor.TimeWorking,
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    GetClinic = new GetClinicVm(){ Id = clinic.Id, Name = clinic.Name},
                    GetSpecialities = specialities.Select(x => new GetSpecialityVm() { Id = x.spe.Id, Title = x.spe.Title }).ToList(),
                    Rates = rates.Select(x => new RateVm() { Id = x.r.Id, Rating = x.r.Rating }).ToList(),
                    Galleries = galleries.Select(x => new GalleryVm() { Id = x.Id, Name = GALLERY_CONTENT_FOLDER_NAME + "/" + x.Img }).ToList(),
                } : new DoctorVm()
                    ,
                PatientVm = patient != null ? new PatientVm()
                {
                    UserId = patient.UserId,
                    Address = patient.Address,
                    Img = patient.Img,
                    No = patient.No,
                    RelativeName = patient.RelativeName,
                    Name = patient.Name,
                    Dob = patient.Dob,
                    Gender = patient.Gender,
                    Identitycard = patient.Identitycard,
                    IsPrimary = patient.IsPrimary,
                    RelativePhone = patient.RelativePhone,
                    RelativeRelationshipId = patient.RelativeRelationshipId,
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    Ethnics = new EthnicVm() { Id = ethnic.Id, Name = ethnic.Name },

                } : new PatientVm()
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public async Task<ApiResult<UserVm>> GetByUserName(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());

            if (user == null)
            {
                return new ApiErrorResult<UserVm>("User không tồn tại");
            }
            var location = new Locations();
            var district = new Locations();
            var province = new Locations();
            var doctor = await _context.Doctors.FindAsync(user.Id);
            var clinic = await _context.Clinics.FindAsync(doctor != null ? doctor.ClinicId : new Guid());

            var specialities = from s in _context.ServicesSpecialities
                               join spe in _context.Specialities on s.SpecialityId equals spe.Id
                               where s.IsDelete == false
                               where s.DoctorId == user.Id
                               select new { s, spe };
            var rates = from r in _context.Rates
                        join a in _context.Appointments on r.AppointmentId equals a.Id
                        where a.DoctorId == user.Id
                        select new { r, a };

            var galleries = _context.Galleries.Where(x => x.DoctorId == user.Id);

            var patient = await _context.Patients.FindAsync(user.Id);
            var ethnic = await _context.Ethnics.FindAsync(patient != null ? patient.EthnicId : new Guid());
            if (doctor != null)
            {
                location = await _context.Locations.FindAsync(doctor.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            else if (patient != null)
            {
                location = await _context.Locations.FindAsync(patient.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            var fulladdreess = location != null ? location.Name + ", " + district.Name + ", " + province.Name : null;
            //var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                UserName = user.UserName,
                Status = user.Status,
                Name = doctor != null ? doctor.LastName + " " + doctor.FirstName : patient != null ? patient.Name : null,
                Address = doctor != null ? doctor.Address + ", " + fulladdreess : patient != null ? patient.Address + ", " + fulladdreess : null,
                GetRole = new GetRoleVm()
                {
                    Id = role.Id,
                    Name = role.Name
                },
                DoctorVm = doctor != null ? new DoctorVm()
                {
                    UserId = doctor.UserId,
                    FirstName = doctor.FirstName,
                    Intro = doctor.Intro,
                    Address = doctor.Address,
                    Img = doctor.Img,
                    No = doctor.No,
                    Services = doctor.Services,
                    Slug = doctor.Slug,
                    Booking = doctor.Booking,
                    BeforeBookingDay = doctor.BeforeBookingDay,
                    Dob = doctor.Dob,
                    Educations = doctor.Educations,
                    Gender = doctor.Gender,
                    LastName = doctor.LastName,
                    IsPrimary = doctor.IsPrimary,
                    MapUrl = doctor.MapUrl,
                    Note = doctor.Note,
                    Prefix = doctor.Prefix,
                    Prizes = doctor.Prizes,
                    View = doctor.View,
                    TimeWorking = doctor.TimeWorking,
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    GetClinic = clinic == null?new GetClinicVm(): new GetClinicVm() { Id = clinic.Id, Name = clinic.Name },
                    GetSpecialities = specialities.Select(x => new GetSpecialityVm() { Id = x.spe.Id, Title = x.spe.Title }).ToList(),
                    Rates = rates.Select(x => new RateVm() { Id = x.r.Id, Rating = x.r.Rating }).ToList(),
                    Galleries = galleries.Select(x => new GalleryVm() { Id = x.Id, Name = GALLERY_CONTENT_FOLDER_NAME + "/" + x.Img }).ToList(),
                } : new DoctorVm()
                    ,
                PatientVm = patient != null ? new PatientVm()
                {
                    UserId = patient.UserId,
                    Address = patient.Address,
                    Img = patient.Img,
                    No = patient.No,
                    RelativeName = patient.RelativeName,
                    Name = patient.Name,
                    Dob = patient.Dob,
                    Gender = patient.Gender,
                    Identitycard = patient.Identitycard,
                    IsPrimary = patient.IsPrimary,
                    RelativePhone = patient.RelativePhone,
                    RelativeRelationshipId = patient.RelativeRelationshipId,
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    Ethnics = new EthnicVm() { Id = ethnic.Id, Name = ethnic.Name },

                } : new PatientVm()
            };
            return new ApiSuccessResult<UserVm>(userVm);
        }

        public List<UserVm> GetNewUser()
        {
            var query = _userManager.GetUsersInRoleAsync("patient").Result;
            var data = query.Select(x => new UserVm()
            {
                Date = x.CreatedAt
            }).ToList();

            return data;
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersAllPaging(GetUserPagingRequest request)
        {
            var query = from u in _context.AppUsers
                        select u;
            /* join d in _context.Doctors on u.Id equals d.UserId into dt
                        from d in dt.DefaultIfEmpty()
                        join s in _context.ServicesSpecialities on d.UserId equals s.DoctorId into ss
                        from s in ss.DefaultIfEmpty()
                        join sp in _context.Specialities on s.SpecialityId equals sp.Id into spe
                        from sp in spe.DefaultIfEmpty()*/
            /*join r in _context.Roles on u.RoleId equals r.Id

            join s in _context.Specialities on d.SpecialityId equals s.Id into spe
            from s in spe.DefaultIfEmpty()
            join c in _context.Clinics on d.ClinicId equals c.Id into cli
            from c in cli.DefaultIfEmpty()
            join p in _context.Patients on u.Id equals p.UserId into pt
            from p in pt.DefaultIfEmpty()*/

            //var patient = _context.Patients;

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName.Contains(request.Keyword)
                 || x.PhoneNumber.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.RoleName))
            {
                query = query.Where(x => x.AppRoles.Name.Contains(request.RoleName));
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    Name = x.AppRoles.Name.ToUpper() == "DOCTOR" ? x.Doctors.LastName+" " +x.Doctors.FirstName : x.AppRoles.Name == "PATIENT" ? x.Patients.FirstOrDefault(x => x.IsPrimary).Name : "Admin",
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    Id = x.Id,
                    Status = x.Status,
                    Img =  x.AppRoles.Name.ToUpper() == "DOCTOR" ? x.Doctors.Img : x.AppRoles.Name == "PATIENT" ? x.Patients.FirstOrDefault(x=>x.IsPrimary).Img : "user_default.png",
                    Date = x.CreatedAt,
                    GetRole = new GetRoleVm()
                    {
                        Id = x.AppRoles.Id,
                        Name =x.AppRoles.Name
                    },
                    DoctorVm = x.AppRoles.Name == "DOCTOR" ? new DoctorVm() 
                    {
                        UserId = x.Doctors.UserId,
                        Intro = x.Doctors.Intro,
                        Address = x.Doctors.Address,
                        Img = x.Doctors.Img,
                        No = x.Doctors.No,
                        //GetSpecialities = x new GetSpecialityVm() { Id = x.Doctors.Specialities.Id , Title = x.Doctors.Specialities.Title },
                        //GetClinic = new GetClinicVm() { Id= x.Doctors.Clinics.Id , Name = x.Doctors.Clinics.Name }
                    }: new DoctorVm(),
                    PatientVm = x.AppRoles.Name == "PATIENT" ? new PatientVm()
                    {
                        UserId = x.Id,
                        Address = x.Patients.Where(x=>x.IsPrimary == true).First().Address,
                        Img = x.Patients.Where(x => x.IsPrimary == true).First().Img
                    }: new PatientVm()
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
                    //Name = x.Name,
                    Id = x.Id,
                    //Gender = x.Gender,
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
            var user = await _userManager.FindByNameAsync(request.Email);

            var role = await _roleManager.FindByNameAsync("doctor");
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await _context.AppUsers.Where(x=>x.Email == request.Email && x.RoleId == role.Id).FirstOrDefaultAsync() != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            int count = await _context.Doctors.Where(x => x.No.Contains("DTD" + year + month)).CountAsync();
            string str = "";
            if (count < 9) str = "DTD" + year + month + "0000" + (count + 1);
            else if (count < 99) str = "DTD" + year + month + "000" + (count + 1);
            else if (count < 999) str = "DTD" + year + month + "00" + (count + 1);
            else if (count < 9999) str = "DTD" + year + month + "0" + (count + 1);
            else if (count < 99999) str = "DTD" + year + month + (count + 1);

            var password = "pw"+ str + new Random().Next(100000,999999) +"$";
            string[] username = request.Email.Split('@');
            user = new AppUsers()
            {
                Email = request.Email,
                UserName = username[0],
                PhoneNumber = request.PhoneNumber,
                Status = Status.Active,
                CreatedAt = DateTime.Now,
                RoleId = role.Id    
            };
            var result = await _userManager.CreateAsync(user, password);
        
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    var rs = await _userManager.AddToRoleAsync(user, role.Name);
                    var location = await _context.Locations.FindAsync(request.SubDistrictId);
                    var district = await _context.Locations.FindAsync(location.ParentId);
                    var province = await _context.Locations.FindAsync(district.ParentId);
                    var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
                    var information = await _context.Informations.FirstOrDefaultAsync();
                    var doctor = new Doctors()
                    {   
                        UserId = user.Id,
                        Dob = request.Dob,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Slug = request.Slug+"-"+str,
                        Booking = false,
                        IsPrimary = false,
                        MapUrl = request.MapUrl,
                        LocationId = request.SubDistrictId,
                        ServicesSpecialities = new List<ServicesSpecialities>(),
                        Gender = request.Gender,
                        Prefix = request.Prefix,
                        View = 0,
                        ClinicId = request.ClinicId,
                        No = str,
                        Address = request.Address,
                        FullAddress = fullAddress,
                        Intro = "<p><strong>Bác sĩ “Nguyễn Văn A” </strong>……….</p><p><strong>Quá trình học tập/Bằng cấp chuyên môn:</strong></p><ul><li>…</li><li>…</li></ul><p><strong>Quá trình công tác:</strong></p><ul><li>…</li><li>…</li></ul><p><strong>Các dịch vụ của phòng khám:</strong></p><ul><li>…</li><li>…</li></ul>",
                        Img = "user_default.png" //await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME)
                    };
                    foreach (var servicesSpeciality in request.SpecialityId)
                    {
                        var servicesSpecialities = new ServicesSpecialities()
                        {
                            IsDelete = false,
                            SpecialityId = servicesSpeciality,
                        };
                        doctor.ServicesSpecialities.Add(servicesSpecialities);
                    }
                    var day = DateTime.Now.ToString("dd") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("YY");
                    count = await _context.AnnualServiceFees.Where(x => x.No.Contains("DMPM" + day)).CountAsync();
                    str = "";
                    if (count < 9) str = "DMPM" + day + "00000" + (count + 1);
                    else if (count < 99) str = "DMPM" + day + "0000" + (count + 1);
                    else if (count < 999) str = "DMPM" + day + "000" + (count + 1);
                    else if (count < 9999) str = "DMPM" + day + "00" + (count + 1);
                    else if (count < 99999) str = "DMPM" + day + "0" + (count + 1);
                    else if (count < 999999) str = "DMPM" + day + (count + 1);
                    doctor.AnnualServiceFees = new List<AnnualServiceFees>();
                    var serviceFee = new AnnualServiceFees()
                    {
                        CreatedAt = DateTime.Now,
                        No = str,
                        NeedToPay = information.ServiceFee,
                        TuitionPaidFreeNumBer = request.PaidtheFee ? information.ServiceFee : 0,
                        InitialAmount = information.ServiceFee,
                        Contingency =  0 ,
                        TuitionPaidFreeText = request.PaidtheFee ? "" : "",
                        PaidDate = request.PaidtheFee ? DateTime.Now : new DateTime(),
                        Type = request.PaidtheFee ? "trực tiếp" : "chưa nộp",
                        Status = request.PaidtheFee ? StatusAppointment.complete : StatusAppointment.pending,
                        Note = request.PaidtheFee ? "Đã thanh toán khi tạo hồ sơ đăng kí thành viên đội ngủ bác sĩ sử dụng dịch vụ." : "",
                    };
                    doctor.AnnualServiceFees.Add(serviceFee);
                    await _context.Doctors.AddAsync(doctor);
                    _context.SaveChanges();
                    user.PasswordHash = password;
                    await GenerateEmailConfirmationTokenAsync(user);
                    return new ApiSuccessResult<bool>();
                    
                }
               

            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }
        public async Task<ApiResult<bool>> RegisterPatient(RegisterEnterProfileRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);

            var role = await _roleManager.FindByNameAsync("patient");
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await _context.AppUsers.Where(x => x.Email == request.Email && x.RoleId == role.Id).FirstOrDefaultAsync() != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            int count = await _context.Patients.Where(x => x.No.Contains("DMP" + year+month)).CountAsync();
            string str = "";
            if (count < 9) str = "DMP" + year + month+ "0000" + (count + 1);
            else if (count < 99) str = "DMP" + year + month + "000" + (count + 1);
            else if (count < 999) str = "DMP" + year + month + "00" + (count + 1);
            else if (count < 9999) str = "DMP" + year + month + "0" + (count + 1);
            else if (count < 99999) str = "DMP" + year + month  + (count + 1);
            user = new AppUsers()
            {
                Email = request.Email,
                UserName = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                Status = Status.Active,
                CreatedAt = DateTime.Now,
                RoleId = role.Id
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    var rs = await _userManager.AddToRoleAsync(user, role.Name);
                    var location = await _context.Locations.FindAsync(request.SubDistrictId);
                    var district = await _context.Locations.FindAsync(location.ParentId);
                    var province = await _context.Locations.FindAsync(district.ParentId);
                    var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
                    var patients = new Patients()
                    {
                        FullAddress = fullAddress,
                        UserId = user.Id,
                        Dob = request.Dob,
                        Name = request.Name,
                        IsPrimary = true,
                        LocationId = request.SubDistrictId,
                        Gender = request.Gender,
                        No = str,
                        Address = request.Address,
                        Img = "user_default.png" ,
                        RelativeName = "Tôi",
                        EthnicId = request.EthnicGroupId,
                        RelativePhone = request.RelativePhone,
                        RelativeRelationshipId = user.Id,
                        Identitycard = request.IdentityCard,
                        RelativeEmail = request.Email
                    };
                    await _context.Patients.AddAsync(patients);
                    _context.SaveChanges();
                    return new ApiSuccessResult<bool>();

                }
            }
            return new ApiErrorResult<bool>("Đăng ký không thành công");
        }
        public async Task GenerateEmailConfirmationTokenAsync(AppUsers user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendEmailConfirmationEmail(user, token);
            }
        }
        private async Task SendEmailConfirmationEmail(AppUsers user, string token)
        {
            string appDomain = _config.GetSection("Application:AppDomain").Value;
            string confirmationLink = _config.GetSection("Application:EmailConfirmation").Value;

            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.UserName),
                    new KeyValuePair<string, string>("{{PasswordHash}}", user.PasswordHash),
                    new KeyValuePair<string, string>("{{Link}}",
                        string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };

            await _emailService.SendEmailForEmailConfirmation(options);
        }
        private async Task<string> SaveFile(IFormFile? file, string folderName)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsyncs(file.OpenReadStream(), fileName, folderName);
            return fileName;
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
                //Dob = DateTime.Now.AddYears(-12),
                //Name = request.Name,
                UserName = request.UserName,
                Status = Status.Active,
                //Date = DateTime.Now
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

        public async Task<ApiResult<bool>> UpdateDoctor(Guid id, UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var doctor = await _context.Doctors.FindAsync(request.Id);
            /*if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }*/
           // string[] username = request.Email.Split('@');
            //user.Email = request.Email;
            //user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status;
            //user.UserName = username[0];

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (doctor != null)
                {
                    /*if (request.ThumbnailImage != null)
                    {
                        if (doctor.Img != null && doctor.Img != "user_default.png")
                        {
                            await _storageService.DeleteFileAsyncs(doctor.Img, USER_CONTENT_FOLDER_NAME);
                        }
                        doctor.Img = await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME);
                    }*/
                    doctor.Address = request.Address;
                    doctor.ClinicId = request.ClinicId;
                    doctor.ServicesSpecialities = new List<ServicesSpecialities>();
                    var spe_service_isdeletes = from s in _context.ServicesSpecialities where s.DoctorId == doctor.UserId && s.IsDelete == false select s;
                    var spe_service_isdelete = new ServicesSpecialities();
                    foreach (var spe in request.Specialities)
                    {
                        var spe_service = _context.ServicesSpecialities.Where(x => x.DoctorId == doctor.UserId).FirstOrDefault(x => x.SpecialityId == spe);

                        if (spe_service != null)
                        {
                            if (spe_service.IsDelete == true)
                            {
                                spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(spe_service.Id);
                                spe_service_isdelete.IsDelete = false;
                            }
                            else
                            {
                                spe_service_isdeletes = spe_service_isdeletes.Where(x => x.SpecialityId != spe);
                            }
                        }
                        else
                        {
                            var add_spe_service = new ServicesSpecialities()
                            {
                                DoctorId = doctor.UserId,
                                IsDelete = false,
                                SpecialityId = spe,
                            };
                            doctor.ServicesSpecialities.Add(add_spe_service);
                        }
                    }
                    foreach (var isdelete in spe_service_isdeletes)
                    {
                        spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(isdelete.Id);
                        spe_service_isdelete.IsDelete = true;
                    }
                    var i = 0;
                   /* var remove_gallery = new Galleries();
                    doctor.Galleries = new List<Galleries>();*/
                    /*if (request.Galleries != null)
                    {
                        *//*var remove_galleries = _context.Galleries.Where(x => x.DoctorId == doctor.UserId).ToList();
                        if (remove_galleries.Count() > 0)
                        {
                            foreach (var item in remove_galleries)
                            {
                                remove_gallery = await _context.Galleries.FindAsync(item.Id);
                                await _storageService.DeleteFileAsyncs(remove_gallery.Img, GALLERY_CONTENT_FOLDER_NAME);
                                _context.Galleries.Remove(remove_gallery);
                            }
                        }*//*
                        foreach (var file in request.Galleries)
                        {
                            var image = new Galleries()
                            {
                                DoctorId = doctor.UserId,
                                Img = await SaveFile(file, GALLERY_CONTENT_FOLDER_NAME),
                                SortOrder = i,
                            };
                            doctor.Galleries.Add(image);
                            i++;
                        }
                    }*/
                    doctor.Intro = WebUtility.HtmlDecode(request.Description);
                    doctor.Services = request.Services;
                    doctor.Prizes = WebUtility.HtmlDecode(request.Prizes);
                    doctor.Note = WebUtility.HtmlDecode(request.Note);
                    doctor.TimeWorking =request.TimeWorking;
                    doctor.Educations = WebUtility.HtmlDecode(request.Educations);
                    doctor.FirstName = request.FirstName;
                    doctor.LastName = request.LastName;
                    doctor.Gender = request.Gender;
                    doctor.Dob = request.Dob;
                    doctor.LocationId = request.SubDistrictId;
                    doctor.Booking = request.Booking;
                    doctor.MapUrl = request.MapUrl;
                    doctor.Slug = request.Slug;
                    doctor.IsPrimary = request.IsPrimary;
                    doctor.Prefix = request.Prefix;
                }
                var rs_dt = await _context.SaveChangesAsync();
                if (rs_dt != 0) return new ApiSuccessResult<bool>(true);
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public async Task<ApiResult<bool>> DoctorUpdateRequest(Guid id, DoctorUpdateRequest request)
        {
            var doctor = await _context.Doctors.FindAsync(request.Id);
            
                if (doctor != null)
                {
                    if (request.ThumbnailImage != null)
                    {
                        if (doctor.Img != null && doctor.Img != "user_default.png")
                        {
                            await _storageService.DeleteFileAsyncs(doctor.Img, USER_CONTENT_FOLDER_NAME);
                        }
                        doctor.Img = await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME);
                    }
                    doctor.Address = request.Address;
                    doctor.ClinicId = request.ClinicId;
                    doctor.ServicesSpecialities = new List<ServicesSpecialities>();
                    var spe_service_isdeletes = from s in _context.ServicesSpecialities where s.DoctorId == doctor.UserId && s.IsDelete == false select s;
                    var spe_service_isdelete = new ServicesSpecialities();
                    foreach (var spe in request.Specialities)
                    {
                        var spe_service = _context.ServicesSpecialities.Where(x => x.DoctorId == doctor.UserId).FirstOrDefault(x => x.SpecialityId == spe);

                        if (spe_service != null)
                        {
                            if (spe_service.IsDelete == true)
                            {
                                spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(spe_service.Id);
                                spe_service_isdelete.IsDelete = false;
                            }
                            else
                            {
                                spe_service_isdeletes = spe_service_isdeletes.Where(x => x.SpecialityId != spe);
                            }
                        }
                        else
                        {
                            var add_spe_service = new ServicesSpecialities()
                            {
                                DoctorId = doctor.UserId,
                                IsDelete = false,
                                SpecialityId = spe,
                            };
                            doctor.ServicesSpecialities.Add(add_spe_service);
                        }
                    }
                    foreach (var isdelete in spe_service_isdeletes)
                    {
                        spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(isdelete.Id);
                        spe_service_isdelete.IsDelete = true;
                    }
                    var i = 0;
                    var remove_gallery = new Galleries();
                    doctor.Galleries = new List<Galleries>();
                    if (request.Galleries != null)
                    {
                        foreach (var file in request.Galleries)
                        {
                            var image = new Galleries()
                            {
                                DoctorId = doctor.UserId,
                                Img = await SaveFile(file, GALLERY_CONTENT_FOLDER_NAME),
                                SortOrder = i,
                            };
                            doctor.Galleries.Add(image);
                            i++;
                        }
                    }
                    doctor.Intro = WebUtility.HtmlDecode(request.Description);
                    doctor.Services = WebUtility.HtmlDecode(request.Services);
                    doctor.Prizes = WebUtility.HtmlDecode(request.Prizes);
                    doctor.Note = WebUtility.HtmlDecode(request.Note);
                    doctor.TimeWorking = WebUtility.HtmlDecode(request.TimeWorking);
                    doctor.Educations = WebUtility.HtmlDecode(request.Educations);
                    doctor.FirstName = request.FirstName;
                    doctor.LastName = request.LastName;
                    doctor.Gender = request.Gender;
                    doctor.Dob = request.Dob;
                    doctor.LocationId = request.SubDistrictId;
                    doctor.Booking = request.Booking;
                    doctor.MapUrl = request.MapUrl;
                    doctor.Slug = request.Slug;
                    doctor.Prefix = request.Prefix;
                }
                var rs_dt = await _context.SaveChangesAsync();
                if (rs_dt != 0) return new ApiSuccessResult<bool>(true);
           
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public async Task<ApiResult<bool>> DoctorUpdateProfile(DoctorUpdateProfile request)
        {
            var doctor = await _context.Doctors.FindAsync(request.Id);

            if (doctor == null) return new ApiErrorResult<bool>(new string[] {"danger", "Tài Khoản không tồn tại!!!" });
            doctor.Address = request.Address;
            doctor.ClinicId = request.ClinicId;
            doctor.ServicesSpecialities = new List<ServicesSpecialities>();
            var spe_service_isdeletes = from s in _context.ServicesSpecialities where s.DoctorId == doctor.UserId && s.IsDelete == false select s;
            var spe_service_isdelete = new ServicesSpecialities();
            foreach (var spe in request.Specialities)
            {
                var spe_service = _context.ServicesSpecialities.Where(x => x.DoctorId == doctor.UserId).FirstOrDefault(x => x.SpecialityId == spe);

                if (spe_service != null)
                {
                    if (spe_service.IsDelete == true)
                    {
                        spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(spe_service.Id);
                        spe_service_isdelete.IsDelete = false;
                    }
                    else
                    {
                        spe_service_isdeletes = spe_service_isdeletes.Where(x => x.SpecialityId != spe);
                    }
                }
                else
                {
                    var add_spe_service = new ServicesSpecialities()
                    {
                        DoctorId = doctor.UserId,
                        IsDelete = false,
                        SpecialityId = spe,
                    };
                    doctor.ServicesSpecialities.Add(add_spe_service);
                }
            }
            foreach (var isdelete in spe_service_isdeletes)
            {
                spe_service_isdelete = await _context.ServicesSpecialities.FindAsync(isdelete.Id);
                spe_service_isdelete.IsDelete = true;
            }
            doctor.Intro = WebUtility.HtmlDecode(request.Description);
            doctor.Services = WebUtility.HtmlDecode(request.Services);
            doctor.Prizes = WebUtility.HtmlDecode(request.Prizes);
            doctor.Note = WebUtility.HtmlDecode(request.Note);
            doctor.TimeWorking = WebUtility.HtmlDecode(request.TimeWorking);
            doctor.Educations = WebUtility.HtmlDecode(request.Educations);
            doctor.FirstName = request.FirstName;
            doctor.LastName = request.LastName;
            doctor.Gender = request.Gender;
            doctor.Dob = request.Dob;
            doctor.LocationId = request.SubDistrictId;
            doctor.Booking = request.Booking;
            doctor.BeforeBookingDay = request.BeforeBookingDay;
            doctor.MapUrl = request.MapUrl;
            doctor.Slug = request.Slug;
            doctor.Prefix = request.Prefix;
            var rs_dt = await _context.SaveChangesAsync();
            if (rs_dt != 0) return new ApiSuccessResult<bool>(true);

            return new ApiErrorResult<bool>(new string[]{ "warning","Cập nhật không thành công!, Dữ liệu không thay đổi."});
        }
        public async Task<ApiResult<bool>> UpdateAdmin(UserUpdateAdminRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            //user.Dob = request.Dob;
            //user.Gender = request.Gender;
            user.Email = request.Email;
            //user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status ;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public async Task<ApiResult<bool>> UpdatePatient(Guid id, UserUpdatePatientRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var patient = await _context.Patients.FindAsync(user.Id);
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }

            //user.Dob = request.Dob;
            user.Email = request.Email;
            //user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status ;
            //user.Gender = request.Gender;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (patient != null)
                {
                    if (request.ThumbnailImage != null)
                    {
                        if (patient.Img != null)
                        {
                            await _storageService.DeleteFileAsync(patient.Img);
                        }
                        patient.Img = await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME);
                       
                    }
                    patient.Address = request.Address;
                    _context.SaveChanges();
                }
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
        }

        public Task<ApiResult<PagedResult<UserVm>>> GetDoctorAllPaging(GetUserPagingRequest request)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResult<List<EthnicVm>>> GetAllEthnicGroup()
        {
            var query = _context.Ethnics;

            var rs = await query.Select(x => new EthnicVm()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
            return new ApiSuccessResult<List<EthnicVm>>(rs);
        }
    }
}
