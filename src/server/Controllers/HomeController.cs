using System;
using System.Linq;
using System.Threading.Tasks;
using DotnetUI.Hubs;
using DotnetUI.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DotnetUI
{
    public class HomeController : Controller
    {
        IProjectManagerService _srv;
        public HomeController(IProjectManagerService srv)
        {
            _srv = srv;
        }
        [HttpGet]
        [Route("/api/dotnetui/home/getdata")]
        public IActionResult GetData()
        {
            var res = DotnetUiHelper.GenerateDirectoryTree();
            return Ok(res);
        }
        [HttpGet]
        [Route("/api/dotnetui/home/directoryList")]
        public IActionResult DirectoryList(string Path, int Id)
        {
            var res = DotnetUiHelper.GetDirectoryDetailsByPath(Path, Id);
            return Ok(res);
        }

        [HttpGet]
        [Route("/api/dotnetui/home/GetTemplates")]
        public IActionResult GetTemplates([FromServices] IWebHostEnvironment environment)
        {
            var res = DotnetUiHelper.GetProjectTemplates(environment).Where(x => x.Enabled).OrderBy(y => y.Order).Select(x => x.TemplateName);
            return Ok(res);
        }
        [HttpGet]
        [Route("/api/dotnetui/home/testApi")]
        public IActionResult TestApi([FromServices] IWebHostEnvironment _env)
        {
            var res = DotnetUiHelper.GetProjectList(_env);
            foreach (var item in res)
            {
                foreach (var proj in item.Projects)
                {
                    proj.Packages.ForEach(p =>
                    {
                        if (p.PackageId == Guid.Empty)
                        {
                            p.PackageId = Guid.NewGuid();
                        }
                        if (p.AddedOn == DateTime.MinValue)
                        {
                            p.AddedOn = DateTime.Now;
                        }
                        if (p.RemovedOn == DateTime.MinValue)
                        {
                            p.RemovedOn = null;
                        }

                    });
                }
            }
            return Ok(res);
        }
        [HttpPost]
        [Route("/api/dotnetui/home/CreateProject")]
        public IActionResult CreateProject([FromServices] IWebHostEnvironment environment, [FromServices] IHubContext<ApplicationHub> appHub, [FromBody] SolutionCreationRequest model)
        {
            _srv.ManageProjectCreationRequest(model);
            return Ok(model);
        }
        [HttpPost]
        [Route("/api/dotnetui/home/ExcuteCommand")]
        public IActionResult ExcuteCommand()
        {
            DotnetUiHelper.RunCommand(" dotnet --list-sdks ");
            return Ok();
        }
    }
}