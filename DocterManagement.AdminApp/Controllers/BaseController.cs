using DoctorManagement.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorManagement.AdminApp.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessions = context.HttpContext.Session.GetString("Token");

            if (sessions == null)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
        public string SetCount(int count)
        {
            switch (count)
            {
                case >= 1000 and < 1000000:
                    return count / 1000 + "K";
                case >= 1000000 and < 1000000000:
                    return count / 1000000 + "M";
                case >= 1000000000:
                    return count / 1000000000 + "B";
                default:
                    return count.ToString();
            }
        }
        public List<SelectListItem> SeletectDay(string day)
        {
            List<SelectListItem> selectListDay = new List<SelectListItem>();
            for (int i = 1; i <= 31; i++)
            {
                selectListDay.Add(new SelectListItem(text: "Ng " + i, value: i < 10 ? ("0" + i.ToString()) : i.ToString()));
            }
            var rs = selectListDay.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = day.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectStatus(Status status)
        {
            List<SelectListItem> lstatus = new List<SelectListItem>()
            {
                new SelectListItem(text: "Ngừng hoạt động", value: Status.NotActivate.ToString()),
                new SelectListItem(text: "Hoạt động", value: Status.Active.ToString()),
                new SelectListItem(text: "Không hoạt động", value: Status.InActive.ToString())
            };
            var rs = lstatus.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = status.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectMonth(string month)
        {
            List<SelectListItem> selectListMonth = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                selectListMonth.Add(new SelectListItem(text: "Thg " + i, value: i < 10?("0"+i.ToString()):i.ToString()));
            }
            var rs = selectListMonth.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = month.ToString() == x.Value
            }).ToList();
            return rs;
        }
        public List<SelectListItem> SeletectYear(string year)
        {
            var yearnow = DateTime.Now.ToString("yyyy");
            List<SelectListItem> selectListYear = new List<SelectListItem>();
            for (int i = int.Parse(yearnow) - 50; i <= int.Parse(yearnow); i++)
            {
                selectListYear.Add(new SelectListItem(text: i.ToString(), value: i.ToString()));
            }
            var rs = selectListYear.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = year.ToString() == x.Value
            }).ToList();
            return rs;
        }
    }
}
