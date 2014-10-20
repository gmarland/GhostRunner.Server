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

        public static String WriteJSScript(String processingLocation, Task task)
        {
            String outputScriptLocation = Path.Combine(processingLocation.TrimEnd(new char[] { '\\' }), task.ExternalId + ".js");

            String parameterizedScript = task.Content;

            foreach (TaskParameter taskParameter in task.TaskParameters)
            {
                parameterizedScript = Regex.Replace(parameterizedScript, "\\[" + taskParameter.Name + "\\]", taskParameter.Value);
            }

            try
            {
                File.WriteAllText(outputScriptLocation, parameterizedScript);

                return outputScriptLocation;
            }
            catch (Exception ex)
            {
                _log.Error("WriteJSScript(" + processingLocation + ", " + task.ID + "): Error writing javasript script", ex);

                return String.Empty;
            }
        }

        public static String ReadJSScript(String javascriptLocation)
        {
            try
            {
                return File.ReadAllText(javascriptLocation);
            }
            catch (Exception ex)
            {
                _log.Error("ReadJSScript(" + javascriptLocation + "): Error reading javasript script", ex);

                return String.Empty;
            }
        }

        public static Boolean DeleteJSScript(String javascriptLocation)
        {
            try
            {
                File.Delete(javascriptLocation);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("DeleteJSScript(" + javascriptLocation + "): Error deleting javasript script", ex);

                return false;
            }
        }
    }
}
