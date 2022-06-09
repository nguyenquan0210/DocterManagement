using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Post;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.WebApp.Controllers
{
    public class PostController : Controller
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
        public async Task<IActionResult> DetailtPost(Guid id)
        {
            var result = await _postApiClient.GetById(id);
            var request = new GetPostPagingRequest()
            {
                PageIndex = 1,
                PageSize = 10,
            };
            ViewBag.Posts = (await _postApiClient.GetAllPaging(request)).Data.Items;
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        public async Task<IActionResult> SearchPost(string keyword,int pageIndex=1, int pageSize=12)
        {
            if (string.IsNullOrEmpty(keyword)) return RedirectToAction("Post", "Home");
            var request = new GetPostPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            };
            ViewBag.Menus = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.Type == "Category").ToList();
            var data = await _postApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            return View(data.Data);
        }
        public async Task<IActionResult> Topic(Guid id,int pageIndex = 1, int pageSize = 12)
        {
            var topic = await _masterDataApiClient.GetByIdMainMenu(id);
            if(!topic.IsSuccessed) return RedirectToAction("Post","Home");
            ViewBag.Menus = (await _masterDataApiClient.GetAllMainMenu()).Data.Where(x => x.ParentId == id).ToList();
            var request = new GetPostPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TopicId = id
            };
            ViewBag.Topic = topic.Data;
            ViewBag.PostCategory = (await _postApiClient.GetAll()).Data.Where(x=>x.Status == Data.Enums.Status.Active).ToList();
            var data = await _postApiClient.GetAllPaging(request);
            ViewBag.TopicId = request.TopicId;
            return View(data.Data);
        }
    }
}
