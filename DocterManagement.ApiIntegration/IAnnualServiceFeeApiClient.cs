using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IAnnualServiceFeeApiClient
    {
        Task<ApiResult<PagedResult<AnnualServiceFeeVm>>> GetAllPaging(GetAnnualServiceFeePagingRequest request);
        Task<ApiResult<AnnualServiceFeeVm>> GetById(Guid Id);

        Task<ApiResult<bool>> CanceledServiceFee(AnnualServiceFeeCancelRequest request);
        Task<ApiResult<bool>> ApprovedServiceFee(Guid Id);

        Task<ApiResult<bool>> PaymentServiceFee(AnnualServiceFeePaymentRequest request);
        Task<ApiResult<bool>> PaymentServiceFeeDoctor(AnnualServiceFeePaymentDoctorRequest request);

        Task<List<StatisticNews>> GetServiceFeeStatiticYear(GetAnnualServiceFeePagingRequest request);
        Task<List<StatisticNews>> GetServiceFeeStatiticDay(GetAnnualServiceFeePagingRequest request);
        Task<List<StatisticNews>> GetServiceFeeStatiticMonth(GetAnnualServiceFeePagingRequest request);
    }
}
