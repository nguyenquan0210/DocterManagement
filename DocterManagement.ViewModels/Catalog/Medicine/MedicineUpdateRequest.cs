using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Medicine
{
    public class MedicineUpdateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Hình ảnh")]
        public IFormFile? Image { get; set; }
        public string? ImageText { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Tên thuốc")]
        public string Name { get; set; }
        [Display(Name = "Giá tiền")]
        public decimal Price { get; set; }
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; }
    }
}
