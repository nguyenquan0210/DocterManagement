using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.MasterData
{
    public class InformationUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Tên công ty")]
        public string Company { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Hình ảnh")]
        public IFormFile? Image { get; set; }
        [Display(Name = "Đường dây nóng")]
        public string Hotline { get; set; }
        [Display(Name = "Giờ làm việc")]
        public string TimeWorking { get; set; }
        [Display(Name = "Địa chi đầy đủ")]
        public string FullAddress { get; set; }
        [Display(Name = "Mức phí dịch vụ")]
        public decimal ServiceFee { get; set; }
        [Display(Name = "Số tài khoản")]
        public string AccountBank { get; set; }
        [Display(Name = "Tên Ngân hàng")]
        public string AccountBankName { get; set; }
        [Display(Name = "Ghi chú")]
        public string Content { get; set; }
    }
}
