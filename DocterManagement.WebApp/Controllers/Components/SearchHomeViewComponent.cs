using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers.Components
{
    public class SearchHomeViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }

    }
}
