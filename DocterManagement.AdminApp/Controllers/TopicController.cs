using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Topic;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class TopicController : BaseController
    {
        private readonly ITopicApiClient _topicApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IUserApiClient _userApiClient;

        public TopicController(ITopicApiClient TopicApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IUserApiClient userApiClient)
        {
            _topicApiClient = TopicApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {

            var request = new GetTopicPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _topicApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] TopicCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _topicApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thêm mới phòng khám " + request.Titile + " thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }


            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _topicApiClient.GetById(id);

            if (result.IsSuccessed)
            {
                var Topic = result.Data;

                ViewBag.Image = Topic.Image;

                var updateRequest = new TopicUpdateRequest()
                {
                    Titile = Topic.Titile,
                    Id = id,
                    IsDeleted = Topic.IsDeleted,
                    Description = Topic.Description,
                    ImageText = Topic.Image,
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] TopicUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _topicApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám " + request.Titile + " thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }


            return View(request);
        }

        public async Task<IActionResult> DetailtTopic(Guid id)
        {
            var result = await _topicApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _topicApiClient.Delete(Id);
            return Json(new { response = result });
        }

    }
}
