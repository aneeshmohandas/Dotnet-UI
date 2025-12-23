using System.Collections.Generic;
using System.Diagnostics;

namespace DotnetUI.Interfaces
{
    public interface IExecuteCommand
    {
        IEnumerable<ProcessStartInfo> ParseCommands(string[] args, string WorkingDirectory = "");
        ProcessStartInfo ParseCommands(string args, string WorkingDirectory = "");
        ExectedCommandResult RunCommand(string Command, string WorkingDirectory = "", bool sendMessage = true, string CommandDesc = "");
        
    }
}