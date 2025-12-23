using System;
using System.Linq;
using System.Threading.Tasks;
using DotnetUI.Hubs;
using DotnetUI.Interfaces;
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
       
        [HttpPost]
        [Route("/api/dotnetui/home/CreateProject")]
        public IActionResult CreateProject([FromServices] IWebHostEnvironment environment, [FromServices] IHubContext<ApplicationHub> appHub, [FromBody] SolutionCreationRequest model)
        {
            _srv.ManageProjectCreationRequest(model);
            return Ok(model);
        }
      
        [HttpGet]
        [Route("/api/dotnetui/home/error")]
        public IActionResult Index()
        {
            throw new Exception("Testing custom exception filter.");
        }
    }
}