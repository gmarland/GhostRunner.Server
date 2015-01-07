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

namespace GhostRunner.Server.Processor.Grunt
{
    public class GruntProcessor : IProcessor
    {
        private int _gruntMinuteTimeout = 10;

        private String _processingLocation;
        private String _nodeLocation;

        private TaskScript _taskScript;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GruntProcessor(String processingLocation, String nodeLocation, TaskScript taskScript)
        {
            _log.Debug("Setting up grunt processing object");

            _log.Debug("Setting processing location to " + processingLocation);

            _processingLocation = processingLocation;
            _nodeLocation = nodeLocation;

            _taskScript = taskScript;
        }

        public string Process()
        {
            _log.Debug("Starting to process grunt task");

            String gruntFileLocation = WriteGruntFile();

            _log.Debug("Grunt file created at " + gruntFileLocation);

            if (!String.IsNullOrEmpty(gruntFileLocation))
            {
                LoadNpmRequirements();

                return CommandWindowHelper.ProcessCommand(_processingLocation, _nodeLocation, _gruntMinuteTimeout, "grunt");
            }
            else return String.Empty;
        }

        private String WriteGruntFile()
        {
            if (!Directory.Exists(_processingLocation)) Directory.CreateDirectory(_processingLocation);

            String outputScriptLocation = Path.Combine(_processingLocation.TrimEnd(new char[] { '\\' }), "GruntFile.js");

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
                _log.Error("WriteGruntFile(" + _processingLocation + ", " + _taskScript.ID + "): Error writing grunt script", ex);

                return String.Empty;
            }
        }

        private void LoadNpmRequirements()
        {
            List<String> npmCommands = new List<String>();

            npmCommands.Add("npm install grunt-cli");

            Regex requirementsRegex = new Regex(@"grunt.loadNpmTasks\(\"".*?\""\)");

            foreach (Match match in requirementsRegex.Matches(_taskScript.Content.Replace("'", "\"")))
            {
                String matched = match.Value;

                Regex npmReqRegex = new Regex(@"\"".*?\""");

                foreach (Match npmMatch in npmReqRegex.Matches(matched))
                {
                    npmCommands.Add("npm install " + npmMatch.Value.Trim(new char[] { '"' }));
                }
            }

            _log.Info(CommandWindowHelper.ProcessCommand(_processingLocation, _nodeLocation, _gruntMinuteTimeout, npmCommands.ToArray()));

        }
    }
}
