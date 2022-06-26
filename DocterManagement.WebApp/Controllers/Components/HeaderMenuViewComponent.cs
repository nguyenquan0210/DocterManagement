using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class HeaderMenuViewComponent : ViewComponent
    {
        private readonly IMasterDataApiClient _masterDataApiClient;
        public HeaderMenuViewComponent(IMasterDataApiClient masterDataApiClient)
        {
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mainMenus = (await _masterDataApiClient.GetAllMainMenu()).Data;
            ViewBag.Menus = mainMenus.Where(x => x.Type == "MenuHeader").ToList();
            ViewBag.Menusdrops = mainMenus.Where(x=>x.Type== "MenuHeaderDrop").ToList();
            var information = (await _masterDataApiClient.GetById()).Data;
            ViewBag.Information = information;
            return View();
        }

    }
}
