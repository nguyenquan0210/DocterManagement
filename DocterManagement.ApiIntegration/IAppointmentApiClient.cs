using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Catalog.MedicalRecords;
using DoctorManagement.ViewModels.Catalog.Rate;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IAppointmentApiClient
    {
        Task<ApiResult<PagedResult<PatientVm>>> GetAppointmentPagingPatient(GetAppointmentPagingRequest request);
        Task<ApiResult<PagedResult<AppointmentVm>>> GetAppointmentPagings(GetAppointmentPagingRequest request);
        Task<ApiResult<PagedResult<AppointmentVm>>> GetAppointmentPagingRating(GetAppointmentPagingRequest request);
        Task<ApiResult<bool>> Update(AppointmentUpdateRequest request);
        Task<ApiResult<bool>> CanceledAppointment(AppointmentCancelRequest request);
        Task<ApiResult<string>> Create(AppointmentCreateRequest request);
        Task<ApiResult<bool>> AddRate(RateCreateRequest request);
        Task<ApiResult<bool>> CreateMedicalRecord(MedicalRecordCreateRequest request);
        Task<int> Delete(Guid Id);
        Task<ApiResult<List<AppointmentVm>>> GetAllAppointment(string? UserNameDoctor);
        Task<ApiResult<AppointmentVm>> GetById(Guid Id);

        Task<List<StatisticNews>> GetAppointmentStatiticYear(GetAppointmentPagingRequest request);
        Task<List<StatisticNews>> GetAppointmentStatiticDay(GetAppointmentPagingRequest request);
        Task<List<StatisticNews>> GetAppointmentStatiticMonth(GetAppointmentPagingRequest request);
    }
}
