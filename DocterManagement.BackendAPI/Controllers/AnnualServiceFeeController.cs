using DoctorManagement.Application.System.AnnualServiceFee;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnualServiceFeeController : ControllerBase
    {
        private readonly IAnnualServiceFeeService _annualServiceFeeService;
        public AnnualServiceFeeController(IAnnualServiceFeeService annualServiceFeeService)
        {
            _annualServiceFeeService = annualServiceFeeService;
        }
        /// <summary>
        /// Lấy danh nộp phí dịch vụ hằng năm phân trang
        /// </summary>
        /// 
        [HttpGet("paging")]
        public async Task<ActionResult<ApiResult<PagedResult<AnnualServiceFeeVm>>>> GetAllPaging([FromQuery] GetAnnualServiceFeePagingRequest request)
        {
            var user = await _annualServiceFeeService.GetAllPaging(request);
            return Ok(user);
        }
        

    }
}
