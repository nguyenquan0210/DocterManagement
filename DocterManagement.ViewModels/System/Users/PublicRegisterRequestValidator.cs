using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class PublicRegisterRequestValidator : AbstractValidator<PublicRegisterRequest>
    {
        public PublicRegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Họ là bắt buộc")
                  .MaximumLength(100).WithMessage("Họ không được quá 100 ký tự");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("Tên là bắt buộc")
                .MaximumLength(100).WithMessage("Tên không được quá 100 ký tự");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Tên tài khoản là bắt buộc");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu là bắt buộc")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")
                .WithMessage("Hơn 3 chữ cái viết hoa và viết thường, ký tự số từ 10-99, kí tự đặt biệt")
                .MinimumLength(6).WithMessage("Mật khẩu có ít nhất 6 ký tự");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure("Xác nhận mật khẩu không khớp");
                }
            });
        }
    }
}
