using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Exceptions;
using DoctorManagement.ViewModels.Catalog.Clinic;
using DoctorManagement.ViewModels.Catalog.Location;
using DoctorManagement.ViewModels.Catalog.Service;
using DoctorManagement.ViewModels.Catalog.Speciality;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.Common.Extensions;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Clinic
{
    public class ClinicService : IClinicService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string CLINIC_CONTENT_FOLDER_NAME = "clinic-content";
        private const string CLINICS_CONTENT_FOLDER_NAME = "clinics-content";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        public ClinicService(DoctorManageDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<ApiResult<bool>> Create(ClinicCreateRequest request)
        {
            string year = DateTime.Now.ToString("yy");
            int count = await _context.Clinics.Where(x => x.No.Contains("DTMC" + year)).CountAsync();
            string str = "";
            if (count < 9) str = "DTMC" + DateTime.Now.ToString("yy") + "-00" + (count + 1);
            else if (count < 99) str = "DTMC" + DateTime.Now.ToString("yy") + "-0" + (count + 1);
            else if (count < 999) str = "DTMC" + DateTime.Now.ToString("yy") + "-" + (count + 1);
            var location = await _context.Locations.FindAsync(request.LocationId);
            var district = await _context.Locations.FindAsync(location.ParentId);
            var province = await _context.Locations.FindAsync(district.ParentId);
            var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
            var clinics = new Clinics()
            {
                Name = request.Name,
                ImgLogo = await SaveFile(request.ImgLogo, CLINIC_CONTENT_FOLDER_NAME),
                Description = WebUtility.HtmlDecode(request.Description),
                Address = request.Address ,
                FullAddress = fullAddress,
                LocationId = request.LocationId,
                Status = Status.Active,
                No = str,
                Note = request.Note,
                MapUrl = request.MapUrl,
                Service = request.Service,
                Contact = request.Contact,
                CreatedAt = DateTime.Now,
            };
            var i = 0;
            if (request.ImgClinics != null)
            {
                clinics.ImageClinics = new List<ImageClinics>();

                foreach (var file in request.ImgClinics)
                {
                    var image = new ImageClinics()
                    {
                        Img = await SaveFile(file, CLINICS_CONTENT_FOLDER_NAME),
                        SortOrder = i + 1
                    };
                    clinics.ImageClinics.Add(image);
                }
            }
           _context.Clinics.Add(clinics);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Tạo phòng khám không thành công!");
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
        public async Task<ApiResult<int>> Delete(Guid Id)
        {
            var clinics = await _context.Clinics.FindAsync(Id);
            int check = 0;
            if (clinics == null) return new ApiSuccessResult<int>(check);

            if (clinics.Status == Status.Active)
            {
                clinics.Status = Status.InActive;
                check = 1;
            }
            else
            {
                //_context.Clinics.Remove(clinics);
                clinics.Status = Status.NotActivate;
                check = 2;
            }

            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check); ;
        }
        public async Task<ApiResult<int>> DeleteImg(Guid Id)
        {
            var image = await _context.ImageClinics.FindAsync(Id);
            int check = 0;
            if (image == null) return new ApiSuccessResult<int>(check);
            
            await _storageService.DeleteFileAsyncs(image.Img,CLINICS_CONTENT_FOLDER_NAME);
            _context.ImageClinics.Remove(image);
            check = 2;
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check); 
        }
        public async Task<ApiResult<int>> DeleteAllImg(Guid Id)
        {
            var images = _context.ImageClinics.Where(x=>x.ClinicId == Id).ToList();
            int check = 0;
            if (images.Count == 0) return new ApiSuccessResult<int>(check);
            foreach (var image in images)
            {
                var removeImg = await _context.ImageClinics.FindAsync(image.Id);
                await _storageService.DeleteFileAsyncs(removeImg.Img, CLINICS_CONTENT_FOLDER_NAME);
                _context.ImageClinics.Remove(removeImg);
            }
            var rs = await _context.SaveChangesAsync();
            if (rs != 0)
            {
                check = 2;
            }
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<ClinicVm>>> GetAll()
        {
            var query = _context.Clinics;

            var rs =  await query.Select(x => new ClinicVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ImgLogo = x.ImgLogo,
                Address = x.Address,
                Status = x.Status
            }).ToListAsync();
            return new ApiSuccessResult<List<ClinicVm>>(rs); ;
        }

        public async Task<ApiResult<PagedResult<ClinicVm>>> GetAllPaging(GetClinicPagingRequest request)
        {
            var query = from c in _context.Clinics
                        join sd in _context.Locations on c.LocationId equals sd.Id
                        join d in _context.Locations on sd.ParentId equals d.Id
                        join p in _context.Locations on d.ParentId equals p.Id
                        select new { c, sd, d, p};
            var img = from i in _context.ImageClinics select i;
            var checkdocter = await _context.Doctors.ToListAsync();
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                foreach (var item in checkdocter)
                {
                    var checkspe = from d in _context.Doctors
                                   join sd in _context.ServicesSpecialities on d.UserId equals sd.DoctorId
                                   join s in _context.Specialities on sd.SpecialityId equals s.Id
                                   where d.UserId == item.UserId
                                   select new { sd, s, d };
                    var i = 0;
                    foreach (var spe in checkspe)
                    {

                        if (spe.s.Title.ToLower().Contains(request.Keyword.ToLower()) )
                        {
                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        checkdocter = checkdocter.Where(x => x.UserId != item.UserId).ToList();
                    }
                }
                foreach (var clinic in query)
                {
                    var i = 0;
                    if (clinic.c.Doctors != null)
                    {
                        foreach (var d in clinic.c.Doctors)
                        {

                            var cd = checkdocter.FirstOrDefault(x => x.UserId == d.UserId);
                            if (cd != null || clinic.c.Name.Contains(request.Keyword))
                            {
                                i++;
                            }
                        }

                    }
                   
                    if (i == 0)
                    {
                        query = query.Where(x => x.c.Id != clinic.c.Id);
                    }
                }
                
                
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ClinicVm()
                {
                    Name = x.c.Name,
                    Description = x.c.Description,
                    Id = x.c.Id,
                    ImgLogo = CLINIC_CONTENT_FOLDER_NAME + "/" + x.c.ImgLogo,
                    Status = x.c.Status,
                    Address =  x.c.Address,
                    No = x.c.No,
                    FullAddress = x.c.Address + ", " + x.sd.Name + ", " + x.d.Name + ", " + x.p.Name,
                    LocationVm = new LocationVm()
                    {
                        Id = x.sd.Id,
                        Name = x.sd.Name,
                        Code = x.sd.Code,
                        IsDeleted = x.sd.IsDeleted,
                        SortOrder = x.sd.SortOrder,
                        ParentId = x.sd.ParentId,
                        Type = x.sd.Type,
                        District =  new DistrictVm()
                        {
                            Id = x.d.Id,
                            Name = x.d.Name,
                            Code = x.d.Code,
                            IsDeleted = x.d.IsDeleted,
                            SortOrder = x.d.SortOrder,
                            ParentId = x.d.ParentId,
                            Type = x.d.Type,
                            Province = new ProvinceVm()
                            {
                                Id = x.p.Id,
                                Name = x.p.Name,
                                 Code = x.p.Code,
                                IsDeleted = x.p.IsDeleted,
                                SortOrder = x.p.SortOrder,
                                ParentId = x.p.ParentId,
                                Type = x.p.Type,
                            }
                        }
                    },
                    Images = x.c.ImageClinics.Select(i=> new ImageClinicVm()
                    {
                        Id = i.Id,
                        Img = CLINICS_CONTENT_FOLDER_NAME + "/" + i.Img,
                        SortOrder = i.SortOrder
                    }).ToList()
                }).ToListAsync();

            var pagedResult = new PagedResult<ClinicVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return  new ApiSuccessResult<PagedResult<ClinicVm>>(pagedResult); 
        }

        public async Task<ApiResult<ClinicVm>> GetById(Guid Id)
        {
            var clinics = await _context.Clinics.FindAsync(Id);
            if (clinics == null) return new ApiErrorResult<ClinicVm>("Phòng khám không được xác nhận!");
            var subdistrict = await _context.Locations.FindAsync(clinics.LocationId);
            var district = await _context.Locations.FindAsync(subdistrict.ParentId);

            var img = from i in _context.ImageClinics select i;
            var user = from dt in _context.Doctors
                       where dt.ClinicId == Id
                       select dt;

            var ratings = from r in _context.Rates select r;
            var appointments = _context.Appointments;
            var rating = 0;
            var icount = 0;
            foreach (var item in user.ToList())
            {
                var ratingdoctor = ratings.Where(x => x.Appointments.DoctorId == item.UserId);

                if (ratingdoctor.Count() != 0)
                {
                    rating = rating + (await ratingdoctor.SumAsync(x => x.Rating));
                    icount++;
                }
            }
            rating = rating / (icount==0?1:icount);

            var rs = new ClinicVm()
            {
                Id = clinics.Id,
                Name = clinics.Name,
                Description = clinics.Description,
                Address = clinics.Address,
                Status = clinics.Status,
                No = clinics.No,
                ImgLogo = CLINIC_CONTENT_FOLDER_NAME + "/" + clinics.ImgLogo,
                Note = clinics.Note,
                CreatedAt = clinics.CreatedAt,
                FullAddress = clinics.FullAddress ,
                MapUrl = clinics.MapUrl,
                Service = clinics.Service,
                Contact = clinics.Contact,
                Rating = rating,
                RatingText = icount.ToString(),
                LocationVm= new LocationVm()
                {
                    Id = subdistrict.Id,
                    Name = subdistrict.Name,
                    District = new DistrictVm()
                    {
                        Id = district.Id,
                        Name = district.Name
                    }
                },
                Images = img.Where(i => i.ClinicId == clinics.Id).Select(i => new ImageClinicVm()
                {
                    Id = i.Id,
                    Img = CLINICS_CONTENT_FOLDER_NAME + "/" + i.Img,
                    SortOrder = i.SortOrder
                }).ToList(),
                DoctorVms = user.Select(u => new DoctorVm()
				{
                    Img = USER_CONTENT_FOLDER_NAME + "/" + u.Img,
                    No = u.No,
                    UserId = u.UserId,
                    Booking = u.Booking,
                    Slug = u.Slug,
                    Address = u.Address,
                    Dob = u.Dob,
                    Educations = u.Educations,
                    FirstName = u.FirstName,
                    Gender = u.Gender,
                    LastName = u.LastName,
                    MapUrl = u.MapUrl,
                    Prefix = u.Prefix,
                    Note = u.Note,
                    IsPrimary = u.IsPrimary,
                    Services = u.Services.Select(s => new ServiceVm()
                    {
                        Id = s.Id,
                        Description = s.Description,
                        ServiceName = s.ServiceName,
                        Price = s.Price,
                    }).ToList(),
                    GetSpecialities = u.ServicesSpecialities.Select(x=>new GetSpecialityVm()
                    {
                        Id= x.Specialities.Id,
                        Title = x.Specialities.Title
                    }).ToList(),
                    User = new UserVm()
					{
                        Email = u.AppUsers.Email,
                        PhoneNumber = u.AppUsers.PhoneNumber,
					},
                    Rates = ratings.Where(x=>x.Appointments.DoctorId==u.UserId).Select(r => new RateVm()
                    {
                        Id = r.Id,
                        Description = r.Description,
                        Rating = r.Rating,
                        Title = r.Title,
                    }).ToList()
                }).ToList()
            };

            return new ApiSuccessResult<ClinicVm>(rs);
        }
     

      
        public async Task<ApiResult<bool>> Update(ClinicUpdateRequest request)
        {
            var i = _context.ImageClinics.Where(x=> x.ClinicId == request.Id).Count();
            var clinics = await _context.Clinics.FindAsync(request.Id);
            if (clinics == null) return new ApiErrorResult<bool>("Phòng khám không được xác nhận!");
            if(clinics.Address == request.Address)
            {
                var location = await _context.Locations.FindAsync(request.LocationId);
                var district = await _context.Locations.FindAsync(location.ParentId);
                var province = await _context.Locations.FindAsync(district.ParentId);
                var fullAddress = request.Address + ", " + location.Name + ", " + district.Name + ", " + province.Name;
                clinics.FullAddress = fullAddress;
            }
            clinics.Note = request.Note;
            clinics.Address = request.Address;
            clinics.Name = request.Name;
            clinics.Description = WebUtility.HtmlDecode(request.Description);
            clinics.LocationId = request.LocationId;
            clinics.Status = request.Status;
            clinics.Note = request.Note;
            clinics.MapUrl = request.MapUrl;
            clinics.Service = request.Service;
            clinics.Contact = request.Contact;
            if (request.ImgLogo != null)
            {
                if(clinics.ImgLogo != null) await _storageService.DeleteFileAsyncs(clinics.ImgLogo, CLINIC_CONTENT_FOLDER_NAME);
                clinics.ImgLogo = await SaveFile(request.ImgLogo, CLINIC_CONTENT_FOLDER_NAME);
            }
                
           
            if (request.ImgClinics != null)
            {
                clinics.ImageClinics = new List<ImageClinics>();

                foreach (var file in request.ImgClinics)
                {
                    var image = new ImageClinics()
                    {
                        Img = await SaveFile(file, CLINICS_CONTENT_FOLDER_NAME),
                        SortOrder = i + 1
                    };
                    clinics.ImageClinics.Add(image);
                }
            }
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật không thành công!");
        }
    }
}
