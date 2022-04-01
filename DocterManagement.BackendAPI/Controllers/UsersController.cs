using DoctorManagement.Application.System.Users;
using DoctorManagement.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        //private readonly IActiveUserService _activeUserService;

        public UsersController(IUserService userService )//IActiveUserService activeUserService
                                                         
        {
            _userService = userService;
            //_activeUserService = activeUserService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(result.ResultObj))
            {
                return BadRequest(result);
            }
            /*if (request.Check == false)
            {
                var listActive = _activeUserService.ListActiveUser().Result.Where(x => x.DateActive.ToShortDateString() == DateTime.Now.ToShortDateString());

                var user = await _userService.GetByUserName(request.UserName);
                var activeUser = listActive.FirstOrDefault(x => x.UserId == user.ResultObj.Id);
                if (activeUser != null)
                    await _activeUserService.UpdateActiveUser(activeUser.Id);
                else
                    await _activeUserService.AddActiveUser(user.ResultObj.Id);
            }*/

            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        [HttpPut("updateuser/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("changepass")]
        public async Task<IActionResult> ChangePass([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePassword(request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //http://localhost/api/users/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetUserPagingRequest request)
        {
            if (request.RoleName != null)
            {
                if (request.RoleName.ToUpper() != "ALL")
                {
                    var userinrole = _userService.GetUsersPaging(request);
                    return Ok(userinrole);
                }
            }
            var user = await _userService.GetUsersAllPaging(request);
            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpGet("getrequest{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUserName(string username)
        {
            var user = await _userService.GetByUserName(username);
            return Ok(user);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _userService.Delete(Id);
            return Ok(result);
        }
        /*[HttpGet("activeusers")]
        public async Task<IActionResult> GetActiveUser()
        {
            var activeUsers = await _activeUserService.ListActiveUser();
            return Ok(activeUsers);
        }*/
        [HttpGet("newuser")]
        public IActionResult GetNewUser()
        {
            var activeUsers = _userService.GetNewUser();
            return Ok(activeUsers);
        }
    }
}
