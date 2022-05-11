using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Mật khẩu hiện tại là bắt buộc");

            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Mật khẩu mới là bắt buộc")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")
                .WithMessage("Hơn 3 chữ cái viết hoa và viết thường, ký tự số từ 10-99, kí tự đặt biệt")
                .MinimumLength(6).WithMessage("Mật khẩu có ít nhất 6 ký tự");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.NewPassword != request.ConfirmPassword)
                {
                    context.AddFailure("Xác nhận mật khẩu không khớp");
                }
            });
        }
    }
}
