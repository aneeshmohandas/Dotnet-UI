using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DotnetUI.Hubs;
using DotnetUI.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DotnetUI.Service
{
    public class ExecuteCommand : IExecuteCommand
    {
        readonly IHubContext<ApplicationHub> _appHub;
        public ExecuteCommand(IHubContext<ApplicationHub> appHub)
        {
            _appHub = appHub;
        }
        public IEnumerable<ProcessStartInfo> ParseCommands(string[] args, string WorkingDirectory = "")
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
        public ProcessStartInfo ParseCommands(string args, string WorkingDirectory = "")
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
        public ExectedCommandResult RunCommand(ProcessStartInfo processInfo, bool sendMessage = false, string CommandDesc = "")
        {
            var result = new ExectedCommandResult();
            if (sendMessage && !string.IsNullOrEmpty(CommandDesc))
                _appHub.Clients.All.SendAsync("ReceiveMessage", CommandDesc);
            result = RunCommand(processInfo);
            if (!string.IsNullOrEmpty(result.Message) || !string.IsNullOrEmpty(result.Error))
            {
                if (sendMessage)
                    _appHub.Clients.All.SendAsync("ReceiveMessage", result.Message);
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

        public ExectedCommandResult RunCommand(string Command, string WorkingDirectory = "", bool sendMessage = false, string CommandDesc = "")
        {
            var result = new ExectedCommandResult();
            if (sendMessage && !string.IsNullOrEmpty(CommandDesc))
                _appHub.Clients.All.SendAsync("ReceiveMessage", CommandDesc);
            result = RunCommand(Command, WorkingDirectory);
            if (!string.IsNullOrEmpty(result.Message) || !string.IsNullOrEmpty(result.Error))
            {
                if (sendMessage)
                    _appHub.Clients.All.SendAsync("ReceiveMessage", result.Message);
            }
            return result;
        }
        private ExectedCommandResult RunCommand(string Command, string WorkingDirectory)
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
            if (!string.IsNullOrEmpty(WorkingDirectory))
                cmd.StartInfo.WorkingDirectory=WorkingDirectory;
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