using System.Collections.Generic;
using System.Linq;
using DotnetUI.Common;
using DotnetUI.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DotnetUI
{
    public class AppConfigController : Controller
    {

        IWebHostEnvironment _env;
        public AppConfigController(IProjectManagerService srv, IWebHostEnvironment env)
        {

            _env = env;
        }
        [HttpGet]
        [Route("/api/dotnetui/AppConfig/DefaultWorkingDirectory")]
        public IActionResult DefaultWorkingDirectory()
        {
            var result = new AppResponse();
            var config = DotnetUiHelper.GetSystemInfo(_env);
            result.Data = new
            {
                Directory = config.First(x => x.Key == "Default" && x.Type == "WorkingDirectory").Value,
                DisableSelection = config.First(x => x.Type == "OS" && x.Key == "Platform").Value == "Windows" ? false : true
            };
            result.Status = System.Net.HttpStatusCode.OK;
            return Ok(result);
        }
    }
}