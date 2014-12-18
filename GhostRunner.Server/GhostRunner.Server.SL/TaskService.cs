using GhostRunner.Server.DAL;
using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using GhostRunner.Server.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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

        private ITaskDataAccess _taskDataAccess;
        private ITaskScriptDataAccess _taskScriptDataAccess;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskService()
        {
            InitializeDataAccess(new GhostRunnerContext("DatabaseConnectionString"));
        }

        public TaskService(IContext context)
        {
            InitializeDataAccess(context);
        }

        public Task GetNextTask()
        {
            _log.Debug("Getting the next unprocessed task");

            Task task = _taskDataAccess.GetNextUnprocessed();

            if (task != null)
            {
                Boolean started = _taskDataAccess.SetTaskProcessing(task.ID);

                if (started) return task;
                else return null;
            }
            else return null;
        }

        public void ProcessTask(Task task)
        {
            foreach (TaskScript taskScript in task.TaskScripts)
            {
                processTaskScript(task.ExternalId, taskScript);
            }

            _taskDataAccess.SetTaskComplete(task.ID, Status.Completed);

            _log.Debug("Deleting processing location");

            int tryCount = 5000;

            for (int i = 0; i < tryCount; i++)
            {
                try
                { 
                    Directory.Delete(ProcessingLocation.TrimEnd(new char[] { '\\' }) + "\\" + task.ExternalId, true);

                    break;
                }
                catch (Exception ex)
                {
                    _log.Debug("Unable to delete processing location", ex);
                }
            }
        }

        #region Private Methods

        private void processTaskScript(String taskId, TaskScript taskScript)
        {
            _log.Debug("Writing out JavaScript script");

            String scriptLocation = PhantomJSHelper.WriteJSScript(ProcessingLocation.TrimEnd(new char[] { '\\' }) + "\\" + taskId, taskScript);

            _log.Debug("JavaScript script wrote out to " + scriptLocation);

            _log.Debug("Processing JavaScript command");

            String processResults = CommandWindowHelper.ProcessCommand(CommandWorkingDirectory, CommandWindowMinuteTimeout, "\"" + PhantomJSLocation.TrimEnd(new char[] { '\\' }) + "\\phantomjs.exe\" " + "\"" + scriptLocation + "\"");

            _log.Debug("Setting up processing at location " + processResults);

            _taskScriptDataAccess.UpdateTaskScriptLog(taskScript.ID, processResults);
        }

        private void InitializeDataAccess(IContext context)
        {
            _taskDataAccess = new TaskDataAccess(context);
            _taskScriptDataAccess = new TaskScriptDataAccess(context);
        }

        #endregion
    }
}
