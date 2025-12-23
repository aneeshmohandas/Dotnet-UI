namespace DotnetUI.Interfaces
{
    public interface IProjectManagerService
    {
        void ManageProjectCreationRequest(SolutionCreationRequest request);
        void CreateProject(ProjectCreationModel model, string Location);
        bool CreateSolution(string SolutionName, string Location);
        void AddProjectToSolution(string SolutionLocation, string ProjectLocation);
        void AddPackage(string ProjectLocation, PackageModel package);
        void RemovePackage(string ProjectLocation, PackageModel package);
        void CreateProjectReference(string WorkingDirectory, string FromLocation, string ToLocation);
        void CreateGitIgnoreFile(string Location);
        void RestoreSolution(string Location);
        void CleanSolution(string Location);
        void BuildSolution(string Location);
        void PublishSolution(PublishProfileCreateRequest request);
    }
}