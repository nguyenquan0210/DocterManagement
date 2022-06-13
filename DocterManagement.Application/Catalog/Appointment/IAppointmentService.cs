using DoctorManagement.Data.Entities;
using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Doctors;
using DoctorManagement.ViewModels.System.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Appointment
{
    public interface IAppointmentService
    {
        Task<ApiResult<Guid>> Create(AppointmentCreateRequest request);

        Task<ApiResult<bool>> Update(AppointmentUpdateRequest request);

        Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<bool>> CanceledAppointment(AppointmentCancelRequest request);

        Task<ApiResult<PagedResult<AppointmentVm>>> GetAllPaging(GetAppointmentPagingRequest request);
        Task<ApiResult<PagedResult<PatientVm>>> GetAllPatient(GetAppointmentPagingRequest request);
        Task<ApiResult<PagedResult<AppointmentVm>>> GetAllPagingRating(GetAppointmentPagingRequest request);
        Task<ApiResult<List<AppointmentVm>>> GetAll();

        Task<ApiResult<bool>> AddExpired(GetAppointmentPagingRequest request);
        //Task<ApiResult<bool>> ToAppointmentTime(GetAppointmentPagingRequest request);

        Task<ApiResult<AppointmentVm>> GetById(Guid Id);
    }
}
