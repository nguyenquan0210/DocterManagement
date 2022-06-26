using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class PostController : BaseController
    {
        private readonly IPostApiClient _postApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.DoctorApp.Controllers.Post";
        public PostController(IPostApiClient postApiClient, IUserApiClient userApiClient, IConfiguration configuration,
            IMasterDataApiClient masterDataApiClient, IStatisticApiClient statisticApiClient)
        {
            _statisticApiClient = statisticApiClient;
            _postApiClient = postApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);

            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name,
                    Usertemporary = User.Identity.Name,
                    Type = "doctor",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        public async Task<IActionResult> Index(string keyword, Guid? topicId,  int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetPostPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Usename = User.Identity.Name,
                TopicId = topicId
            };
           
            var data = await _postApiClient.GetAllPaging(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Index",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            ViewBag.Keyword = keyword;
            ViewBag.Type = topicId;
            ViewBag.Types = await SeletectType(topicId);
            return View(data.Data);

        }
        public async Task<IActionResult> CreatePost()
        {
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Create",
                MethodName = "Get",
                ExtraProperties = "success",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
            if (!result.IsSuccessed) return RedirectToAction("Index");
            ViewBag.DoctorId = result.Data.Id;
            ViewBag.Menus = await SeletectTypeMenu(new Guid());
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateRequest request)
        {
            ViewBag.Menus = await SeletectTypeMenu(request.TopicId);
            if (!ModelState.IsValid) return View(request);
            var rs = await _postApiClient.Create(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".CreatePost",
                MethodName = "Post",
                ExtraProperties = rs.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index");
            }
            return View(request);
        }
        public async Task<List<SelectListItem>> SeletectType(Guid? id)
        {
            var rs = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "Topic" || x.Type == "Category" || x.Type == "Categoryfeature");
            var select = rs.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = id == x.Id
            }).ToList();
            select.Add(new SelectListItem() { Text = "Tất cả",
                Value = "",
                Selected = id.ToString() == ""
            });
            return select;

        }
        public async Task<List<SelectListItem>> SeletectTypeMenu(Guid? id)
        {
            var rs = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "Topic"|| x.Type == "Category" || x.Type == "Categoryfeature");
            var select = rs.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = id == x.Id
            }).ToList();
            
            return select;
           
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile file)
        {
            var image = new ImageCreateRequest()
            {
                File = file
            };
            var rs = await _postApiClient.AddImage(image);
            var fileurl = "";
            if(rs.IsSuccessed)
            {
                fileurl = _configuration["BaseAddress"] + "/img/" + rs.Data;
            }
            return Json(new {url = fileurl });
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _postApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Update",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{id: " + id + "}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                var Post = result.Data;

                ViewBag.Image = Post.Image;
                ViewBag.Menus = await SeletectTypeMenu(Post.Topic.Id);

                var updateRequest = new PostUpdateRequest()
                {
                    Title = Post.Title,
                    Id = id,
                    DoctorId = Post.Doctors.UserId,
                    Description = Post.Description,
                    Content = Post.Content,
                    Status = Post.Status == Status.Active?true:false,
                    TopicId = Post.Topic.Id,
                    ImageText = Post.Image,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] PostUpdateRequest request)
        {
            ViewBag.Menus = await SeletectTypeMenu(request.TopicId);
            if (!ModelState.IsValid)
                return View();

            var result = await _postApiClient.Update(request);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Create",
                MethodName = "Post",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Title + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }
            return View(request);
        }

        public async Task<IActionResult> DetailtPost(Guid id)
        {
            var result = await _postApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DetailtMedicine",
                MethodName = "Get",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{id: "+ id+"}",
            };
            await HistoryActive(historyactive);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _postApiClient.DeleteDoctor(Id);
            return Json(new { response = result });
        }
    }
}
