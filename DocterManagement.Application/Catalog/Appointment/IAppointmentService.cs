using DoctorManagement.ViewModels.Catalog.Appointment;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Catalog.Appointment
{
    public interface IAppointmentService
    {
        Task<Guid> Create(AppointmentCreateRequest request);

        Task<int> Update(AppointmentUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<PagedResult<AppointmentVm>> GetAllPaging(GetAppointmentPagingRequest request);

        Task<List<AppointmentVm>> GetAll();

        Task<AppointmentVm> GetById(Guid Id);
    }
}
