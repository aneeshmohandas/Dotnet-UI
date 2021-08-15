using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DotnetUI.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace DotnetUI
{
    public static class DotnetUiHelper
    {
        public static string[] ExcludeFolders = { "node_modules", "bin", "dev", "es", "de-DE", "nl", "runtimes", "boot", "zh", "zh-TW", "pt-BR", "tests", ".vscode", ".git", "public", "src" };
        public static string ProjectExtension = ".csproj";
        public static string DataLocation = "Data";
        public static string ConfigFilesLocation = "config";
        public static string SystemInfoLocation = "systemInfo.json";
        public static string SolutionFileLocation = "projectData.json";
        public static string AppLogsLocation = "appLogs.json";

        public static List<DirectoryTreeModel> GenerateDirectoryTree()
        {
            var driveInfo = DriveInfo.GetDrives().ToList();
            var result = new List<DirectoryTreeModel>();
            var id = 1;
            var name = "";
            foreach (var drive in driveInfo)
            {
                name = drive.Name;
                var node = new DirectoryTreeModel
                {

                    Name = drive.Name,
                    Path = drive.Name,
                    Id = id,
                    HasChildren = drive.RootDirectory.GetDirectories().Any(x => x.Attributes == FileAttributes.Directory && !Array.Exists(ExcludeFolders, f => f == x.Name))
                };
                result.Add(node);
                id++;
            }
            return result;
        }
        public static List<DirectoryTreeModel> GetDirectoryDetailsByPath(string SearchPath, int id = 1)
        {
            var result = new List<DirectoryTreeModel>();
            DirectoryInfo di = new DirectoryInfo(SearchPath);
            var directories = di.GetDirectories().Where(x => x.Attributes == FileAttributes.Directory && !Array.Exists(ExcludeFolders, f => f == x.Name));
            var count = 1;
            foreach (var dir in directories)
            {
                var currentpath = SearchPath.Substring(SearchPath.Length - 2) == "\\" ? (SearchPath + dir.Name) : (SearchPath + "\\" + dir.Name);
                var childCount = dir.GetDirectories().Where(x => x.Attributes == FileAttributes.Directory && !Array.Exists(ExcludeFolders, f => f == x.Name)).Count();
                var node = new DirectoryTreeModel
                {
                    Name = dir.Name,
                    Path = dir.FullName,
                    Id = id * 100 + count,
                    HasChildren = childCount > 0 ? true : false
                };
                result.Add(node);
                count++;
            }
            return result;
        }
        public static IEnumerable<ProcessStartInfo> ParseCommands(string[] args, string WorkingDirectory = "")
        {
            var argsPrepend = "";
            var shellName = "/bin/bash";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shellName = "cmd";
                argsPrepend = "/c ";
            }

            return args
                .Select(q => new ProcessStartInfo()
                {
                    FileName = shellName,
                    Arguments = argsPrepend + q,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = WorkingDirectory
                }).ToList();
        }
        public static ProcessStartInfo ParseCommands(string args, string WorkingDirectory = "")
        {
            var argsPrepend = "";
            var shellName = "/bin/bash";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                shellName = "cmd";
                argsPrepend = "/c ";
            }

            return new ProcessStartInfo()
            {
                FileName = shellName,
                Arguments = argsPrepend + args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory
            };
        }

        public static ExectedCommandResult RunCommand(ProcessStartInfo processInfo, IHubContext<ApplicationHub> appHub, bool sendMessage = false, string CommandDesc = "")
        {
            var result = new ExectedCommandResult();
            if (sendMessage && !string.IsNullOrEmpty(CommandDesc))
                appHub.Clients.All.SendAsync("ReceiveMessage", CommandDesc);
            result = RunCommand(processInfo);
            if (!string.IsNullOrEmpty(result.Message) || !string.IsNullOrEmpty(result.Error))
            {
                if (sendMessage)
                    appHub.Clients.All.SendAsync("ReceiveMessage", result.Message);
            }
            return result;
        }
        public static ExectedCommandResult RunCommand(ProcessStartInfo processInfo)
        {
            var result = new ExectedCommandResult();
            var process = new Process()
            {
                StartInfo = processInfo,
            };
            process.Start();

            var output = "";
            var sbMsg = new StringBuilder();
            var sbError = new StringBuilder();
            var err = "";
            while (!process.StandardError.EndOfStream)
            {
                sbError.Append(process.StandardError.ReadToEnd());
            }
            err = sbError.ToString();
            while (!process.StandardOutput.EndOfStream)
            {
                sbMsg.Append(process.StandardOutput.ReadToEnd());
            }
            process.WaitForExit();
            var code = process.ExitCode;
            output = sbMsg.ToString();
            if (!string.IsNullOrEmpty(output))
            {
                result.Succeed = code == 0 ? true : false;
                result.Message = output;
                result.Error = err;
            }
            return result;
        }
        public static List<ProjectTemplateModel> GetProjectTemplates(IWebHostEnvironment env)
        {
            var result = new List<ProjectTemplateModel>();
            try
            {
                var templateString = System.IO.File.ReadAllText(Path.Join(env.WebRootPath, "config", "projectTemplateDetails.json"));
                result = JsonConvert.DeserializeObject<List<ProjectTemplateModel>>(templateString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public static List<SolutionCreationRequest> GetProjectList(IWebHostEnvironment env)
        {
            var result = new List<SolutionCreationRequest>();
            try
            {
                if (!File.Exists(Path.Join(env.WebRootPath, DataLocation, SolutionFileLocation)))
                {
                    File.Create(Path.Join(env.WebRootPath, DataLocation, SolutionFileLocation));
                    return result;
                }
                var projectDataStr = File.ReadAllText(Path.Join(env.WebRootPath, DataLocation, SolutionFileLocation));
                if (!string.IsNullOrEmpty(projectDataStr))
                    result = JsonConvert.DeserializeObject<List<SolutionCreationRequest>>(projectDataStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public static bool UpdateProjectList(IWebHostEnvironment env, List<SolutionCreationRequest> data)
        {

            try
            {
                var projectDataStr = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(projectDataStr))
                    System.IO.File.WriteAllText(Path.Join(env.WebRootPath, DataLocation, SolutionFileLocation), projectDataStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }
        public static string GetWorkingDirectory(SolutionCreationRequest request)
        {
            return request.CreateSolution ? Path.Join(request.WorkingDirectory, request.SolutionName) : request.WorkingDirectory;
        }
        public static List<Activities> GetActivities(IWebHostEnvironment env)
        {
            var result = new List<Activities>();
            try
            {
                if (!File.Exists(Path.Join(env.WebRootPath, DataLocation, AppLogsLocation)))
                {
                    File.Create(Path.Join(env.WebRootPath, DataLocation, AppLogsLocation));
                    return result;
                }
                var logDataStr = File.ReadAllText(Path.Join(env.WebRootPath, DataLocation, AppLogsLocation));
                if (!string.IsNullOrEmpty(logDataStr))
                    result = JsonConvert.DeserializeObject<List<Activities>>(logDataStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public static bool UpdateActivities(IWebHostEnvironment env, List<Activities> data)
        {

            try
            {
                var logDataStr = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(logDataStr))
                    System.IO.File.WriteAllText(Path.Join(env.WebRootPath, DataLocation, AppLogsLocation), logDataStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }
        public static List<AppConfigurationSystemInfo> GetSystemInfo(IWebHostEnvironment env)
        {
            var result = new List<AppConfigurationSystemInfo>();
            try
            {
                if (!File.Exists(Path.Join(env.WebRootPath, ConfigFilesLocation, SystemInfoLocation)))
                {
                    File.Create(Path.Join(env.WebRootPath, ConfigFilesLocation, SystemInfoLocation));
                    return result;
                }
                var projectDataStr = File.ReadAllText(Path.Join(env.WebRootPath, ConfigFilesLocation, SystemInfoLocation));
                if (!string.IsNullOrEmpty(projectDataStr))
                    result = JsonConvert.DeserializeObject<List<AppConfigurationSystemInfo>>(projectDataStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        public static bool UpdateSystemInfo(IWebHostEnvironment env, List<AppConfigurationSystemInfo> data)
        {

            try
            {
                var systemInfoStr = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(systemInfoStr))
                    System.IO.File.WriteAllText(Path.Join(env.WebRootPath, ConfigFilesLocation, SystemInfoLocation), systemInfoStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }
        public static bool AppIntialSetup(IWebHostEnvironment _env)
        {
            var info = GetSystemInfo(_env);
            if (info.Count == 0)
            {
                var osInfo = GetOSInfo(_env);
                if (osInfo == null)
                {
                    return false;
                }
                info.Add(osInfo);
                var dotnetVersion = GetDotnetVersion(_env, osInfo.Value);
                if (dotnetVersion != null && dotnetVersion.Count > 0)
                    info.AddRange(dotnetVersion);
                var editorDetails = GetCodeEditorVersion(_env);
                if (editorDetails != null)
                    info.Add(editorDetails);
                var defaultWrkingDirectory = new AppConfigurationSystemInfo
                {
                    Type = "WorkingDirectory",
                    Key = "Default",
                    Value = _env.ContentRootPath,

                };
                info.Add(defaultWrkingDirectory);
                return UpdateSystemInfo(_env, info);

            }
            else
            {
                return true;
            }


        }

        public static List<AppConfigurationSystemInfo> GetDotnetVersion(IWebHostEnvironment _env, string Platform = "", bool SaveData = false)
        {
            var cmdResult = DotnetUiHelper.RunCommand(" dotnet --list-sdks ");
            if (cmdResult.Succeed)
            {
                var sdks = cmdResult.Message.Split("\n").ToList();
                var dotnetVersion = new List<AppConfigurationSystemInfo>();
                var pattern = @"\[(.*?)\]";
                var regex = new Regex(pattern);
                sdks.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x))
                    {
                        //
                        var r = regex.Split(x);
                        if (r.Length >= 2)
                        {
                            dotnetVersion.Add(new AppConfigurationSystemInfo()
                            {
                                Type = "dotnet",
                                Key = "version",
                                Value = r[0].Trim(),
                                Remarks = r[1].Trim()

                            });
                        }

                    }
                });
                return dotnetVersion;
            }
            else
                return null;
        }
        // only checking for vscode
        public static AppConfigurationSystemInfo GetCodeEditorVersion(IWebHostEnvironment _env, bool SaveData = false)
        {
            var cmdResult = DotnetUiHelper.RunCommand(" code --version ");
            if (cmdResult.Succeed)
            {
                var vscodeDetails = cmdResult.Message.Split("\n").ToList();
                var vscodeVersion = new AppConfigurationSystemInfo()
                {
                    Type = "Editor",
                    Key = "vscode",
                    Value = vscodeDetails[0],
                    Label = "VS Code"
                };
                return vscodeVersion;
            }
            else
                return null;
        }
        public static AppConfigurationSystemInfo GetOSInfo(IWebHostEnvironment _env, bool SaveData = false)
        {

            var osInfo = new AppConfigurationSystemInfo()
            {
                Type = "OS",
                Key = "Platform",
                Label = "OS Info"
            };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osInfo.Value = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osInfo.Value = "Linux";
            }
            else
            {
                return null;
            }
            return osInfo;

        }
        public static ExectedCommandResult RunCommand(string Command)
        {
            var result = new ExectedCommandResult();
            var cmd = new Process();
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(OSPlatform.Windows);
            //Console.WriteLine(isWindows);

            if (isWindows)
            {
                cmd.StartInfo.FileName = "cmd.exe";
            }
            else
            {
                cmd.StartInfo.FileName = "/bin/bash";
            }
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;

            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;

            cmd.Start();

            /* execute "dir" */

            cmd.StandardInput.WriteLine(Command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            var output = cmd.StandardOutput.ReadToEnd();
            var err = cmd.StandardError.ReadToEnd();
            var code = cmd.ExitCode;
            if (!string.IsNullOrEmpty(output))
            {
                result.Succeed = code == 0 ? true : false;
                result.Message = output;
                result.Error = err;
            }
            return result;
        }




    }
}