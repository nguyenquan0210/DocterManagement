using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers.Components
{
    public class NavMainViewComponent : ViewComponent
    {
        private readonly IUserApiClient _userApiClient;
        public NavMainViewComponent(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.User = (await _userApiClient.GetByUserName(User.Identity.Name)).Data;
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}
