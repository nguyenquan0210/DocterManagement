using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    public class PostController : BaseController
    {
        private readonly IPostApiClient _postApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        public PostController(IPostApiClient postApiClient, IUserApiClient userApiClient, IConfiguration configuration,
            IMasterDataApiClient masterDataApiClient)
        {
            _postApiClient = postApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _masterDataApiClient = masterDataApiClient;
        }
        public async Task<IActionResult> Index(string keyword, Guid? topicId, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetPostPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TopicId = topicId
            };

            var data = await _postApiClient.AdminGetAllPaging(request);
            ViewBag.Keyword = keyword;
            ViewBag.Type = topicId;
            ViewBag.Types = await SeletectType(topicId);
            return View(data.Data);

        }
        public async Task<IActionResult> CreatePost()
        {
            var result = await _userApiClient.GetByUserName(User.Identity.Name);
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
            select.Add(new SelectListItem()
            {
                Text = "Tất cả",
                Value = "",
                Selected = id.ToString() == ""
            });
            return select;

        }
        public async Task<List<SelectListItem>> SeletectTypeMenu(Guid? id)
        {
            var rs = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "Topic" || x.Type == "Category" || x.Type == "Categoryfeature");
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
            if (rs.IsSuccessed)
            {
                fileurl = _configuration["BaseAddress"] + "/img/" + rs.Data;
            }
            return Json(new { url = fileurl });
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _postApiClient.GetById(id);

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
                    Status = Post.Status == Status.Active ? true : false,
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
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _postApiClient.DeleteAdmin(Id);
            return Json(new { response = result });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAction(Guid Id)
        {
            var result = await _postApiClient.DeleteAdmin(Id);
            if (result == 2)
            {
                TempData["AlertMessage"] = "Dừng hoạt động bài viết thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }
            return RedirectToAction("DetailtPost", new {id = Id});
        }
    }
}
