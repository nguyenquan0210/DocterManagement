using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers.Components
{
    public class NavMainViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}
