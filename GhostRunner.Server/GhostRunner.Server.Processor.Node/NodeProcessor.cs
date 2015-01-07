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

namespace GhostRunner.Server.Processor.Node
{
    public class NodeProcessor : IProcessor
    {
        private int _nodeMinuteTimeout = 10;

        private String _processingLocation;
        private String _nodeLocation;

        private TaskScript _taskScript;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NodeProcessor(String processingLocation, String nodeLocation, TaskScript taskScript)
        {
            _log.Debug("Setting up node processing object");

            _log.Debug("Setting processing location to " + processingLocation);

            _processingLocation = processingLocation;
            _nodeLocation = nodeLocation;

            _taskScript = taskScript;
        }

        public string Process()
        {
            _log.Debug("Starting to process node task");

            String nodeFileLocation = WriteNodeFile();

            _log.Debug("Node file created at " + nodeFileLocation);

            if (!String.IsNullOrEmpty(nodeFileLocation))
            {
                LoadNpmRequirements();

                return CommandWindowHelper.ProcessCommand(_processingLocation, _nodeLocation, _nodeMinuteTimeout, "node " + nodeFileLocation);
            }
            else return String.Empty;
        }

        private String WriteNodeFile()
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
                _log.Error("WriteNodeFile(" + _processingLocation + ", " + _taskScript.ID + "): Error writing node script", ex);

                return String.Empty;
            }
        }

        private void LoadNpmRequirements()
        {
            Regex requirementsRegex = new Regex(@"require\(\"".*?\""\)");

            foreach (Match match in requirementsRegex.Matches(_taskScript.Content.Replace("'", "\"")))
            {
                String matched = match.Value;

                Regex npmReqRegex = new Regex(@"\"".*?\""");

                foreach (Match npmMatch in npmReqRegex.Matches(matched))
                {
                    _log.Info(CommandWindowHelper.ProcessCommand(_processingLocation, _nodeLocation, 5, "npm install " + npmMatch.Value.Trim(new char[] { '"' })));
                }
            }
        }
    }
}
