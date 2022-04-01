using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class ManageRegisterRequestValidator : AbstractValidator<ManageRegisterRequest>
    {
        public ManageRegisterRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Họ tên là bắt buộc")
                .MaximumLength(100).WithMessage("Họ không được quá 100 ký tự");

            RuleFor(x => x.Dob).NotEmpty().WithMessage("Ngày sinh là bắt buộc là bắt buộc");

            RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail là bắt buộc")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                .WithMessage("Định dạng email không khớp");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại là bắt buộc");

            RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ là bắt buộc");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("Tên tài khoản là bắt buộc");

            RuleFor(x => x.ThumbnailImage).NotEmpty().WithMessage("Hình ảnh là bắt buộc");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")
                .WithMessage("Hơn 3 chữ cái viết hoa và viết thường, ký tự số từ 10-99, kí tự đặt biệt")
                .MinimumLength(6).WithMessage("Mật khẩu có ít nhất 6 ký tự");
        }
    }
}
