using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.MasterData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    public class MasterDataController : BaseController
    {
        private readonly IMasterDataApiClient _masterDataApiClient;
        private readonly IConfiguration _configuration;

        public MasterDataController(IMasterDataApiClient masterDataApiClient,
            IConfiguration configuration)
        {
            _masterDataApiClient = masterDataApiClient;
            _configuration = configuration;
        }

       
       
        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var result = await _masterDataApiClient.GetById();
            //var doctor = await _ClinicApiClient.Get
            if (result.IsSuccessed)
            {
                var information = result.Data;
             
                ViewBag.Image = information.Image;
                var updateRequest = new InformationUpdateRequest()
                {
                    IsDeleted = information.IsDeleted,
                    Id = information.Id,
                    Company = information.Company,
                    Email = information.Email,
                    FullAddress = information.FullAddress,
                    Hotline = information.Hotline,
                    TimeWorking = information.TimeWorking,
                    ServiceFee = information.ServiceFee,
                    AccountBank = information.AccountBank,
                    AccountBankName = information.AccountBankName,
                    Content = information.Content,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }
        
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] InformationUpdateRequest request)
        {
            ViewBag.Image = request.Image;
            if (!ModelState.IsValid)
                return View(request);

            var result = await _masterDataApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin" + request.Company + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Detailt");
            }
            TempData["AlertMessage"] = "Thay đổi thông tin" + request.Company + " không thành công.";
            TempData["AlertType"] = "alert-warning";
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt()
        {
            var result = await _masterDataApiClient.GetById();
            if (result.IsSuccessed)
            {
                var informationdata = result.Data;
                ViewBag.Image = informationdata.Image;
                return View(informationdata);
            }
            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> MenuMain(string keyword, string type, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetMainMenuPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Type = type
            };
            ViewBag.Type = request.Type==null?"": request.Type;
            ViewBag.Types = SeletectType(request.Type == null ? "" : request.Type);
            var data = await _masterDataApiClient.GetAllPagingMainMenu(request);
            ViewBag.Keyword = keyword;

            return View(data.Data);
            
        }
        public List<SelectListItem> SeletectType(string? str)
        {
            List<SelectListItem> type = new List<SelectListItem>()
            {
                new SelectListItem(text: "Phân nhánh menu tiêu đề", value: "MenuHeaderDrop"),
                new SelectListItem(text: "Menu tiêu đề", value: "MenuHeader"),
                new SelectListItem(text: "Menu biểu ngữ", value: "MenuPanner"),
                new SelectListItem(text: "Chủ đề y tế", value: "Topic"),
                new SelectListItem(text: "Chủ đề y tế quan tâm", value: "Category"),
                new SelectListItem(text: "Phân nhánh chủ đề y tế", value: "Categoryfeature"),
                new SelectListItem(text: "Tất cả", value: ""),
            };
            var rs = type.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = str == x.Value
            }).ToList();
            return rs;
        }
        public  IActionResult CreateMainMenu()
        {
            ViewBag.Type = SeletectTypeMenu();
            ViewBag.ParentMenu = new List<SelectListItem>();
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateMainMenu([FromForm] MainMenuCreateRequest request)
        {
            ViewBag.Type = SeletectTypeMenu();
            if(request.ParentId != new Guid())
            {
                var menu = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "MenuHeader");
                ViewBag.ParentMenu = menu.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else ViewBag.ParentMenu = new List<SelectListItem>();
            if (!ModelState.IsValid)
                return View(request);
            request.Controller = request.Controller == null ? "Home" : request.Controller;
            request.Action = request.Action == null ? "Index" : request.Action;
            var result = await _masterDataApiClient.CreateMainMenu(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Name + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("MenuMain");
            }

            return View(request);
        }
        public async Task<IActionResult> UpdateMainMenu(Guid Id)
        {
            ViewBag.Type = SeletectTypeMenu();
            var menu = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "MenuHeader");
            ViewBag.ParentMenu = menu.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            var result = await _masterDataApiClient.GetByIdMainMenu(Id);
            if (result.IsSuccessed)
            {
                var upadetmenu = new MainMenuUpdateRequest() { 
                    Id = result.Data.Id,
                    SortOrder = result.Data.SortOrder,
                    Action = result.Data.Action,
                    Controller = result.Data.Controller,
                    ImageHiden = result.Data.Image,
                    IsDeleted = result.Data.IsDeleted,
                    Name = result.Data.Name,
                    ParentId = result.Data.ParentId,
                    Type = result.Data.Type== "MenuHeader"?"0": result.Data.Type == "MenuPanner" ? "2":"1",
                };
                return View(upadetmenu);
            }
            return View("MenuMain");
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMainMenu([FromForm] MainMenuUpdateRequest request)
        {
            ViewBag.Type = SeletectTypeMenu();
            if (request.ParentId != new Guid())
            {
                var menu = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "MenuHeader");
                ViewBag.ParentMenu = menu.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else  ViewBag.ParentMenu = new List<SelectListItem>();
            if (!ModelState.IsValid)
                return View(request);

            var result = await _masterDataApiClient.UpdateMainMenu(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Name + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("MenuMain");
            }

            return View(request);
        }
        public async Task<IActionResult> DetailtMainMenu(Guid id)
        {
            var result = await _masterDataApiClient.GetByIdMainMenu(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        public List<SelectListItem> SeletectTypeMenu()
        {
            List<SelectListItem> type = new List<SelectListItem>()
            {
                new SelectListItem(text: "Phân nhánh menu tiêu đề", value: "1"),
                new SelectListItem(text: "Menu tiêu đề", value: "0"),
                new SelectListItem(text: "Menu biểu ngữ", value: "2"),
                new SelectListItem(text: "Chủ đề y tế", value: "3"),
                new SelectListItem(text: "Chủ đề y tế quan tâm", value: "4"),
                new SelectListItem(text: "Phân nhánh chủ đề y tế", value: "5"),
            };
            var rs = type.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value
            }).ToList();
            return rs;
        }
        [HttpGet]
        public async Task<IActionResult> GetSubParentMenu(int type)
        {
            var select = new List<SelectListItem>();
            if (!string.IsNullOrWhiteSpace(type.ToString()))
            {
                var typetext = type == 1 || type == 3 || type == 4 ? "MenuHeader" : type == 5 ? "Category" : "null";
                var menu = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x=>x.Type == typetext);
                select = menu.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return Json(select);
            }
            return Json(select);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteMainMenu(Guid Id)
        {
            var result = await _masterDataApiClient.DeleteMainMenu(Id);
            return Json(new { response = result });
        }
        public async Task<IActionResult> Ethnic(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetEthnicPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            ViewBag.Keyword = keyword;
            var data = await _masterDataApiClient.GetAllPagingEthnic(request);
            if (data.IsSuccessed)
            {
                return View(data.Data);
            }
            
            return RedirectToAction("Error", "Home");
        }
        public IActionResult CreateEthnic()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateEthnic( EthnicCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _masterDataApiClient.CreateEthnic(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Name + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Ethnic");
            }

            return View(request);
        }
        public async Task<IActionResult> UpdateEthnic(Guid Id)
        {
            
            var result = await _masterDataApiClient.GetByIdEthnic(Id);
            if (result.IsSuccessed)
            {
                var upadetethnic = new EthnicUpdateRequest()
                {
                    Id = result.Data.Id,
                    SortOrder = result.Data.SortOrder,
                    Description = result.Data.Description,
                    IsDeleted = result.Data.IsDeleted,
                    Name = result.Data.Name,
                };
                return View(upadetethnic);
            }
            return View("Ethnic");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEthnic( EthnicUpdateRequest request)
        {
         
            if (!ModelState.IsValid)
                return View(request);

            var result = await _masterDataApiClient.UpdateEthnic(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Name + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Ethnic");
            }

            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteEthnic(Guid Id)
        {
            var result = await _masterDataApiClient.DeleteEthnic(Id);
            return Json(new { response = result });
        }
    }
}
