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
        /// Lấy tất cả danh sách đặt khám
        /// </summary>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<ApiResult<List<HistoryActiveVm>>>> GetAll()
        {
            var result = await _statisticService.ListActiveUser();
            return Ok(result);
        }
    }
}
