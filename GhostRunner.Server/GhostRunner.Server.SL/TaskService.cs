using GhostRunner.Server.DAL;
using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using GhostRunner.Server.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.SL
{
    public class TaskService
    {
        public static String PhantomJSLocation = String.Empty;
        public static int CommandWindowMinuteTimeout = 1;
        public static String ProcessingLocation = String.Empty;
        public static String CommandWorkingDirectory = String.Empty;

        private GhostRunnerContext _context;

        private IInitializationTaskDataAccess _initializationTaskDataAccess;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskService()
        {
            _context = new GhostRunnerContext("DatabaseConnectionString");

            _initializationTaskDataAccess = new InitializationTaskDataAccess(_context);
        }

        public Task GetNextTask()
        {
            Task task = _initializationTaskDataAccess.GetNextUnprocessed();

            if (task != null)
            {
                Boolean started = _initializationTaskDataAccess.SetTaskProcessing(task.ID);

                if (started) return task;
                else return null;
            }
            else return null;
        }

        public void ProcessTask(Task task)
        {
            _log.Debug("Writing out JavaScript script");

            String scriptLocation = PhantomJSHelper.WriteJSScript(ProcessingLocation, task);

            _log.Debug("JavaScript script wrote out to " + scriptLocation);

            _log.Debug("Reading in built JavaScript file");

            String phantomJSScript = PhantomJSHelper.ReadJSScript(scriptLocation);

            _log.Debug("Processing JavaScript command");

            String processResults = CommandWindowHelper.ProcessCommand(CommandWorkingDirectory, CommandWindowMinuteTimeout, "\"" + PhantomJSLocation.TrimEnd(new char[] { '\\' }) + "\\phantomjs.exe\" " + "\"" + scriptLocation + "\"");

            _log.Debug("Setting up processing at location " + processResults);

            _initializationTaskDataAccess.SetTaskComplete(task.ID, Status.Completed, processResults, phantomJSScript);

            _log.Debug("Deleting JavaScript file");

            PhantomJSHelper.DeleteJSScript(scriptLocation);    
        }
    }
}
