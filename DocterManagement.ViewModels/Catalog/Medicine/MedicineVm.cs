using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Medicine
{
    public class MedicineVm
    {
        public Guid Id { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Image { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Tên thuốc")]
        public string Name { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid ParentId { get; set; }
        [Display(Name = "Giá tiền")]
        public decimal Price { get; set; }
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; }
    }
}
