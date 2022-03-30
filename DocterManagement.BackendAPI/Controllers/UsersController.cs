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
    }
}
