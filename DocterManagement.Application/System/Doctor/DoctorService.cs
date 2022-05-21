using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
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
        private readonly DoctorManageDbContext _context;
        private readonly IConfiguration _config;
        private const string GALLERY_CONTENT_FOLDER_NAME = "gallery-content";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public DoctorService(UserManager<AppUsers> userManager,
            IConfiguration config,
            DoctorManageDbContext context)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
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
                GetClinic = new GetClinicVm()
                {
                    Id = x.ClinicId,
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
                Services = doctor.Services,
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
                Name = x.p.Name,
                Address = x.p.Address,
                FullAddress = x.p.Address + ", " + x.sd.Name+", " + x.d.Name + ", " + x.pro.Name ,
                RelativePhone = x.p.RelativePhone,
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
            if (patient == null) return new ApiErrorResult<bool>("");

            patient.LocationId = request.LocationId;
            patient.Name = request.Name;
            patient.Address = request.Address;
            patient.Identitycard = request.Identitycard;
            patient.Dob = request.Dob;
            patient.Gender = request.Gender;
            patient.EthnicId = request.EthnicId;
            patient.RelativeName = request.RelativeName;
            patient.RelativePhone = request.RelativePhone;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("");
        }

        public async Task<ApiResult<bool>> AddInfo(AddPatientInfoRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var patiens = _context.Patients.Where(x => x.IsPrimary && x.UserId == user.Id);
            if (user == null) return new ApiErrorResult<bool>("Tài khoản tạo hồ sơ không được phép!");
            string year = DateTime.Now.ToString("yy");
            string month = DateTime.Now.ToString("MM");
            int count = await _context.Patients.Where(x => x.No.Contains("DMP" + year + month)).CountAsync();
            string str = "";
            if (count < 9) str = "DMP" + year + month + "0000" + (count + 1);
            else if (count < 99) str = "DMP" + year + month + "000" + (count + 1);
            else if (count < 999) str = "DMP" + year + month + "00" + (count + 1);
            else if (count < 9999) str = "DMP" + year + month + "0" + (count + 1);
            else if (count < 99999) str = "DMP" + year + month + (count + 1);
            var patient = new Patients()
            {
                Address = request.Address,
                Identitycard = request.Identitycard,
                Dob = request.Dob,
                Gender = request.Gender,
                Name = request.Name,
                LocationId = request.LocationId,
                EthnicId = request.EthnicId,
                RelativeName = request.RelativeName,
                RelativePhone = request.RelativePhone,
                UserId = user.Id,
                No = str,
                IsPrimary = false,
                RelativeRelationshipId = patiens.FirstOrDefault().PatientId
            };
            await _context.Patients.AddAsync(patient);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Thêm hồ sơ bệnh không thành công!");
        }
    }
}
