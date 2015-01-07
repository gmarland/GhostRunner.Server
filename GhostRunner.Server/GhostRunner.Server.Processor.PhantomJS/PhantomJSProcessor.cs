using GhostRunner.Server.Models;
using GhostRunner.Server.Processor.Interface;
using GhostRunner.Server.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GhostRunner.Server.Processor.PhantomJS
{
    public class PhantomJSProcessor : IProcessor
    {
        private int _gruntMinuteTimeout = 10;

        private String _processingLocation;
        private String _phantomJSLocation = String.Empty;

        private TaskScript _taskScript;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PhantomJSProcessor(String processingLocation, String phantomJSLocation, TaskScript taskScript)
        {
            _log.Debug("Setting up phantomjs processing object");

            _log.Debug("Setting processing location to " + processingLocation);

            _processingLocation = processingLocation;

            _log.Debug("Setting phantomjs location to " + phantomJSLocation);

            _phantomJSLocation = phantomJSLocation;

            _taskScript = taskScript;
        }

        public string Process()
        {
            _log.Debug("Starting to process PhantomJS task");

            String phantomJSFileLocation = WritePhantomJSFile();

            _log.Debug("Grunt file created at " + phantomJSFileLocation);

            if (!String.IsNullOrEmpty(phantomJSFileLocation)) return CommandWindowHelper.ProcessCommand(_processingLocation, _gruntMinuteTimeout, "\"" + _phantomJSLocation.TrimEnd(new char[] { '\\' }) + "\\phantomjs.exe\" " + "\"" + phantomJSFileLocation + "\"");
            else return String.Empty;
        }

        private String WritePhantomJSFile()
        {
            if (!Directory.Exists(_processingLocation)) Directory.CreateDirectory(_processingLocation);

            String outputScriptLocation = Path.Combine(_processingLocation.TrimEnd(new char[] { '\\' }), _taskScript.ID + ".js");

            String parameterizedScript = _taskScript.Content;

            foreach (TaskScriptParameter taskScriptParameter in _taskScript.TaskScriptParameters)
            {
                parameterizedScript = Regex.Replace(parameterizedScript, "\\[\\[" + taskScriptParameter.Name + "\\]\\]", taskScriptParameter.Value);
            }

            try
            {
                File.WriteAllText(outputScriptLocation, parameterizedScript);

                return outputScriptLocation;
            }
            catch (Exception ex)
            {
                _log.Error("WritePhantomJSFile(" + _processingLocation + ", " + _taskScript.ID + "): Error writing PhantomJS script", ex);

                return String.Empty;
            }
        }
    }
}
