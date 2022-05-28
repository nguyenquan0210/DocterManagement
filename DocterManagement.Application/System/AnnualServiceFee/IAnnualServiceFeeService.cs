using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.AnnualServiceFee
{
    public interface IAnnualServiceFeeService
    {
        //Task<ApiResult<bool>> Create(AnnualServiceFeeCreateRequest request);

        Task<ApiResult<bool>> CanceledServiceFee(AnnualServiceFeeCancelRequest request);
        Task<ApiResult<bool>> ApprovedServiceFee(Guid Id);

        Task<ApiResult<bool>> PaymentServiceFee(AnnualServiceFeePaymentRequest request);
        Task<ApiResult<bool>> PaymentServiceFeeDoctor(AnnualServiceFeePaymentDoctorRequest request);
        //Task<ApiResult<int>> Delete(Guid Id);

        Task<ApiResult<PagedResult<AnnualServiceFeeVm>>> GetAllPaging(GetAnnualServiceFeePagingRequest request);

        //Task<ApiResult<List<AnnualServiceFeeVm>>> GetAllAnnualServiceFee();

        Task<ApiResult<AnnualServiceFeeVm>> GetById(Guid Id);
    }
}
