using DoctorManagement.ApiIntegration;
using DoctorManagement.Utilities.Constants;
using DoctorManagement.ViewModels.Catalog.Post;
using DoctorManagement.ViewModels.System.Statistic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoctorManagement.WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostApiClient _postApiClient;
        private readonly IConfiguration _configuration;
        private readonly IUserApiClient _userApiClient;
        private readonly IMasterDataApiClient _masterDataApiClient;
        private readonly IStatisticApiClient _statisticApiClient;
        private readonly string NAMESAPACE = "DoctorManagement.WebApp.Controllers.Contact";
        public PostController(IPostApiClient postApiClient, IUserApiClient userApiClient, IConfiguration configuration,
            IMasterDataApiClient masterDataApiClient, IStatisticApiClient statisticApiClient)
        {
            _postApiClient = postApiClient;
            _configuration = configuration;
            _userApiClient = userApiClient;
            _masterDataApiClient = masterDataApiClient;
            _statisticApiClient = statisticApiClient;
        }
        public async Task HistoryActive(HistoryActiveCreateRequest request)
        {
            var session = HttpContext.Session.GetString(SystemConstants.History);
            string? usertemporary = null;
            string? user = null;
            string? ServiceName = null;
            if (session != null)
            {
                var currentHistory = JsonConvert.DeserializeObject<HistoryActiveCreateRequest>(session);
                currentHistory.ToTime = DateTime.Now;
                ServiceName = currentHistory.ServiceName + request.MethodName;
                if (ServiceName != request.ServiceName + request.MethodName) await _statisticApiClient.AddActiveUser(currentHistory);
                usertemporary = currentHistory.Usertemporary;
                user = currentHistory.User;
            }
            if (ServiceName == null || ServiceName != request.ServiceName + request.MethodName)
            {
                var history = new HistoryActiveCreateRequest()
                {
                    User = User.Identity.Name == null ? user : User.Identity.Name,
                    Usertemporary = (usertemporary == null && User.Identity.Name == null) ? ("patient" + new Random().Next(10000000, 99999999) + new Random().Next(10000000, 99999999)) : (usertemporary == null ? User.Identity.Name : usertemporary),
                    Type = user == null ? "patientlogout" : "patient",
                    ServiceName = request.ServiceName,
                    MethodName = request.MethodName,
                    ExtraProperties = request.ExtraProperties,
                    Parameters = request.Parameters,
                    FromTime = DateTime.Now
                };

                HttpContext.Session.SetString(SystemConstants.History, JsonConvert.SerializeObject(history));
            }
        }
        public async Task<IActionResult> DetailtPost(Guid id)
        {
            var result = await _postApiClient.GetById(id);
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".DetailtPost",
                MethodName = "GET",
                ExtraProperties = result.IsSuccessed ? "success" : "error",
                Parameters = "{}",
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".SearchPost",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
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
            var historyactive = new HistoryActiveCreateRequest()
            {
                ServiceName = NAMESAPACE + ".Topic",
                MethodName = "Get",
                ExtraProperties = data.IsSuccessed ? "success" : "error",
                Parameters = JsonConvert.SerializeObject(request),
            };
            await HistoryActive(historyactive);
            ViewBag.TopicId = request.TopicId;
            return View(data.Data);
        }
    }
}
