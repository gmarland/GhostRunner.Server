using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using log4net;

namespace GhostRunner.Server.Utils
{
    public class CommandWindowHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static String ProcessCommand(String commandWorkingDirectory, int minuteTimeout, String commandLineAction)
        {
            _log.Debug(commandWorkingDirectory);
            _log.Debug(commandLineAction);

            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = commandWorkingDirectory;

            StringBuilder output = new StringBuilder();

            Process proc = System.Diagnostics.Process.Start(psi);
            proc.StandardInput.WriteLine(commandLineAction);
            proc.StandardInput.WriteLine("exit");

            proc.ErrorDataReceived += (sender, errorLine) => {
                if (errorLine.Data != null)
                {
                    if (!errorLine.Data.ToLower().Trim().StartsWith(commandWorkingDirectory.ToLower().Trim() + ">")) output.Append(errorLine.Data + Environment.NewLine);
                }
            };
            proc.OutputDataReceived += (sender, outputLine) => {
                if (outputLine.Data != null)
                {
                    if (!outputLine.Data.ToLower().Trim().StartsWith(commandWorkingDirectory.ToLower().Trim() + ">")) output.Append(outputLine.Data + Environment.NewLine);
                }
            };
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            proc.WaitForExit(minuteTimeout * 60000);

            return output.ToString();
        }
    }
}
