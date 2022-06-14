using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers.Components
{
    public class NavMainViewComponent : ViewComponent
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        public NavMainViewComponent(IUserApiClient userApiClient, IMasterDataApiClient masterDataApiClient)
        {
            _userApiClient = userApiClient;
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.User = (await _userApiClient.GetByUserName(User.Identity.Name)).Data;
            ViewBag.Information = (await _masterDataApiClient.GetById()).Data;
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}
