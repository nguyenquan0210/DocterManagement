using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
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
                             select new {s,spe};
            var rates = from r in _context.Rates
                        join a in _context.Appointments on r.AppointmentId equals a.Id
                        select new {r,a};

            if (doctor != null)
            {
                specialities = specialities.Where(x => x.s.DoctorId == doctor.UserId);
                rates = rates.Where(x => x.a.DoctorId == doctor.UserId);
            }
            var patient = await _context.Patients.FindAsync(user.Id);
            var ethnic = await _context.Ethnics.FindAsync(patient != null ? patient.EthnicId : new Guid());
            if(doctor != null)
            {
                location = await _context.Locations.FindAsync(doctor.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            else
            {
                location = await _context.Locations.FindAsync(patient.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            //var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                UserName = user.UserName,
                Status = user.Status,
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
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    GetClinic = new GetClinicVm(){ Id = clinic.Id, Name = clinic.Name},
                    GetSpecialities = specialities.Select(x => new GetSpecialityVm() { Id = x.s.Id, Title = x.spe.Title }).ToList(),
                    Rates = rates.Select(x => new RateVm() { Id = x.r.Id, Rating = x.r.Rating }).ToList(),
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
                               select new { s, spe };
            var rates = from r in _context.Rates
                        join a in _context.Appointments on r.AppointmentId equals a.Id
                        select new { r, a };

            if (doctor != null)
            {
                specialities = specialities.Where(x => x.s.DoctorId == doctor.UserId);
                rates = rates.Where(x => x.a.DoctorId == doctor.UserId);
            }
            var patient = await _context.Patients.FindAsync(user.Id);
            var ethnic = await _context.Ethnics.FindAsync(patient != null ? patient.EthnicId : new Guid());
            if (doctor != null)
            {
                location = await _context.Locations.FindAsync(doctor.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            else
            {
                location = await _context.Locations.FindAsync(patient.LocationId);
                district = await _context.Locations.FindAsync(location.ParentId);
                province = await _context.Locations.FindAsync(district.ParentId);
            }
            //var roles = await _userManager.GetRolesAsync(user);
            var userVm = new UserVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                UserName = user.UserName,
                Status = user.Status,
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
                    Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
                    GetClinic = new GetClinicVm() { Id = clinic.Id, Name = clinic.Name },
                    GetSpecialities = specialities.Select(x => new GetSpecialityVm() { Id = x.s.Id, Title = x.spe.Title }).ToList(),
                    Rates = rates.Select(x => new RateVm() { Id = x.r.Id, Rating = x.r.Rating }).ToList(),
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
                        join d in _context.Doctors on u.Id equals d.UserId into dt
                        from d in dt.DefaultIfEmpty()
                        join s in _context.ServicesSpecialities on d.UserId equals s.DoctorId into ss
                        from s in ss.DefaultIfEmpty()
                        join sp in _context.Specialities on s.SpecialityId equals sp.Id into spe
                        from sp in spe.DefaultIfEmpty()
                            /*join r in _context.Roles on u.RoleId equals r.Id

                            join s in _context.Specialities on d.SpecialityId equals s.Id into spe
                            from s in spe.DefaultIfEmpty()
                            join c in _context.Clinics on d.ClinicId equals c.Id into cli
                            from c in cli.DefaultIfEmpty()
                            join p in _context.Patients on u.Id equals p.UserId into pt
                            from p in pt.DefaultIfEmpty()*/
                        select new {u, sp};
            
            //var patient = _context.Patients;

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.u.UserName.Contains(request.Keyword)
                 || x.u.PhoneNumber.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.RoleName) && request.RoleName.ToUpper() != "ALL")
            {
                query = query.Where(x => x.u.AppRoles.Name.Contains(request.RoleName));
            }

            //3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.u.Email,
                    PhoneNumber = x.u.PhoneNumber,
                    UserName = x.u.UserName,
                    Id = x.u.Id,
                    Status = x.u.Status,
                    //Img =  x.u.AppRoles.Name.ToUpper() == "DOCTOR" ? x.Doctors.Img : x.AppRoles.Name == "PATIENT" ? x.Patients.Img : "user_default.png",
                    Date = x.u.CreatedAt,
                    GetRole = new GetRoleVm()
                    {
                        Id = x.u.AppRoles.Id,
                        Name =x.u.AppRoles.Name
                    },
                    DoctorVm = x.u.AppRoles.Name == "DOCTOR" ? new DoctorVm() 
                    {
                        UserId = x.u.Doctors.UserId,
                        Intro = x.u.Doctors.Intro,
                        Address = x.u.Doctors.Address,
                        Img = x.u.Doctors.Img,
                        No = x.u.Doctors.No,
                        //GetSpecialities = x new GetSpecialityVm() { Id = x.Doctors.Specialities.Id , Title = x.Doctors.Specialities.Title },
                        //GetClinic = new GetClinicVm() { Id= x.Doctors.Clinics.Id , Name = x.Doctors.Clinics.Name }
                    }: new DoctorVm(),
                    PatientVm = x.u.AppRoles.Name == "PATIENT" ? new PatientVm()
                    {
                        UserId = x.u.Id,
                        Address = x.u.Patients.Where(x=>x.IsPrimary == true).First().Address,
                        Img = x.u.Patients.Where(x => x.IsPrimary == true).First().Img
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
            var user = await _userManager.FindByNameAsync(request.UserName);
            var role = await _roleManager.FindByNameAsync("doctor");
            if (user != null)
            {
                return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            string year = DateTime.Now.ToString("yy");
            int count = await _context.Doctors.Where(x => x.No.Contains("DT-" + year)).CountAsync();
            string str = "";
            if(count<9) str = "DT-" + DateTime.Now.ToString("yy") + "-00" + (count + 1);
            else if(count<99) str = "DT-" + DateTime.Now.ToString("yy") + "-0" + (count + 1);
            else if(count<999) str = "DT-" + DateTime.Now.ToString("yy") + "-" + (count + 1);

            user = new AppUsers()
            {
                //Dob = request.Dob,
                Email = request.Email,
                //Name = request.Name,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Status = Status.Active,
                //Date = DateTime.Now,
                RoleId = role.Id
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name) == false)
                {
                    var rs = await _userManager.AddToRoleAsync(user, role.Name);

                    if (rs.Succeeded)
                    {
                        var userIdRs = await _userManager.FindByNameAsync(user.UserName);
                        var doctor = new Doctors()
                        {
                            UserId = userIdRs.Id,
                            //SpecialityId = request.SpecialityId,
                            ClinicId = request.ClinicId,
                            No = str,
                            Address = request.Address,
                            Intro = "<p><strong>Bác sĩ “Nguyễn Văn A” </strong>……….</p><p><strong>Quá trình học tập/Bằng cấp chuyên môn:</strong></p><ul><li>…</li><li>…</li></ul><p><strong>Quá trình công tác:</strong></p><ul><li>…</li><li>…</li></ul><p><strong>Các dịch vụ của phòng khám:</strong></p><ul><li>…</li><li>…</li></ul>",
                            Img = await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME)
                        };
                        await _context.Doctors.AddAsync(doctor);
                        _context.SaveChanges();
                        user.PasswordHash = request.Password;
                        await GenerateEmailConfirmationTokenAsync(user);
                        return new ApiSuccessResult<bool>();
                    }
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

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var doctor = await _context.Doctors.FindAsync(user.Id);
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != id))
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }

            //user.Dob = request.Dob;
            user.Email = request.Email;
            //user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status;
            //user.Gender = request.Gender;
           
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (doctor != null)
                {
                    if (request.ThumbnailImage != null)
                    {
                        if (doctor.Img != null)
                        {
                            await _storageService.DeleteFileAsyncs(doctor.Img, USER_CONTENT_FOLDER_NAME);
                        }
                        doctor.Img = await this.SaveFile(request.ThumbnailImage, USER_CONTENT_FOLDER_NAME);
                    }
                    doctor.Address = request.Address;
                    doctor.ClinicId = request.ClinicId;
                    //doctor.SpecialityId = request.SpecialityId;
                    //doctor.Description = WebUtility.HtmlDecode(request.Description);
                    _context.SaveChanges();
                }
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Cập nhật không thành công");
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
    }
}
