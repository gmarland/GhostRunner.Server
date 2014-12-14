using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GhostRunner.Server.Utils
{
    public class PhantomJSHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static String WriteJSScript(String processingLocation, TaskScript taskScript)
        {
            String outputScriptLocation = Path.Combine(processingLocation.TrimEnd(new char[] { '\\' }), taskScript.ID + ".js");

            String parameterizedScript = taskScript.Content;

            foreach (TaskScriptParameter taskScriptParameter in taskScript.TaskScriptParameters)
            {
                parameterizedScript = Regex.Replace(parameterizedScript, "\\[" + taskScriptParameter.Name + "\\]", taskScriptParameter.Value);
            }

            try
            {
                File.WriteAllText(outputScriptLocation, parameterizedScript);

                return outputScriptLocation;
            }
            catch (Exception ex)
            {
                _log.Error("WriteJSScript(" + processingLocation + ", " + taskScript.ID + "): Error writing javasript script", ex);

                return String.Empty;
            }
        }
    }
}
