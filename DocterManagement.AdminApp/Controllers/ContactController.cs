using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Contact;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IContactApiClient _contactApiClient;
        private readonly IConfiguration _configuration;

        public ContactController(IContactApiClient contactApiClient,
            IConfiguration configuration)
        {
            _contactApiClient = contactApiClient;
            _configuration = configuration;
        }
        public async Task<IActionResult> ContactPaging(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetContactPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _contactApiClient.GetAllPagingContact(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);

        }
        public async Task<IActionResult> DetailtContact(Guid id)
        {
            var result = await _contactApiClient.GetByIdContact(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
