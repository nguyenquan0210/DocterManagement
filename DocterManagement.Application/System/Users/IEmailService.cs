using DoctorManagement.ViewModels.System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.Users
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptions userEmailOptions);

        Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions);

        Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions);

        Task SendEmailChangePassword(UserEmailOptions userEmailOptions);
        Task SendEmailAppoitment(UserEmailOptions userEmailOptions);
        Task SendEmailCancelAppoitment(UserEmailOptions userEmailOptions);
    }
}
