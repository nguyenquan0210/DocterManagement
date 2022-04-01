using DoctorManagement.Application.Catalog.Distric;
using DoctorManagement.ViewModels.Catalog.Distric;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistricController : ControllerBase
    {
        private readonly IDistricService _districService;
        public DistricController(IDistricService districService)
        {
            _districService = districService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] DistricCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _districService.Create(request);
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
            var result = await _districService.Delete(Id);

            return Ok(result);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] DistricUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _districService.Update(request);
            if (result == 0)
                return BadRequest();
            return Ok();
        }


        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetDistricPagingRequest request)
        {
            var user = await _districService.GetAllPaging(request);
            return Ok(user);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _districService.GetById(Id);
            if (result == null)
                return BadRequest("Cannot find product");
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _districService.GetAll();
            return Ok(result);
        }
    }
}