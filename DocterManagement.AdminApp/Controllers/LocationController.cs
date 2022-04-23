using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Location;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    public class LocationController : BaseController
    {
        private readonly ILocationApiClient _LocationApiClient;

        public LocationController(ILocationApiClient LocationApiClient)
        {
            _LocationApiClient = LocationApiClient;
        }

        public async Task<IActionResult> Index(string keyword, string Type, int pageIndex = 1, int pageSize = 10)
        {
            if (ViewBag.Type != null)
            {
                Type = ViewBag.Type;
            }
            if (Type == null)
            {
                Type = "all";
            }
            ViewBag.Types = SeletectType(Type);
            var request = new GetLocationPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Type = Type
            };
            var data = await _LocationApiClient.GetLocationPagings(request);
            ViewBag.Keyword = keyword;
            if (Type != null)
                ViewBag.Type = Type;
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        /* public async Task<IActionResult> Index(string Type)
         {
             if (ViewBag.Type != null)
             {
                 Type = ViewBag.Type;
             }
             if (Type == null)
             {
                 Type = "all";
             }
             ViewBag.Types = SeletectType(Type);

             var data = await _LocationApiClient.GetLocationAllPagings(Type);

             if (Type != null)
                 ViewBag.Type = Type;
             if (TempData["result"] != null)
             {
                 ViewBag.SuccessMsg = TempData["result"];
             }
             return View(data.Data);
         }*/
        public List<SelectListItem> SeletectType(string str)
        {
            List<SelectListItem> type = new List<SelectListItem>()
            {
                new SelectListItem(text: "Phường/Xã", value: "subdistrict"),
                new SelectListItem(text: "Quận/Huyện", value: "district"),
                new SelectListItem(text: "Tỉnh/Thành Phố", value: "province"),
                new SelectListItem(text: "Miền chính", value: "Region"),
                new SelectListItem(text: "Miền Phụ", value: "SubRegion"),
                new SelectListItem(text: "Tất cả", value: "all")
            };

            var rs = type.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = str == x.Value
            }).ToList();
            return rs;
        }
    }
}
