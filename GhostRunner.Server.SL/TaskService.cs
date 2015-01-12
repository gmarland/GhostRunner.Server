using GhostRunner.Server.DAL;
using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using GhostRunner.Server.Processor.Batch;
using GhostRunner.Server.Processor.Git;
using GhostRunner.Server.Processor.Grunt;
using GhostRunner.Server.Processor.Interface;
using GhostRunner.Server.Processor.Node;
using GhostRunner.Server.Processor.PhantomJS;
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
        public static String NodeLocation = String.Empty;
        public static String PhantomJSLocation = String.Empty;

        public static String ProcessingLocation = String.Empty;

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
            String processingLocation = ProcessingLocation.TrimEnd(new char[] { '\\' }) + "\\" + task.ExternalId + "\\";

            try
            {
                foreach (TaskScript taskScript in task.TaskScripts.OrderBy(ts => ts.Position))
                {
                    processTaskScript(processingLocation, task.ExternalId, taskScript);
                }
            }
            catch (Exception ex)
            {
                _log.Error("ProcessTask(): An error occured processing the task", ex);
            }

            _log.Debug("Setting task complete");

            _taskDataAccess.SetTaskComplete(task.ID, Status.Completed);

            _log.Debug("Deleting processing location");

            int tryCount = 5000;
            Exception deleteError = null;

            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    DeleteDirectory(processingLocation);

                    deleteError = null;
                    break;
                }
                catch (Exception ex)
                {
                    deleteError = ex;
                }
            }

            if (deleteError != null) _log.Error("Unable to delete processing location", deleteError);
        }

        public IList<Task> ClearHungTasks()
        {
            IList<Task> tasks = _taskDataAccess.GetProcessingTasks();

            _log.Debug(tasks.Count + " processing tasks found");

            IList<Task> returnTasks = new List<Task>();

            foreach(Task task in tasks)
            {
                _log.Debug("Task ID " + task.ID + " processing for " + (DateTime.UtcNow - task.Started.Value).TotalHours + " hours");

                if ((task.Started.HasValue) && ((DateTime.UtcNow - task.Started.Value).TotalHours > 4)) _taskDataAccess.SetTaskComplete(task.ID, Status.Errored);
            }

            return returnTasks;
        }

        #region Private Methods

        private void processTaskScript(String processingLocation, String taskId, TaskScript taskScript)
        {

            IProcessor processor = null;

            switch (taskScript.Type)
            {
                case ScriptType.Git:
                    processor = new GitProcessor(processingLocation, taskScript);
                    break;
                case ScriptType.CommandLine:
                    processor = new CommandLineProcessor(processingLocation, NodeLocation, taskScript);
                    break;
                case ScriptType.Node:
                    processor = new NodeProcessor(processingLocation, NodeLocation, taskScript);
                    break;
                case ScriptType.Grunt:
                    processor = new GruntProcessor(processingLocation, NodeLocation, taskScript);
                    break;
                case ScriptType.PhantomJS:
                    processor = new PhantomJSProcessor(processingLocation, PhantomJSLocation, taskScript);
                    break;
            }

            String processResults = String.Empty;

            if (processor != null) processResults = processor.Process();

            _taskScriptDataAccess.UpdateTaskScriptLog(taskScript.ID, processResults);
        }

        private void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private void InitializeDataAccess(IContext context)
        {
            _taskDataAccess = new TaskDataAccess(context);
            _taskScriptDataAccess = new TaskScriptDataAccess(context);
        }

        #endregion
    }
}
