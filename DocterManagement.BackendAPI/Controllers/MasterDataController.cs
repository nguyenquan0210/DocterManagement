using DoctorManagement.Application.Catalog.MasterData;
using DoctorManagement.ViewModels.Catalog.MasterData;
using DoctorManagement.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _masterDataService;
        public MasterDataController(IMasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }
       
        
        /// <summary>
        /// Cập nhật phòng khám
        /// </summary>
        /// 
        [Authorize]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> Update([FromForm] InformationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _masterDataService.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

       
        /// <summary>
        /// Lấy dữ liệu phòng khám theo id
        /// </summary>
        /// 
        [HttpGet("get-by-information")]
        public async Task<ActionResult<ApiResult<InformationVm>>> GetById()
        {
            var result = await _masterDataService.GetById();
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới chuyên khoa
        [HttpPost("create-mainmenu")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> CreateMainMenu([FromForm] MainMenuCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _masterDataService.CreateMainMenu(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa chuyên khoa
        /// </summary>
        /// 
        [HttpDelete("delete-mainmenu/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> DeleteMainMenu([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _masterDataService.DeleteMainMenu(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật chuyên khoa
        /// </summary>
        /// 
        [HttpPut("update-mainmenu")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateMainMenu([FromForm] MainMenuUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _masterDataService.UpdateMainMenu(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang chuyên khoa
        /// </summary>
        /// 
        //http://localhost/api/categories/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("get-paging-mainmenu")]
        public async Task<ActionResult<ApiResult<PagedResult<MainMenuVm>>>> GetAllPagingMainMenu([FromQuery] GetMainMenuPagingRequest request)
        {
            var result = await _masterDataService.GetAllPagingMainMenu(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chuyên khoa theo id
        /// </summary>
        /// 
        [HttpGet("get-by-mainmenuid/{Id}")]
        public async Task<ActionResult<ApiResult<MainMenuVm>>> GetByIdMainMenu(Guid Id)
        {
            var result = await _masterDataService.GetByIdMainMenu(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chuyên khoa
        /// </summary>
        /// 
        [HttpGet("get-all-mainmenu")]
        public async Task<ActionResult<ApiResult<List<MainMenuVm>>>> GetAllMainMenu()
        {
            var MainMenu = await _masterDataService.GetAllMainMenu();
            return Ok(MainMenu);
        }
        /// <summary>
        /// 
        /// </summary>
        /// Tạo mới chuyên khoa
        [HttpPost("create-mainmenu")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> CreateEthnic(EthnicCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _masterDataService.CreateEthnic(request);
            if (!result.IsSuccessed)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Xóa chuyên khoa
        /// </summary>
        /// 
        [HttpDelete("delete-ethnic/{Id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<int>>> DeleteEthnic([FromRoute] Guid Id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _masterDataService.DeleteEthnic(Id);

            return Ok(affectedResult);
        }
        /// <summary>
        /// Cập nhật chuyên khoa
        /// </summary>
        /// 
        [HttpPut("update-ethnic")]
        [Authorize]
        public async Task<ActionResult<ApiResult<bool>>> UpdateEthnic( EthnicUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _masterDataService.UpdateEthnic(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy danh sách phân trang chuyên khoa
        /// </summary>
        /// 
        //http://localhost/api/categories/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("get-paging-ethnic")]
        public async Task<ActionResult<ApiResult<PagedResult<EthnicVm>>>> GetAllPagingEthnic([FromQuery] GetEthnicPagingRequest request)
        {
            var result = await _masterDataService.GetAllPagingEthnic(request);
            return Ok(result);
        }
        /// <summary>
        /// Lấy chuyên khoa theo id
        /// </summary>
        /// 
        [HttpGet("get-by-ethnicid/{Id}")]
        public async Task<ActionResult<ApiResult<EthnicVm>>> GetByIdEthnic(Guid Id)
        {
            var result = await _masterDataService.GetByIdEthnic(Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách chuyên khoa
        /// </summary>
        /// 
        [HttpGet("get-all-ethnic")]
        public async Task<ActionResult<ApiResult<List<EthnicVm>>>> GetAllEthnic()
        {
            var Ethnic = await _masterDataService.GetAllEthnic();
            return Ok(Ethnic);
        }
    }
}
