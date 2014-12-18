using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server
{
    public class TaskController
    {
        TaskService _taskService;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskController()
        {
            ConfigureTaskService();

            _taskService = new TaskService();
        }

        public void ClaimTasks()
        {
            _log.Debug("Claiming tasks");

            Task task = null;

            do
            {
                task = _taskService.GetNextTask();

                if (task != null)
                {
                    _log.Debug("Task found");

                    _taskService.ProcessTask(task);
                }
            }
            while (task != null);

            _log.Debug("No more tasks found");
        }

        private void ConfigureTaskService()
        {
            _log.Debug("Setting command timeout to " + TaskService.CommandWindowMinuteTimeout);

            TaskService.CommandWindowMinuteTimeout = Properties.Settings.Default.CommandWindowMinuteTimeout;

            _log.Debug("Setting PhantomJS location to " + TaskService.PhantomJSLocation);

            TaskService.PhantomJSLocation = Properties.Settings.Default.PhantomJSLocation;

            _log.Debug("Setting command working direction location to " + TaskService.CommandWorkingDirectory);

            TaskService.CommandWorkingDirectory = Properties.Settings.Default.CommandWorkingDirectory;

            _log.Debug("Setting processing location to " + TaskService.ProcessingLocation);

            TaskService.ProcessingLocation = Properties.Settings.Default.ProcessingLocation;
        }
    }
}
