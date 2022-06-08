using DoctorManagement.Application.Common;
using DoctorManagement.Data.EF;
using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.MasterData
{
    public class MasterDataService : IMasterDataService
    {
        private readonly DoctorManageDbContext _context;
        private readonly IStorageService _storageService;
        private const string MASTERDATA_CONTENT_FOLDER_NAME = "masterData-content";
        public MasterDataService(DoctorManageDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }
        public async Task<ApiResult<InformationVm>> GetById()
        {
            var MasterDatas =  _context.Informations.FirstOrDefault();
          
           
            if (MasterDatas == null) return new ApiErrorResult<InformationVm>("Thông tin không được xác nhân!");
            var rs = new InformationVm()
            {
                Id = MasterDatas.Id,
                IsDeleted = MasterDatas.IsDeleted,
                Company = MasterDatas.Company,
                Email = MasterDatas.Email,
                FullAddress = MasterDatas.FullAddress,
                Hotline = MasterDatas.Hotline,
                Image = MASTERDATA_CONTENT_FOLDER_NAME +"/"+ MasterDatas.Image,
                TimeWorking = MasterDatas.TimeWorking,
            };

            return new ApiSuccessResult<InformationVm>(rs);
        }



        public async Task<ApiResult<bool>> Update(InformationUpdateRequest request)
        {
            
            var informations = await _context.Informations.FindAsync(request.Id);
            if (informations == null) return new ApiErrorResult<bool>("Thông tin không được xác nhân!");
            informations.Hotline = request.Hotline;
            informations.TimeWorking = request.TimeWorking;
            informations.FullAddress = request.FullAddress;
            informations.IsDeleted = request.IsDeleted;
            informations.Company = request.Company;
            informations.Email = request.Email;
            if (request.Image != null)
            {
                if (informations.Image != null && informations.Image != "default") await _storageService.DeleteFileAsyncs(informations.Image, MASTERDATA_CONTENT_FOLDER_NAME);
                informations.Image = await SaveFile(request.Image, MASTERDATA_CONTENT_FOLDER_NAME);
            }
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiSuccessResult<bool>(false);
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
        public async Task<ApiResult<bool>> CreateMainMenu(MainMenuCreateRequest request)
        {
            var mainMenu = new MainMenus()
            {
                Name = request.Name,
                SortOrder = request.SortOrder,
                Action = request.Action,
                Controller = request.Controller,
                CratedAt = DateTime.Now,
                IsDeleted = false,
                Type = request.Type == "0" ? "MenuHeader" : request.Type == "2" ? "MenuPanner" : request.Type == "1" ? "MenuHeaderDrop" : request.Type == "3" ? "Topic" : request.Type == "4" ? "Category" : "Categoryfeature",
                ParentId = request.ParentId.Value==null ? Guid.NewGuid() : request.ParentId.Value,
                Image = "default",
                Title = request.Title,
                Description = request.Description,
            };
            if(request.Image!= null)
            {
                mainMenu.Image = await SaveFile(request.Image, MASTERDATA_CONTENT_FOLDER_NAME);
            }
            _context.MainMenus.Add(mainMenu);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>(true);
            return new ApiErrorResult<bool>("Tạo menu không thành công!");
        }
       
        public async Task<ApiResult<int>> DeleteMainMenu(Guid Id)
        {
            var MainMenu = await _context.MainMenus.FindAsync(Id);
            int check = 0;
            if (MainMenu == null) return new ApiSuccessResult<int>(check);
            if (MainMenu.IsDeleted == false)
            {
                MainMenu.IsDeleted = true;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<List<MainMenuVm>>> GetAllMainMenu()
        {
            var query = _context.MainMenus.Where(x => x.IsDeleted == false);

            var rs = await query.Select(x => new MainMenuVm()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder,
                IsDeleted = x.IsDeleted,
                Action = x.Action,
                Controller = x.Controller,
                Image = MASTERDATA_CONTENT_FOLDER_NAME + "/" + x.Image,
                CratedAt = x.CratedAt,
                ParentId = x.ParentId,
                Type = x.Type,
                Title = x.Title,
                Description = x.Description,
            }).ToListAsync();
            return new ApiSuccessResult<List<MainMenuVm>>(rs);
        }

        public async Task<ApiResult<PagedResult<MainMenuVm>>> GetAllPagingMainMenu(GetMainMenuPagingRequest request)
        {
            var query = from m in _context.MainMenus
                        select m;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(x => x.Type == request.Type);
            }
            int totalRow = await query.CountAsync();
           
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MainMenuVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    IsDeleted = x.IsDeleted,
                    Action = x.Action,
                    Controller = x.Controller,
                    Image = MASTERDATA_CONTENT_FOLDER_NAME + "/" + x.Image,
                    CratedAt = x.CratedAt,
                    ParentId = x.ParentId,
                    Type = x.Type,
                    Title = x.Title,
                    Description = x.Description,
                }).ToListAsync();

            var pagedResult = new PagedResult<MainMenuVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<MainMenuVm>>(pagedResult);
        }

        public async Task<ApiResult<MainMenuVm>> GetByIdMainMenu(Guid Id)
        {
            var MainMenu = await _context.MainMenus.FindAsync(Id);
            if (MainMenu == null) return new ApiErrorResult<MainMenuVm>("Menu không được xác nhân!");
            var menuparent = new MainMenus();
            if(MainMenu.ParentId != Guid.Empty) menuparent = await _context.MainMenus.FindAsync(MainMenu.ParentId);
            var rs = new MainMenuVm()
            {
                Id = MainMenu.Id,
                Name = MainMenu.Name,
                SortOrder = MainMenu.SortOrder,
                IsDeleted = MainMenu.IsDeleted,
                Action = MainMenu.Action,
                Controller = MainMenu.Controller,
                CratedAt = MainMenu.CratedAt,
                Image = MASTERDATA_CONTENT_FOLDER_NAME + "/" + MainMenu.Image,
                ParentId = MainMenu.ParentId,
                Type = MainMenu.Type,
                Description = MainMenu.Description,
                Title = MainMenu.Title,
                ParentName = menuparent.Name,
            };
            return new ApiSuccessResult<MainMenuVm>(rs);
        }

        public async Task<ApiResult<bool>> UpdateMainMenu(MainMenuUpdateRequest request)
        {
            var MainMenu = await _context.MainMenus.FindAsync(request.Id);
            if (MainMenu == null) return new ApiErrorResult<bool>("Menu không được xác nhân!");
            MainMenu.Name = request.Name;
            MainMenu.Title = request.Title;
            MainMenu.Description = request.Description;
            MainMenu.SortOrder = request.SortOrder;
            MainMenu.Controller = request.Controller;
            MainMenu.IsDeleted = request.IsDeleted;
            MainMenu.Action = request.Action;
            MainMenu.Type = request.Type == "0" ? "MenuHeader" : request.Type == "2" ? "MenuPanner" : "MenuHeaderDrop";
            if (request.Image != null)
            {
                if (MainMenu.Image != null&& MainMenu.Image!="default") await _storageService.DeleteFileAsyncs(MainMenu.Image, MASTERDATA_CONTENT_FOLDER_NAME);
                MainMenu.Image = await SaveFile(request.Image, MASTERDATA_CONTENT_FOLDER_NAME);
            }

            var rs = await _context.SaveChangesAsync();
            if(rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Cập nhật không thành công!");
        }

        public async Task<ApiResult<bool>> CreateEthnic(EthnicCreateRequest request)
        {
            var ethnic = new Ethnics()
            {
                IsDeleted = false,
                Name = request.Name,
                SortOrder = request.SortOrder,
                Description = request.Description,

            };
            await _context.Ethnics.AddAsync(ethnic);
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Tạo dân tộc không thành công!!");
        }

        public async Task<ApiResult<bool>> UpdateEthnic(EthnicUpdateRequest request)
        {
            var ethnic = await _context.Ethnics.FindAsync(request.Id);
            if (ethnic == null) new ApiErrorResult<bool>("Dân tộc không tồn tại!");
            ethnic.Name = request.Name;
            ethnic.SortOrder = request.SortOrder;
            ethnic.Description = request.Description;
            ethnic.IsDeleted = request.IsDeleted;
            var rs = await _context.SaveChangesAsync();
            if (rs != 0) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Cập nhật dân tộc không thành công!!");
        }

        public async Task<ApiResult<int>> DeleteEthnic(Guid Id)
        {
            var ethnics = await _context.Ethnics.FindAsync(Id);
            int check = 0;
            if (ethnics == null) return new ApiSuccessResult<int>(check);
            if (ethnics.IsDeleted == false)
            {
                ethnics.IsDeleted = true;
                check = 2;
            }
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(check);
        }

        public async Task<ApiResult<PagedResult<EthnicsVm>>> GetAllPagingEthnic(GetEthnicPagingRequest request)
        {
            var query = from e in _context.Ethnics select e;
            //2. filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EthnicsVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    SortOrder = x.SortOrder,
                    IsDeleted = x.IsDeleted,
                  
                }).ToListAsync();

            var pagedResult = new PagedResult<EthnicsVm>()
            {
                TotalRecords = totalRow,
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<EthnicsVm>>(pagedResult);
        }

        public async Task<ApiResult<List<EthnicsVm>>> GetAllEthnic()
        {
            var query = _context.Ethnics.Where(x => x.IsDeleted == false);

            var rs = await query.Select(x => new EthnicsVm()
            {
                Id = x.Id,
                Name = x.Name,
                SortOrder = x.SortOrder,
                IsDeleted = x.IsDeleted,
            
            }).ToListAsync();
            return new ApiSuccessResult<List<EthnicsVm>>(rs);
        }

        public async Task<ApiResult<EthnicsVm>> GetByIdEthnic(Guid Id)
        {
            var MainMenu = await _context.Ethnics.FindAsync(Id);
            if (MainMenu == null) return new ApiErrorResult<EthnicsVm>("Dân tộc không được xác nhân!");
            var rs = new EthnicsVm()
            {
                Id = MainMenu.Id,
                Name = MainMenu.Name,
                SortOrder = MainMenu.SortOrder,
                IsDeleted = MainMenu.IsDeleted,
         
            };
            return new ApiSuccessResult<EthnicsVm>(rs);
        }
    }
}
