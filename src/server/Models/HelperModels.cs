using System;
using System.Collections.Generic;

namespace DotnetUI
{
    public class DirectoryTreeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<DirectoryTreeModel> Children { get; set; } = new List<DirectoryTreeModel>();
        public bool HasChildren { get; set; }
        public bool IsDrive { get; set; }
    }
    public class PackageModel
    {
        public Guid PackageId { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public decimal TotalDownloads { get; set; }
        public String Description { get; set; }
        public string IconUrl { get; set; }
        public bool IsNew { get; set; } = false;
        public bool Removed { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? RemovedOn { get; set; } = null;
    }
    public class ProjectCreationModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> LinkProject { get; set; }
        public List<PackageModel> Packages { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; } = null;

        public string LastBuildStatus { get; set; }
        public string LastPublishStatus { get; set; }

    }
    public class ProjectTemplateModel
    {
        public string TemplateName { get; set; }
        public string ShortName { get; set; }
        public string Language { get; set; }
        public string Tags { get; set; }
        public bool Enabled { get; set; } = true;
        public int? Order { get; set; }
    }
    public class SolutionCreationRequest
    {
        public Guid SolutionId { get; set; }
        public List<ProjectCreationModel> Projects { get; set; }
        public bool CreateSolution { get; set; }
        public bool CreateGitIgnoreFile { get; set; }
        public string WorkingDirectory { get; set; }
        public string SolutionName { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; } = null;
        public string Description { get; set; }
        public List<ProjectPublishProfile> PublishProfiles { get; set; } = new List<ProjectPublishProfile>();
        public string Language { get; set; } = "C#";
    }
    public class AddPackageToProjectModel
    {
        public PackageModel Package { get; set; }
        public Guid ProjectId { get; set; }
        public Guid SolutionId { get; set; }

    }
    public class AddProjectToSolutionModel
    {
        public ProjectCreationModel Project { get; set; }
        public Guid SolutionId { get; set; }

    }
    public class ProjectPublishProfile
    {
        public Guid Id { get; set; }
        public string ProfileName { get; set; }
        public string Configuration { get; set; } = "Release";
        public string OutputDirectory { get; set; }
        public string Framework { get; set; }
        public bool NoBuild { get; set; }
        public bool NoRestore { get; set; }
        public bool NoDependencies { get; set; }
        public string RunTime { get; set; }
        public bool SelfContained { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? LastModifiedTime { get; set; } = null;
    }
    public class PublishProfileCreateRequest : ProjectPublishProfile
    {
        public Guid SolutionId { get; set; }
        public bool SaveAsProfile { get; set; }
        public string Location { get; set; }
    }
    public class ExectedCommandResult
    {
        public bool Succeed { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
    public class Activities
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? SolutionId { get; set; } = null;
        public Guid? ProjectId { get; set; } = null;
    }
    public class AppConfigurationSystemInfo
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }
        public string Label { get; set; }
    }

}