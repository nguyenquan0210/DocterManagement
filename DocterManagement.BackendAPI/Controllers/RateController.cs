using DoctorManagement.Application.Catalog.Rate;
using DoctorManagement.ViewModels.Catalog.Rate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;
        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RateCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Create(request);
            if (result.ToString() == null)
                return BadRequest();

            return Ok();
        }
        [HttpDelete("{Id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Delete(Id);

            return Ok(result);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RateUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _rateService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }


        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetRatePagingRequest request)
        {
            var user = await _rateService.GetAllPaging(request);
            return Ok(user);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _rateService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rateService.GetAll();
            return Ok(result);
        }
    }
}