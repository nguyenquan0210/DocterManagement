using DoctorManagement.ApiIntegration;
using DoctorManagement.Data.Enums;
using DoctorManagement.ViewModels.Catalog.Schedule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.DoctorApp.Controllers
{
    public class ScheduleController : BaseController
    {
        private readonly IScheduleApiClient _scheduleApiClient;
        private readonly IConfiguration _configuration;

        public ScheduleController(IScheduleApiClient ScheduleApiClient,
            IConfiguration configuration)
        {
            _scheduleApiClient = ScheduleApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new GetSchedulePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _scheduleApiClient.GetSchedulePagings(request);
            ViewBag.Keyword = keyword;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.TimeLine = SeletectTimeLine("");
            ViewBag.WeekDay = SeletectWeekDay("");
            ViewBag.Selection = new List<SelectListItem>();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetSelection(string chose)
        {
            if (!string.IsNullOrWhiteSpace(chose))
            {
                chose = chose.Trim();
                var district = new List<SelectListItem>();
                if(chose == "1") district = SeletectManyDay(chose);
                else if(chose == "2") district = SeletectManyWeek(chose);
                else district = SeletectManyMonth(chose);
                return Json(district);
            }
            return null;
        }
        public List<SelectListItem> SeletectTimeLine(string day)
        {
            List<SelectListItem> selectListDay = new List<SelectListItem>();
            selectListDay.Add(new SelectListItem(text: "Ngày", value: "1"));
            selectListDay.Add(new SelectListItem(text: "Tuần", value: "7"));
            selectListDay.Add(new SelectListItem(text: "Tháng", value: "24"));
            var rs = selectListDay.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = day.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectManyDay(string day)
        {
            List<SelectListItem> selectListDay = new List<SelectListItem>();
            for (int i = 1; i <= 31; i++)
            {
                selectListDay.Add(new SelectListItem(text: i + " Ngày", value: i.ToString()));
            }
            var rs = selectListDay.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = day.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectManyWeek(string week)
        {
            List<SelectListItem> selectListWeek = new List<SelectListItem>();
            for (int i = 1; i <= 4; i++)
            {
                selectListWeek.Add(new SelectListItem(text: i + " Tuần", value: i.ToString()));
            }
            var rs = selectListWeek.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = week.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectManyMonth(string month)
        {
            List<SelectListItem> selectListMonth = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                selectListMonth.Add(new SelectListItem(text: i + " Tháng", value: i.ToString()));
            }
            var rs = selectListMonth.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = month.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectWeekDay(string weekDay)
        {
            List<SelectListItem> selectWeekDay = new List<SelectListItem>();

            selectWeekDay.Add(new SelectListItem(text:"Thứ 2", value: "Monday"));
            selectWeekDay.Add(new SelectListItem(text:"Thứ 3", value:"Tuesday" ));
            selectWeekDay.Add(new SelectListItem(text: "Thứ 4", value:"Wednesday" ));
            selectWeekDay.Add(new SelectListItem(text: "Thứ 5", value: "Thursday"));
            selectWeekDay.Add(new SelectListItem(text: "Thứ 6", value: "Friday"));
            selectWeekDay.Add(new SelectListItem(text: "Thứ 7", value: "Saturday"));
            selectWeekDay.Add(new SelectListItem(text: "Chủ nhật", value: "Sunday"));

            var rs = selectWeekDay.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = weekDay.ToString() == x.Value
            }).ToList();
            return rs;
        }
        [HttpPost]
        public async Task<IActionResult> Create(ScheduleCreateRequest request)
        {
            //ViewBag.TimeLine = SeletectTimeLine(request.TimeLine.ToString());
            ViewBag.WeekDay = SeletectWeekDay(request.WeekDay);
            /*if (request.TimeLine.ToString() == "1") ViewBag.Selection = SeletectManyDay(request.Selection.ToString());
            else if (request.TimeLine.ToString() == "7") ViewBag.Selection = SeletectManyWeek(request.Selection.ToString());
            else ViewBag.Selection = SeletectManyMonth(request.Selection.ToString());*/
            if (!ModelState.IsValid)
                return View();
            request.Username = User.Identity.Name;
            var result = await _scheduleApiClient.Create(request);

            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Đăt khám thành công";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _scheduleApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var Schedule = result.Data;
                var updateRequest = new ScheduleUpdateRequest()
                {
                    FromTime = Schedule.FromTime,
                    Id = id,
                    ToTime = Schedule.ToTime,
                    Status = Schedule.IsDeleted ,
                    Qty = Schedule.Qty,
                    AvailableQty = Schedule.AvailableQty,
                    BookedQty = Schedule.BookedQty,
                    CheckInDate = Schedule.CheckInDate.ToShortDateString(),
                };
                return View(updateRequest);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(ScheduleUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _scheduleApiClient.Update(request);
            if (result.IsSuccessed)
            {
                TempData["AlertMessage"] = "Thay đổi thông tin phòng khám thành công.";
                TempData["AlertType"] = "alert-success";
                return RedirectToAction("Index");
            }

            TempData["AlertMessage"] = result.Message;
            TempData["AlertType"] = "alert-error";
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Detailt(Guid id)
        {
            var result = await _scheduleApiClient.GetById(id);
            if (result.IsSuccessed)
            {
                var ScheduleData = result.Data;
                //ViewBag.Status = ScheduleData.Status == Status.InActive ? "Ngừng hoạt động" : ScheduleData.Status == Status.Active ? "Hoạt động" : "không hoạt động";
                var Schedule = new ScheduleVm()/*_mapper.Map<ScheduleVm>(Scheduledata);*/
                {
                    FromTime = ScheduleData.FromTime,
                    Id = id,
                    ToTime = ScheduleData.ToTime,
                    CheckInDate = ScheduleData.CheckInDate,
                    IsDeleted = ScheduleData.IsDeleted, //== Status.Active ? true : false
                    Qty = ScheduleData.Qty,
                    ScheduleDetailts = ScheduleData.ScheduleDetailts
                };
                return View(Schedule);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = await _scheduleApiClient.Delete(Id);
            return Json(new { response = result });
        }
    }
}
