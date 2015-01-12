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

namespace GhostRunner.Server.Processor.Batch
{
    public class CommandLineProcessor : IProcessor
    {
        private int _commandLineMinuteTimeout = 10;

        private String _processingLocation;
        private String _nodeLocation;

        private TaskScript _taskScript;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CommandLineProcessor(String processingLocation, String nodeLocation, TaskScript taskScript)
        {
            _log.Debug("Setting up Command Line processing object");

            _log.Debug("Setting processing location to " + processingLocation);

            _processingLocation = processingLocation;
            _nodeLocation = nodeLocation;

            _taskScript = taskScript;
        }

        public string Process()
        {
            if (!Directory.Exists(_processingLocation)) Directory.CreateDirectory(_processingLocation);

            _log.Debug("Starting to process Command Line task");

            String[] commandList = _taskScript.Content.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return CommandWindowHelper.ProcessCommand(_processingLocation, _nodeLocation, _commandLineMinuteTimeout, commandList);
        }
    }
}
