using System.Collections.Generic;
using System.Linq;
using DotnetUI.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DotnetUI
{
    public class ProjectManagerController : Controller
    {
        IProjectManagerService _srv;
        IWebHostEnvironment _env;
        public ProjectManagerController(IProjectManagerService srv, IWebHostEnvironment env)
        {
            _srv = srv;
            _env = env;
        }
        [HttpGet]
        [Route("/api/dotnetui/ProjectManager/List")]
        public IActionResult SolutionList(string Filter = "", int Limit = -1)
        {
            var res = DotnetUiHelper.GetProjectList(_env);
            res = res.OrderByDescending(x => x.LastModifiedTime ?? x.CreatedTime).ToList();
            if (!string.IsNullOrEmpty(Filter))
                res = res.Where(x => x.SolutionName.Contains(Filter)).ToList();
            if (Limit > 0)
                res = res.Take(Limit).ToList();
            return Ok(res);
        }
        [HttpGet]
        [Route("/api/dotnetui/ProjectManager/AutoComplete")]
        public IActionResult SolutionAutoComplete(string Filter = "", int Limit = 20)
        {
            var res = DotnetUiHelper.GetProjectList(_env);
            var drpdwnList = new List<string>();
            if (string.IsNullOrEmpty(Filter))
                drpdwnList = res.Select(x => x.SolutionName).Take(Limit).ToList();
            else
                drpdwnList = res.Where(x => x.SolutionName.ToLower().Contains(Filter.ToLower().Trim())).Select(x => x.SolutionName).Take(Limit).ToList();
            return Ok(drpdwnList);
        }
    }
}