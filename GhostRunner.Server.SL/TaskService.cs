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
        private String _nodeLocation;
        private String _phantomJSLocation;
        private String _processingLocation;
        private String _packageCacheLocation;

        private ITaskDataAccess _taskDataAccess;
        private ITaskScriptDataAccess _taskScriptDataAccess;
        private IPackageCacheDataAccess _packageCacheDataAccess;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskService(IContext context, String processingLocation, String packageCacheLocation, String nodeLocation, String phantomJSLocation)
        {
            _nodeLocation = nodeLocation;
            _phantomJSLocation = phantomJSLocation;
            _processingLocation = processingLocation;
            _packageCacheLocation = packageCacheLocation;

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
            _log.Debug("Starting to process task");

            String processingLocation = _processingLocation.TrimEnd(new char[] { '\\' }) + "\\" + task.ExternalId + "\\";

            _log.Debug("Processing Location: " + processingLocation);

            try
            {
                _log.Debug("Task contains " + task.TaskScripts.Count + " scripts");

                foreach (TaskScript taskScript in task.TaskScripts.OrderBy(ts => ts.Position))
                {
                    processTaskScript(processingLocation, task, taskScript);
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

        private void processTaskScript(String scriptProcessingLocation, Task task, TaskScript taskScript)
        {
            _log.Debug("Determining task type");

            IProcessor processor = null;

            switch (taskScript.Type)
            {
                case ScriptType.Git:
                    processor = new GitProcessor(scriptProcessingLocation, taskScript);
                    break;
                case ScriptType.CommandLine:
                    processor = new CommandLineProcessor(scriptProcessingLocation, _nodeLocation, taskScript);
                    break;
                case ScriptType.Node:
                    processor = new NodeProcessor(scriptProcessingLocation, _nodeLocation, taskScript);
                    break;
                case ScriptType.Grunt:
                    processor = new GruntProcessor(scriptProcessingLocation, _nodeLocation, taskScript);
                    break;
                case ScriptType.PhantomJS:
                    processor = new PhantomJSProcessor(scriptProcessingLocation, _phantomJSLocation, taskScript);
                    break;
            }

            String processResults = String.Empty;

            if (processor != null)
            {
                _log.Debug("Starting to process task");

                if (!Directory.Exists(scriptProcessingLocation)) Directory.CreateDirectory(scriptProcessingLocation);

                _log.Debug("Checking for any required node packages");

                foreach(String requiredPackage in processor.GetRequiredPackages())
                {
                    _log.Debug("Package required: " + requiredPackage);

                    String packageCacheLocation = _packageCacheLocation.TrimEnd(new char[] { '\\' }) + "\\" + task.Project.ExternalId + "\\" + requiredPackage;
                    String targetPackageLocation = scriptProcessingLocation.TrimEnd(new char[] { '\\' }) + "\\node_modules\\" + requiredPackage;

                    _log.Debug("Package cache location: " + packageCacheLocation);
                    _log.Debug("target package location: " + targetPackageLocation);

                    if (!Directory.Exists(packageCacheLocation)) _log.Info(CommandWindowHelper.ProcessCommand(scriptProcessingLocation, _nodeLocation, 5, "npm install " + requiredPackage));
                    else IOHelper.CopyDirectory(packageCacheLocation, targetPackageLocation);

                    if (Directory.Exists(targetPackageLocation))
                    {
                        PackageCache packageCache = _packageCacheDataAccess.Get(requiredPackage);

                        if (packageCache == null)
                        {
                            packageCache = new PackageCache();
                            packageCache.ExternalId = System.Guid.NewGuid().ToString();
                            packageCache.Name = requiredPackage;
                            packageCache.Version = IOHelper.GetPackageVersion(targetPackageLocation);
                            packageCache.Store = true;
                            packageCache.Project = task.Project;

                            packageCache = _packageCacheDataAccess.Insert(packageCache);
                        }

                        if ((packageCache.Store) && (!Directory.Exists(packageCacheLocation))) IOHelper.CopyDirectory(targetPackageLocation, packageCacheLocation);
                    }
                }

                processResults = processor.Process();
            }

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
            _packageCacheDataAccess = new PackageCacheDataAccess(context);
        }

        #endregion
    }
}
