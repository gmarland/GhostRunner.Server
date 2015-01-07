using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using log4net;
using System.Threading;

namespace GhostRunner.Server.Utils
{
    public class CommandWindowHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static String ProcessCommand(String commandWorkingDirectory, int minuteTimeout, String commandLineAction)
        {
            return ProcessCommand(commandWorkingDirectory, String.Empty, minuteTimeout, commandLineAction);
        }

        public static String ProcessCommand(String commandWorkingDirectory, String commandArguments, int minuteTimeout, String commandLineAction)
        {
            return ProcessCommand(commandWorkingDirectory, commandArguments, minuteTimeout, new String[] { commandLineAction });
        }

        public static String ProcessCommand(String commandWorkingDirectory, String commandArguments, int minuteTimeout, String[] commandLineActions)
        {
            _log.Debug(commandWorkingDirectory);

            ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe");
            if (!String.IsNullOrEmpty(commandArguments)) psi.Arguments = "/k \"" + commandArguments + "\"";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.WorkingDirectory = commandWorkingDirectory;

            int timeout = minuteTimeout * 60000;
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            {
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    Process proc = System.Diagnostics.Process.Start(psi);

                    foreach (String commandLineAction in commandLineActions)
                    {
                        _log.Debug(commandLineAction);
                        proc.StandardInput.WriteLine(commandLineAction);
                    }

                    proc.StandardInput.WriteLine("exit");
                    proc.StandardInput.Close();

                    proc.OutputDataReceived += (sender, outputLine) =>
                    {
                        if (outputLine.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            if (!outputLine.Data.ToLower().Trim().StartsWith(commandWorkingDirectory.ToLower().Trim() + ">")) output.Append(outputLine.Data + Environment.NewLine);
                        }
                    };

                    proc.ErrorDataReceived += (sender, errorLine) =>
                    {
                        if (errorLine.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            if (!errorLine.Data.ToLower().Trim().StartsWith(commandWorkingDirectory.ToLower().Trim() + ">")) output.Append(errorLine.Data + Environment.NewLine);
                        }
                    };

                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();

                    proc.WaitForExit(timeout);
                    outputWaitHandle.WaitOne(timeout);
                    errorWaitHandle.WaitOne(timeout);

                    proc.Close();

                    return output.ToString();
                }
            }
        }
    }
}
