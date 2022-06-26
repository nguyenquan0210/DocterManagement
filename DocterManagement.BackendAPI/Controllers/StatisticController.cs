using DoctorManagement.Application.System.StatisticService;
using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }
        /// <summary>
        /// Tạo mới đặt khám
        /// </summary>
        /// 
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<ApiResult<bool>>> Create([FromBody] HistoryActiveCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _statisticService.AddActiveUser(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả lịch sử hoạt động người dùng
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<HistoryActiveVm>>>> GetAll([FromQuery] GetHistoryActivePagingRequest request)
        {
            var result = await _statisticService.ListActiveUser(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả chi tiết lịch sử hoạt động người dùng
        /// </summary>
        /// 
        [HttpGet("get-history-active-detailt-all")]
        public async Task<ActionResult<ApiResult<List<HistoryActiveVm>>>> GetHistoryActiveDetailtAll()
        {
            var result = await _statisticService.ListActiveUserDetailt();
            return Ok(result);
        }
    }
}
