using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }

    }
}
