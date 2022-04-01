using DoctorManagement.Application.Catalog.Speciality;
using DoctorManagement.ViewModels.Catalog.Speciality;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialityController : ControllerBase
    {
        private readonly ISpecialityService _specialityService;
        public SpecialityController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] SpecialityCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newsId = await _specialityService.Create(request);
            if (newsId.ToString() == null)
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
            var affectedResult = await _specialityService.Delete(Id);

            return Ok(affectedResult);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] SpecialityUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _specialityService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }


        //http://localhost/api/categories/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetSpecialityPagingRequest request)
        {
            var user = await _specialityService.GetAllPaging(request);
            return Ok(user);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(int Id)
        {
            var speciality = await _specialityService.GetById(Id);
            if (speciality == null)
                return BadRequest("Cannot find product");
            return Ok(speciality);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var speciality = await _specialityService.GetAll();
            return Ok(speciality);
        }
    }
}
