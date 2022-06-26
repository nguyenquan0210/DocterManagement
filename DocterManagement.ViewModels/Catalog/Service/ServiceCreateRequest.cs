using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Catalog.Service
{
    public class ServiceCreateRequest
    {
        public Guid Id { get; set; }
        [Display(Name = "Bác sĩ")]
        public Guid DoctorId { get; set; }
        [Display(Name = "Trạng thái")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Tên dịch vụ")]
        public string ServiceName { get; set; }
        [Display(Name = "Đơn vị")]
        public string Unit { get; set; }
        [Display(Name = "Giá tiền")]
        public decimal Price { get; set; }
    }
}
