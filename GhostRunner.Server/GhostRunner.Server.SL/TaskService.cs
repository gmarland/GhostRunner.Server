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
            String scriptLocation = PhantomJSHelper.WriteJSScript(ProcessingLocation, task);

            String processResults = CommandWindowHelper.ProcessCommand(CommandWorkingDirectory, "\"" + PhantomJSLocation.TrimEnd(new char[] { '\\' }) + "\\phantomjs.exe\" " + "\"" + scriptLocation + "\"");

            String phantomJSScript = PhantomJSHelper.ReadJSScript(scriptLocation);

            _initializationTaskDataAccess.SetTaskComplete(task.ID, Status.Completed, processResults, phantomJSScript);

            PhantomJSHelper.DeleteJSScript(scriptLocation);    
        }
    }
}
