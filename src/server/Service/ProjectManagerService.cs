using System;
using System.IO;
using System.Linq;
using DotnetUI.Common;
using DotnetUI.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
namespace DotnetUI.Service
{
    public class ProjectManagerService : IProjectManagerService
    {
        IWebHostEnvironment _env;
        IHubContext<ApplicationHub> _appHub;
        IActivityService _actServ;
        public ProjectManagerService(IWebHostEnvironment env, IHubContext<ApplicationHub> appHub, IActivityService actServ)
        {
            _env = env;
            _appHub = appHub;
            _actServ = actServ;
        }
        public void ManageProjectCreationRequest(SolutionCreationRequest request)
        {
            var projectData = DotnetUiHelper.GetProjectList(_env);
            var wrkingDirectory = request.WorkingDirectory;
            request.CreatedTime = DateTime.Now;
            if (request.CreateSolution)
            {
                request.SolutionId = Guid.NewGuid();
                if (!CreateSolution(request.SolutionName, request.WorkingDirectory))
                {
                    return;
                }
            }
            wrkingDirectory = DotnetUiHelper.GetWorkingDirectory(request);
            if (request.CreateGitIgnoreFile)
            {
                CreateGitIgnoreFile(wrkingDirectory);
            }
            foreach (var item in request.Projects)
            {
                CreateProject(item, wrkingDirectory);
                if (request.CreateSolution)
                    AddProjectToSolution(wrkingDirectory, Path.Join(item.Name, item.Name + DotnetUiHelper.ProjectExtension));
                item.Packages.ForEach(p =>
                {
                    AddPackage(Path.Join(wrkingDirectory, item.Name, item.Name + DotnetUiHelper.ProjectExtension), p);
                });

            }
            request.Projects.ForEach(p =>
            {
                foreach (var pro in p.LinkProject)
                {
                    if (!string.IsNullOrEmpty(pro) && pro != p.Name)
                    {
                        var referenceProject = request.Projects.First(x => x.Name == pro);
                        CreateProjectReference(wrkingDirectory, $"{referenceProject.Name}\\{referenceProject.Name}.csproj", $"{p.Name}\\{p.Name}.csproj");
                    }
                }
            });
            projectData.Add(request);
            DotnetUiHelper.UpdateProjectList(_env, projectData);
            _actServ.Save(new Activities
            {
                Id = Guid.NewGuid(),
                SolutionId = request.SolutionId,
                Type = (int)LogType.SolutionCreated,
                Message = $" New Solution Created {request.SolutionName} ",
                CreatedOn = request.CreatedTime
            });

        }
        public void AddPackage(string ProjectLocation, PackageModel package)
        {
            var command = DotnetUiHelper.ParseCommands(new string[] { $" dotnet add {ProjectLocation} package  {package.Id} -v {package.Version} " }).ToList();
            package.IsNew = false;
            package.AddedOn = DateTime.Now;
            package.PackageId = Guid.NewGuid();
            foreach (var comm in command)
            {
                DotnetUiHelper.RunCommand(comm, _appHub, true);
            }
        }

        public void AddProjectToSolution(string SolutionLocation, string ProjectLocation)
        {
            var command = DotnetUiHelper.ParseCommands(new string[] { $" dotnet sln  add {ProjectLocation} " }, SolutionLocation).ToList();
            foreach (var comm in command)
            {
                DotnetUiHelper.RunCommand(comm, _appHub, true);
            }
        }

        public void CreateProject(ProjectCreationModel model, string Location)
        {
            var templates = DotnetUiHelper.GetProjectTemplates(_env);
            model.Id = Guid.NewGuid();
            var type = templates.First(x => x.TemplateName == model.Type).ShortName;
            model.CreatedTime = DateTime.Now;
            var command = DotnetUiHelper.ParseCommands(new string[] { $" dotnet new {type} -n {model.Name} -o {Location}\\{model.Name} " }).ToList();
            foreach (var comm in command)
            {
                DotnetUiHelper.RunCommand(comm, _appHub, true);
            }
        }

        public void CreateProjectReference(string WorkingDirectory, string FromLocation, string ToLocation)
        {
            var command = DotnetUiHelper.ParseCommands(new string[] { $" dotnet add  {ToLocation} reference {FromLocation} " }, WorkingDirectory).ToList();
            foreach (var comm in command)
            {
                DotnetUiHelper.RunCommand(comm, _appHub, true);
            }
        }

        public bool CreateSolution(string SolutionName, string Location)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet new sln -n {SolutionName} -o {Location}\\{SolutionName}");
            var result = DotnetUiHelper.RunCommand(command, _appHub, true, $" Creating Solution {SolutionName} \n");
            return result.Succeed;
        }
        public void RemovePackage(string ProjectLocation, PackageModel package)
        {
        }

        public void CreateGitIgnoreFile(string Location)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet new gitignore  -o {Location} ");
            DotnetUiHelper.RunCommand(command, _appHub, true);
        }

        public void RestoreSolution(string Location)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet restore {Location} ");
            DotnetUiHelper.RunCommand(command, _appHub, true);
        }

        public void CleanSolution(string Location)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet clean {Location} ");
            DotnetUiHelper.RunCommand(command, _appHub, true, "");
        }

        public void BuildSolution(string Location)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet build {Location} ");
            DotnetUiHelper.RunCommand(command, _appHub, true, " Building Solution...\n ");

        }

        public void PublishSolution(PublishProfileCreateRequest request)
        {
            var command = DotnetUiHelper.ParseCommands($" dotnet publish {request.Location} -c {request.Configuration} -o {request.OutputDirectory} ");
            DotnetUiHelper.RunCommand(command, _appHub, true, " Publishing Solution...\n ");

        }
    }
}