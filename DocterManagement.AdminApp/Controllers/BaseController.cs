﻿using Microsoft.AspNetCore.Authorization;
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

        public List<SelectListItem> SeletectDay(string day)
        {
            List<SelectListItem> selectListDay = new List<SelectListItem>();
            for (int i = 1; i <= 31; i++)
            {
                selectListDay.Add(new SelectListItem(text: "Ng " + i, value: i.ToString()));
            }
            var rs = selectListDay.Select(x => new SelectListItem()
            {
                Text = x.Text,
                Value = x.Value,
                Selected = day.ToString() == x.Value
            }).ToList();
            return rs;
        }

        public List<SelectListItem> SeletectMonth(string month)
        {
            List<SelectListItem> selectListMonth = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                selectListMonth.Add(new SelectListItem(text: "Thg " + i, value: i.ToString()));
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
            List<SelectListItem> selectListYear = new List<SelectListItem>();
            for (int i = int.Parse(year) - 50; i <= int.Parse(year); i++)
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