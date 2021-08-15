using System;
using System.Collections.Generic;
using System.Linq;
using DotnetUI.Service;
using Microsoft.AspNetCore.Mvc;
namespace DotnetUI
{
    public class ActivityController : Controller
    {
        IActivityService _actsrv;
        public ActivityController(IActivityService actsrv)
        {
            _actsrv = actsrv;
        }
        [HttpGet]
        [Route("/api/dotnetui/activity/timeline")]
        public IActionResult GetData(string Type)
        {
            var res = new List<Activities>();
            switch (Type.ToLower().Replace(" ", ""))
            {
                case "today":
                    var date = DateTime.Now;
                    res = _actsrv.GetList(date.Date, date);
                    break;
                case "last24hours":
                    date = DateTime.Now;
                    res = _actsrv.GetList(date.AddDays(-1), date);
                    break;
                case "lastweek":
                    date = DateTime.Now;
                    res = _actsrv.GetList(date.AddDays(-7), date);
                    break;
            }
            res = res.OrderByDescending(x => x.CreatedOn).ToList();
            return Ok(res);
        }
    }
}