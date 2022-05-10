using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.System.Users
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Tên là bắt buộc").MaximumLength(50).WithMessage("Tên không được quá 50 ký tự");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Họ tên là bắt buộc").MaximumLength(50).WithMessage("Họ không được quá 50 ký tự");

            //RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail là bắt buộc").Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Định dạng email không khớp");

            //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Số điện thoại là bắt buộc").MaximumLength(11).WithMessage("Số điện thoại không được quá 100 ký tự"); ;

            RuleFor(x => x.Address).NotEmpty().WithMessage("Địa chỉ là bắt buộc");

            RuleFor(x => x.Dob).NotEmpty().WithMessage("Ngày sinh là bắt buộc là bắt buộc");

        }
    }
}
