using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.Catalog.Post;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class PostController : BaseController
    {
        private readonly IPostApiClient _postApiClient;
        private readonly IConfiguration _configuration;
        public PostController(IPostApiClient postApiClient,IConfiguration configuration)
        {
            _postApiClient = postApiClient;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePost()
        {
            return View();
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost(PostCreateRequest request)
        {
            if (!ModelState.IsValid) return View(request);
            var rs = await _postApiClient.Create(request);
            if (rs.IsSuccessed)
            {
                return RedirectToAction("Index");
            }
            return View(request);
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
    }
}
