using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
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
        
        private readonly DoctorManageDbContext _context;
        private readonly IConfiguration _config;
        private const string GALLERY_CONTENT_FOLDER_NAME = "gallery-content";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public DoctorService(
            IConfiguration config,
            DoctorManageDbContext context)
        {
            _config = config;
            _context = context;
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
    }
}
