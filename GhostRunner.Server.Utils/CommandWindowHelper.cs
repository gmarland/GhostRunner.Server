using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GhostRunner.Server.Utils
{
    public class CommandWindowHelper
    {
        public static String ProcessCommand(String commandWorkingDirectory, String commandLineAction)
        {
            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = commandWorkingDirectory;

            Process proc = System.Diagnostics.Process.Start(psi);
            proc.StandardInput.WriteLine(commandLineAction);
            proc.StandardInput.WriteLine("exit");

            return proc.StandardOutput.ReadToEnd();
        }
    }
}
