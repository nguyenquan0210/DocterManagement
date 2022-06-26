using DoctorManagement.ApiIntegration;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class InformationViewComponent : ViewComponent
    {
        private readonly IMasterDataApiClient _masterDataApiClient;
        public InformationViewComponent(IMasterDataApiClient masterDataApiClient)
        {
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var information = (await _masterDataApiClient.GetById()).Data;
            ViewBag.Information = information;
            return View();
        }

    }
}

