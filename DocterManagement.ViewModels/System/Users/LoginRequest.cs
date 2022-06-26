using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class LoginRequest
    {
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string? Check { get; set; } 
    }
}
