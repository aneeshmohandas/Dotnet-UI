using System;
using System.IO;
using System.Linq;
using DotnetUI.Common;
using DotnetUI.Hubs;
using DotnetUI.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
namespace DotnetUI.Service
{
    public class ProjectManagerService : IProjectManagerService
    {
        IWebHostEnvironment _env;
        IHubContext<ApplicationHub> _appHub;
        IActivityService _actServ;
        IExecuteCommand _cmd;
        public ProjectManagerService(IWebHostEnvironment env, IHubContext<ApplicationHub> appHub, IActivityService actServ, IExecuteCommand cmd)
        {
            _env = env;
            _appHub = appHub;
            _actServ = actServ;
            _cmd = cmd;
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
                        CreateProjectReference(wrkingDirectory, $"{Path.Join(referenceProject.Name, referenceProject.Name + DotnetUiHelper.ProjectExtension)}", $"{Path.Join(p.Name, p.Name + DotnetUiHelper.ProjectExtension)}");
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
            var command = $" dotnet add {ProjectLocation} package  {package.Id} -v {package.Version}";
            package.IsNew = false;
            package.AddedOn = DateTime.Now;
            package.PackageId = Guid.NewGuid();
            _cmd.RunCommand(command);
        }
        public void AddProjectToSolution(string SolutionLocation, string ProjectLocation)
        {
            var command = $" dotnet sln  add {ProjectLocation} ";
            _cmd.RunCommand(command, SolutionLocation);
        }

        public void CreateProject(ProjectCreationModel model, string Location)
        {
            var templates = DotnetUiHelper.GetProjectTemplates(_env);
            model.Id = Guid.NewGuid();
            var type = templates.First(x => x.TemplateName == model.Type).ShortName;
            model.CreatedTime = DateTime.Now;
            var command = $" dotnet new {type} -n {model.Name} -o {Path.Join(Location,model.Name)} ";
            _cmd.RunCommand(command);

        }

        public void CreateProjectReference(string WorkingDirectory, string FromLocation, string ToLocation)
        {
            var command = $" dotnet add  {ToLocation} reference {FromLocation} ";
            _cmd.RunCommand(command, WorkingDirectory);

        }

        public bool CreateSolution(string SolutionName, string Location)
        {
            var command = $" dotnet new sln -n {SolutionName} -o {Path.Join(Location,SolutionName)}";
            var result = _cmd.RunCommand(command, CommandDesc: $" Creating Solution {SolutionName} \n");
            return result.Succeed;
        }
        public void RemovePackage(string ProjectLocation, PackageModel package)
        {
        }

        public void CreateGitIgnoreFile(string Location)
        {
            var command = $" dotnet new gitignore  -o {Location} ";
            _cmd.RunCommand(command);
        }

        public void RestoreSolution(string Location)
        {
            var command = $" dotnet restore {Location} ";
            _cmd.RunCommand(command);
        }

        public void CleanSolution(string Location)
        {
            var command = $" dotnet clean {Location} ";
            _cmd.RunCommand(command);
        }

        public void BuildSolution(string Location)
        {
            var command = $" dotnet build {Location} ";
            _cmd.RunCommand(command, CommandDesc: " Building Solution...\n ");

        }

        public void PublishSolution(PublishProfileCreateRequest request)
        {
            var command = $" dotnet publish {request.Location} -c {request.Configuration} -o {request.OutputDirectory} ";
            _cmd.RunCommand(command, CommandDesc: " Publishing Solution...\n ");

        }
    }
}