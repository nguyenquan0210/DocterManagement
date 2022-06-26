using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Contact;
using DoctorManagement.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Contact
{
    public class ContactService : IContactService
    {
        private readonly DoctorManageDbContext _context;
        public ContactService(DoctorManageDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> CreateContact(ContactCreateRequest request)
        {
            var contact = new Contacts()
            {
                IsDeleted = false,
                CratedAt = DateTime.Now,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                YourMessage = request.YourMessage,
                Name = request.Name,
            };
            _context.Contacts.Add(contact);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Tạo liện hệ không thành công!");
        }

        public async Task<ApiResult<int>> DeleteContact(Guid Id)
        {
            var contacts = await _context.Contacts.FindAsync(Id);
            int check = 0;
            if (contacts == null) return new ApiSuccessResult<int>(check);
            if (contacts.IsDeleted = false)
            {
                contacts.IsDeleted = true;
                check = 1;
            }else if(contacts.IsDeleted = true)
            {
                _context.Contacts.Remove(contacts);
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<ContactVm>>> GetAllContact()
        {
            var query = _context.Contacts.Where(x => x.IsDeleted == false);

            var rs = await query.Select(x => new ContactVm()
            {
                Id = x.Id,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                CratedAt = x.CratedAt,
                IsDeleted = x.IsDeleted,
                YourMessage = x.YourMessage,
                Name = x.Name
            }).ToListAsync();
            return new ApiSuccessResult<List<ContactVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<ContactVm>>> GetAllPagingContact(GetContactPagingRequest request)
        {
            var query = from m in _context.Contacts select m;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Email.Contains(request.Keyword)|| x.PhoneNumber.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            query = query.OrderByDescending(x=>x.CratedAt);
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ContactVm()
                {
                    Id = x.Id,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    CratedAt = x.CratedAt,
                    IsDeleted = x.IsDeleted,
                    YourMessage = x.YourMessage,
                    Name = x.Name
                }).ToListAsync();

            var pagedResult = new PagedResult<ContactVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<ContactVm>>(pagedResult);
        }

        public async Task<ApiResult<ContactVm>> GetByIdContact(Guid Id)
        {
            var contacts = await _context.Contacts.FindAsync(Id);
            if (contacts.IsDeleted == false) contacts.IsDeleted = true;
            await _context.SaveChangesAsync();
            if (contacts == null) return new ApiErrorResult<ContactVm>("Liên hệ không được xác nhận!");
            var rs = new ContactVm()
            {
                Id = contacts.Id,
                CratedAt = contacts.CratedAt,
                Email = contacts.Email,
                IsDeleted = contacts.IsDeleted,
                YourMessage = contacts.YourMessage,
                PhoneNumber = contacts.PhoneNumber,
               Name = contacts.Name
            };
            return new ApiSuccessResult<ContactVm>(rs);
        }

        public async Task<ApiResult<bool>> UpdateContact(ContactUpdateRequest request)
        {
            var speciality = await _context.Contacts.FindAsync(request.Id);
            if (speciality == null) return new ApiErrorResult<bool>("Liên hệ không được xác nhận!");
         
            speciality.IsDeleted = request.IsDeleted;
          
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Cập nhật không thành công!");
        }
    }
}
