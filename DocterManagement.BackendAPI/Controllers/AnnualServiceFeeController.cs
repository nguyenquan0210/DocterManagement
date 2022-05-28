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
        /// <summary>
        /// Lấy nộp phí dịch vụ theo id
        /// </summary>
        /// 
        [HttpGet("{Id}")]
        public async Task<ActionResult<ApiResult<AnnualServiceFeeVm>>> GetById(Guid Id)
        {
            var result = await _annualServiceFeeService.GetById(Id);
            if (result == null)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPut("canceled-service-fee")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> CanceledServiceFee([FromBody] AnnualServiceFeeCancelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _annualServiceFeeService.CanceledServiceFee(request);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
        [HttpGet("approved-service-fee/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> ApprovedServiceFee(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _annualServiceFeeService.ApprovedServiceFee(Id);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
        [HttpPut("payment-service-fee")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> PaymentServiceFee([FromBody] AnnualServiceFeePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _annualServiceFeeService.PaymentServiceFee(request);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
        [HttpPut("payment-service-fee-doctor")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> PaymentServiceFeeDoctor([FromBody] AnnualServiceFeePaymentDoctorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _annualServiceFeeService.PaymentServiceFeeDoctor(request);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
    }
}
