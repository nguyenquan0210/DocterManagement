using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostApiClient _postApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        public PostController(IPostApiClient postApiClient, IUserApiClient userApiClient, IConfiguration configuration,
            IMasterDataApiClient masterDataApiClient)
        {
            _postApiClient = postApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IActionResult> DetailtPost(Guid id)
        {
            var result = await _postApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
