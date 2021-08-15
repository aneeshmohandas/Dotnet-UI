using DotnetUI.Common;
using DotnetUI.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetUI.Hubs
{
    public interface IApplicationHub
    {
        Task BlockUI();
        Task EnableUI();
        Task UpdatedProject(ProjectCreationModel model);
        Task UpdatedSolution(SolutionCreationRequest model);
        Task RefreshSolutionList();
    }
    public class ApplicationHub : Hub<IApplicationHub>
    {
        IProjectManagerService _service;
        IWebHostEnvironment _env;
        IActivityService _actServ;
        public ApplicationHub(IProjectManagerService service, IWebHostEnvironment env, IActivityService actServ)
        {
            _service = service;
            _env = env;
            _actServ = actServ;
        }
        public async Task AddNewPackageToProject(AddPackageToProjectModel model)
        {
            await Clients.All.BlockUI();
            var solutions = DotnetUiHelper.GetProjectList(_env);
            var currentSolution = solutions.First(x => x.SolutionId == model.SolutionId);

            var wrkingDirectory = DotnetUiHelper.GetWorkingDirectory(currentSolution);
            var project = currentSolution.Projects.First(x => x.Id == model.ProjectId);
            _service.AddPackage(wrkingDirectory + "\\" + project.Name + "\\" + project.Name + ".csproj", model.Package);
            currentSolution.LastModifiedTime = DateTime.Now;
            project.LastModifiedTime = DateTime.Now;
            project.Packages.Add(model.Package);
            DotnetUiHelper.UpdateProjectList(_env, solutions);
            await Clients.All.UpdatedProject(project);
            await Clients.All.UpdatedSolution(currentSolution);
            await Clients.All.EnableUI();
            _actServ.Save(new Activities
            {
                Id = Guid.NewGuid(),
                Type = (int)LogType.PackageAdded,
                SolutionId = currentSolution.SolutionId,
                ProjectId = project.Id,
                Message = $"New Package {model.Package.Title} Added in {project.Name} ",
                CreatedOn = model.Package.AddedOn

            });
        }
        public async Task AddNewProjectToSolution(AddProjectToSolutionModel model)
        {
            await Clients.All.BlockUI();
            var solutions = DotnetUiHelper.GetProjectList(_env);
            var currentSolution = solutions.First(x => x.SolutionId == model.SolutionId);
            var wrkingDirectory = DotnetUiHelper.GetWorkingDirectory(currentSolution);
            _service.CreateProject(model.Project, wrkingDirectory);
            if (currentSolution.CreateSolution)
                _service.AddProjectToSolution(wrkingDirectory, $" {model.Project.Name}\\{model.Project.Name}.csproj");
            model.Project.Packages.ForEach(p =>
            {
                _service.AddPackage(wrkingDirectory + "\\" + model.Project.Name + "\\" + model.Project.Name + ".csproj", p);
            });
            foreach (var pro in model.Project.LinkProject)
            {
                if (!string.IsNullOrEmpty(pro) && pro != model.Project.Name)
                {
                    var referenceProject = currentSolution.Projects.First(x => x.Name == pro);
                    _service.CreateProjectReference(wrkingDirectory, $"{referenceProject.Name}\\{referenceProject.Name}.csproj", $"{model.Project.Name}\\{model.Project.Name}.csproj");
                }
            }
            currentSolution.LastModifiedTime = DateTime.Now;
            currentSolution.Projects.Add(model.Project);
            DotnetUiHelper.UpdateProjectList(_env, solutions);
            await Clients.All.UpdatedSolution(currentSolution);
            await Clients.All.EnableUI();
            _actServ.Save(new Activities
            {
                Id = Guid.NewGuid(),
                Type = (int)LogType.ProjectAdded,
                SolutionId = currentSolution.SolutionId,
                ProjectId = model.Project.Id,
                Message = $"New Project {model.Project.Name} Added in {currentSolution.SolutionName} ",
                CreatedOn = model.Project.CreatedTime

            });
        }
        public async Task ManageProjectCreationRequest(SolutionCreationRequest model)
        {
            await Clients.All.BlockUI();
            _service.ManageProjectCreationRequest(model);
            await Clients.All.RefreshSolutionList();
            await Clients.All.EnableUI();
        }
        public async Task RestoreSolution(string Location)
        {
            await Clients.All.BlockUI();
            _service.RestoreSolution(Location);
            await Clients.All.EnableUI();
        }
        public async Task CleanSolution(string Location)
        {
            await Clients.All.BlockUI();
            _service.CleanSolution(Location);
            await Clients.All.EnableUI();
        }
        public async Task BuildSolution(string Location)
        {
            await Clients.All.BlockUI();
            _service.BuildSolution(Location);
            await Clients.All.EnableUI();
        }
        public async Task PublishSolution(PublishProfileCreateRequest request)
        {
            await Clients.All.BlockUI();
            var solutions = DotnetUiHelper.GetProjectList(_env);
            var currentSolution = solutions.First(x => x.SolutionId == request.SolutionId);
            var wrkingDirectory = DotnetUiHelper.GetWorkingDirectory(currentSolution);
            request.Location = wrkingDirectory;
            _service.PublishSolution(request);
            if (request.SaveAsProfile && request.Id == Guid.Empty)
            {
                currentSolution.PublishProfiles.Add(new ProjectPublishProfile
                {
                    ProfileName = string.IsNullOrEmpty(request.ProfileName) ? "Profile " + (currentSolution.PublishProfiles.Count() + 1) : request.ProfileName,
                    OutputDirectory = request.OutputDirectory,
                    Configuration = request.Configuration,
                    Id = Guid.NewGuid(),
                    CreatedTime = DateTime.Now
                });
                currentSolution.LastModifiedTime = DateTime.Now;
                DotnetUiHelper.UpdateProjectList(_env, solutions);
            }
            await Clients.All.RefreshSolutionList();
            await Clients.All.EnableUI();
        }
    }
}