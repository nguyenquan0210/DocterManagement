using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using DoctorManagement.ViewModels.System.Roles;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.Doctor
{
    public class DoctorService : IDoctorService
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly RoleManager<AppRoles> _roleManager;
        private readonly DoctorManageDbContext _context;
        private readonly IConfiguration _config;
        private const string GALLERY_CONTENT_FOLDER_NAME = "gallery-content";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public DoctorService(UserManager<AppUsers> userManager,
            IConfiguration config,
            DoctorManageDbContext context, RoleManager<AppRoles> roleManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResult<List<DoctorVm>>> GetTopFavouriteDoctors()
        {
            var query = from d in _context.Doctors
                        select d;
            var doctors = await query.Select(x => new DoctorVm()
            {
                UserId = x.UserId,
                Img = x.Img,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Prefix = x.Prefix,
                GetSpecialities= x.ServicesSpecialities.Select(x=> new GetSpecialityVm()
                {
                    Id = x.SpecialityId,
                    Title = x.Specialities.Title
                }).ToList(),
                GetClinic = x.ClinicId==null? new GetClinicVm() : new GetClinicVm()
                {
                    Id = x.ClinicId.Value,
                    Name = x.Clinics.Name
                }
               
            }).ToListAsync();
            return new ApiSuccessResult<List<DoctorVm>>(doctors);
        }
        public async Task<ApiResult<DoctorVm>> GetById(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return new ApiErrorResult<DoctorVm>("User không tồn tại");
            }
            var clinic = await _context.Clinics.FindAsync(doctor != null ? doctor.ClinicId : new Guid());
            var specialities = from s in _context.ServicesSpecialities
                               join spe in _context.Specialities on s.SpecialityId equals spe.Id
                               where s.IsDelete == false
                               where s.DoctorId == doctor.UserId
                               select new { s, spe };
            var rates = from r in _context.Rates
                        join a in _context.Appointments on r.AppointmentId equals a.Id
                        where a.DoctorId == doctor.UserId
                        select new { r, a };

            var galleries = _context.Galleries.Where(x => x.DoctorId == doctor.UserId);
            var service = from ser in _context.Services where ser.DoctorId == doctor.UserId select ser;
            var location = await _context.Locations.FindAsync(doctor.LocationId);
            var district = await _context.Locations.FindAsync(location.ParentId);
            var province = await _context.Locations.FindAsync(district.ParentId);
            
            var fulladdreess = doctor.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name ;
            //var roles = await _userManager.GetRolesAsync(user);
            var doctorVm = new DoctorVm()
            {
                UserId = doctor.UserId,
                FirstName = doctor.FirstName,
                Intro = doctor.Intro,
                Address = doctor.Address,
                FullAddress = fulladdreess,
                Img = USER_CONTENT_FOLDER_NAME + "/" + doctor.Img,
                No = doctor.No,
                Services = service.Select(s => new ServiceVm()
                {
                    Id = s.Id,
                    Description = s.Description,
                    ServiceName = s.ServiceName,
                    Price = s.Price,
                }).ToList(),
                Slug = doctor.Slug,
                Booking = doctor.Booking,
                Dob = doctor.Dob,
                Educations = doctor.Educations,
                Experiences = doctor.Experiences,
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
                GetClinic = new GetClinicVm() { Id = clinic.Id, Name = clinic.Name },
                GetSpecialities = specialities.Select(x => new GetSpecialityVm() { Id = x.spe.Id, Title = x.spe.Title }).ToList(),
                Rates = rates.Select(x => new RateVm() { Id = x.r.Id, Rating = x.r.Rating }).ToList(),
                Galleries = galleries.Select(x => new GalleryVm() { Id = x.Id, Name = GALLERY_CONTENT_FOLDER_NAME + "/" + x.Img }).ToList(),
            };
            return new ApiSuccessResult<DoctorVm>(doctorVm);
        }

        public async Task<ApiResult<PatientVm>> GetByPatientId(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return new ApiErrorResult<PatientVm>("User không tồn tại");
            }
            var location = await _context.Locations.FindAsync(patient.LocationId);
            var district = await _context.Locations.FindAsync(location.ParentId);
            var province = await _context.Locations.FindAsync(district.ParentId);
            var ethnics = await _context.Ethnics.FindAsync(patient.EthnicId);
            var fulladdreess = patient.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
            //var roles = await _userManager.GetRolesAsync(user);
            var patientvm = new PatientVm()
            {
                UserId = patient.UserId,
                Name = patient.Name,
                Address = patient.Address,
                FullAddress = fulladdreess,
                Img = USER_CONTENT_FOLDER_NAME + "/" + patient.Img,
                No = patient.No,
                Dob = patient.Dob,
                Gender = patient.Gender,
                IsPrimary = patient.IsPrimary,
                RelativePhone = patient.RelativePhone,
                RelativeName = patient.RelativeName,
                RelativeEmail = patient.RelativeEmail,
                EthnicId = patient.EthnicId,
                Id = patient.PatientId,
                Identitycard = patient.Identitycard,
                Ethnics = new EthnicVm()
                {
                    Id=ethnics.Id,
                    Name=ethnics.Name,
                },
                Location = new LocationVm() { Id = location.Id, Name = location.Name, District = new DistrictVm() { Id = district.Id, Name = district.Name, Province = new ProvinceVm() { Id = province.Id, Name = province.Name } } },
               
            };
            return new ApiSuccessResult<PatientVm>(patientvm);
        }
        public async Task<ApiResult<List<PatientVm>>> GetPatientProfile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var query = from p in _context.Patients
                        join e in _context.Ethnics on p.EthnicId equals e.Id
                        join sd in _context.Locations on p.LocationId equals sd.Id
                        join d in _context.Locations on sd.ParentId equals d.Id
                        join pro in _context.Locations on d.ParentId equals pro.Id
                        where p.UserId == user.Id
                        select new {p, e, sd, d,pro};
            var patients = await query.Select(x => new PatientVm()
            {
                No= x.p.No,
                Id = x.p.PatientId,
                UserId = x.p.UserId,
                Img = x.p.Img,
                Text = "Bn",
                DobText = x.p.Dob.ToShortDateString(),
                GenderText = x.p.Gender == Data.Enums.Gender.Male?"Nam":"Nữ",
                RelativeName = x.p.RelativeName,
                Name = x.p.Name,
                Address = x.p.Address,
                FullAddress = x.p.Address + ", " + x.sd.Name+", " + x.d.Name + ", " + x.pro.Name ,
                RelativePhone = x.p.RelativePhone,
                RelativeEmail = x.p.RelativeEmail,
                Dob = x.p.Dob,
                Gender = x.p.Gender,
                Identitycard = x.p.Identitycard,
                Ethnics = new EthnicVm()
                {
                    Id = x.e.Id,
                    Name = x.e.Name
                },
                IsPrimary = x.p.IsPrimary,
            }).ToListAsync();
            return new ApiSuccessResult<List<PatientVm>>(patients);
        }

        public async Task<ApiResult<bool>> UpdateInfo(UpdatePatientInfoRequest request)
        {
            var patient = await _context.Patients.FindAsync(request.Id);
            var role = await _roleManager.FindByNameAsync("patient");
            var location = await _context.Locations.FindAsync(request.LocationId);
            var district = await _context.Locations.FindAsync(location.ParentId);
            var province = await _context.Locations.FindAsync(district.ParentId);
            var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
            if (patient == null) return new ApiErrorResult<bool>("Tài khoản này không xác thực!");
            var user = await _userManager.FindByIdAsync(patient.UserId.ToString());
            var check = await _context.AppUsers.Where(x => x.Email == request.RelativeEmail && x.Id != patient.UserId && x.RoleId == role.Id).ToListAsync();
            if (check.FirstOrDefault() != null)
            {
                return new ApiErrorResult<bool>("Emai đã tồn tại");
            }
            if (patient.IsPrimary && request.RelativeEmail != patient.RelativeEmail)
            {
                
                await _userManager.ChangeEmailAsync(user,request.RelativeEmail, await _userManager.GenerateChangeEmailTokenAsync(user, request.RelativeEmail));
                patient.RelativeEmail = request.RelativeEmail;
            }
            patient.LocationId = request.LocationId;
            patient.Name = request.Name;
            patient.FullAddress = fullAddress;
            patient.Address = request.Address;
            patient.Identitycard = request.Identitycard;
            patient.Dob = request.Dob;
            patient.Gender = request.Gender;
            patient.EthnicId = request.EthnicId;
            patient.RelativeName = request.RelativeName;
            patient.RelativePhone = request.RelativePhone;
          
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Thông tin của bạn không thay đổi?");
        }

        public async Task<ApiResult<Guid>> AddInfo(AddPatientInfoRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var patiens = _context.Patients.Where(x => x.IsPrimary && x.UserId == user.Id);
            if (user == null) return new ApiErrorResult<Guid>("Tài khoản tạo hồ sơ không được phép!");
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            int count = await _context.Patients.Where(x => x.No.Contains("DMP" + year + month)).CountAsync();
            string str = "";
            if (count < 9) str = "DMP" + year + month + "0000" + (count + 1);
            else if (count < 99) str = "DMP" + year + month + "000" + (count + 1);
            else if (count < 999) str = "DMP" + year + month + "00" + (count + 1);
            else if (count < 9999) str = "DMP" + year + month + "0" + (count + 1);
            else if (count < 99999) str = "DMP" + year + month + (count + 1);
            var location = await _context.Locations.FindAsync(request.LocationId);
            var district = await _context.Locations.FindAsync(location.ParentId);
            var province = await _context.Locations.FindAsync(district.ParentId);
            var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
            var patient = new Patients()
            {
                FullAddress = fullAddress,
                Address = request.Address,
                Identitycard = request.Identitycard,
                Dob = request.Dob,
                Gender = request.Gender,
                Name = request.Name,
                LocationId = request.LocationId,
                EthnicId = request.EthnicId,
                RelativeName = request.RelativeName,
                RelativePhone = request.RelativePhone,
                RelativeEmail = request.RelativeEmail,
                UserId = user.Id,
                No = str,
                IsPrimary = false,
                RelativeRelationshipId = patiens.FirstOrDefault().PatientId
            };
            await _context.Patients.AddAsync(patient);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<Guid>(patient.PatientId);
            return new ApiErrorResult<Guid>("Thêm hồ sơ bệnh không thành công!");
        }

        public async Task<ApiResult<List<UserVm>>> GetAllUser(string? role)
        {
            var query = from u in _context.AppUsers
                        select u;

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(x => x.AppRoles.Name.Contains(role));
            }

            //3. Paging

            var data = await query.Select(x => new UserVm()
                {
                    Email = x.Email,
                    Name = x.AppRoles.Name.ToUpper() == "DOCTOR" ? x.Doctors.LastName + " " + x.Doctors.FirstName : x.AppRoles.Name == "PATIENT" ? x.Patients.FirstOrDefault(x => x.IsPrimary).Name : "Admin",
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    Id = x.Id,
                    Status = x.Status,
                    Img = x.AppRoles.Name.ToUpper() == "DOCTOR" ? x.Doctors.Img : x.AppRoles.Name == "PATIENT" ? x.Patients.FirstOrDefault(x => x.IsPrimary).Img : "user_default.png",
                    Date = x.CreatedAt,
                    GetRole = new GetRoleVm()
                    {
                        Id = x.AppRoles.Id,
                        Name = x.AppRoles.Name
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
                    } : new DoctorVm(),
                    PatientVm = x.AppRoles.Name == "PATIENT" ? new PatientVm()
                    {
                        UserId = x.Id,
                        Address = x.Patients.FirstOrDefault(x => x.IsPrimary == true).Address,
                        Img = x.Patients.FirstOrDefault(x => x.IsPrimary == true).Img,
                        Dob = x.Patients.FirstOrDefault(x => x.IsPrimary == true).Dob,
                        Gender = x.Patients.FirstOrDefault(x => x.IsPrimary == true).Gender,
                        FullAddress = x.Patients.FirstOrDefault(x => x.IsPrimary == true).FullAddress,
                        Name = x.Patients.FirstOrDefault(x => x.IsPrimary == true).Name,
                        No = x.Patients.FirstOrDefault(x => x.IsPrimary == true).No,
                    } : new PatientVm()
                }).ToListAsync();

            //4. Select and projection
           
            return new ApiSuccessResult<List<UserVm>>(data);
        }
    }
}
