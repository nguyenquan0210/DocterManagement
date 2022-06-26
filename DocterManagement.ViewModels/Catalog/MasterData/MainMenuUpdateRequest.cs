﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class MainMenuUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Tên menu")]
        public string Name { get; set; }
        [Display(Name = "Loại menu")]
        public string Type { get; set; }
        [Display(Name = "Stt")]
        public int SortOrder { get; set; }
        [Display(Name = "Hình ảnh")]
        public IFormFile? Image { get; set; }
        [Display(Name = "Đia chỉ hoạt động")]
        public string? Action { get; set; }
        [Display(Name = "Địa chỉ điều khiển")]
        public string? Controller { get; set; }
        [Display(Name = "Quan hệ menu")]
        public Guid? ParentId { get; set; }
        [Display(Name = "Tiêu đề")]
        public string? Title { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        public string? ImageHiden { get; set; }
    }
}
