using DoctorManagement.ApiIntegration;
using DoctorManagement.ViewModels.System.AnnualServiceFee;
using Microsoft.AspNetCore.Mvc;

namespace DoctorManagement.AdminApp.Controllers
{
    public class ServiceFeeController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly ILocationApiClient _locationApiClient;
        private readonly IAnnualServiceFeeApiClient _annualServiceFeeApiClient;

        public ServiceFeeController(IUserApiClient userApiClient,
            IConfiguration configuration, ILocationApiClient locationApiClient,
            IAnnualServiceFeeApiClient annualServiceFeeApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _locationApiClient = locationApiClient;
            _annualServiceFeeApiClient = annualServiceFeeApiClient;
        }
        public async Task<IActionResult> ServiceFeePaging(string keyword, string rolename, int pageIndex = 1, int pageSize = 10)
        {
            /* if (ViewBag.Role != null)
             {
                 rolename = ViewBag.Role;
             }
             if (rolename == null)
             {
                 rolename = "all";
             }
             ViewBag.rolename = SeletectRole(rolename);*/

            var request = new GetAnnualServiceFeePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                //RoleName = rolename
            };
            var data = await _annualServiceFeeApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            
           return View(data.Data);

           
        }
        public async Task<IActionResult> DetailtServiceFee(Guid id)
        {
            var result = await _annualServiceFeeApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                return View(result.Data);
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
