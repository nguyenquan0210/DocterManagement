using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class SearchHomeViewComponent : ViewComponent
    {
        private readonly IMasterDataApiClient _masterDataApiClient;
        public SearchHomeViewComponent(IMasterDataApiClient masterDataApiClient)
        {
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mainMenus = (await _masterDataApiClient.GetAllMainMenu()).Data;
            ViewBag.MenuPanner = mainMenus.Where(x => x.Type == "MenuPanner").ToList();
           
            return View();
        }

    }
}
