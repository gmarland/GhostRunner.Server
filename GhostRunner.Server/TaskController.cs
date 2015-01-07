using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GhostRunner.Server
{
    public class TaskController
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskController()
        {
            ConfigureTaskService();
        }

        public void ClaimTasks()
        {
            TaskService taskService = new TaskService();

            _log.Debug("Claiming tasks");

            Task task = null;

            do
            {
                task = taskService.GetNextTask();

                if (task != null)
                {
                    _log.Debug("Task found");
                    
                    taskService.ProcessTask(task);
                }
            }
            while (task != null);

            _log.Debug("No more tasks found");
        }

        public void ClearTasks()
        {
            TaskService taskService = new TaskService();

            _log.Debug("Clearing hung tasks");

            taskService.ClearHungTasks();
        }

        private void ConfigureTaskService()
        {
            _log.Debug("Setting Node location to " + TaskService.NodeLocation);

            TaskService.NodeLocation = Properties.Settings.Default.NodeLocation;

            _log.Debug("Setting PhantomJS location to " + TaskService.PhantomJSLocation);

            TaskService.PhantomJSLocation = Properties.Settings.Default.PhantomJSLocation;

            _log.Debug("Setting processing location to " + TaskService.ProcessingLocation);

            TaskService.ProcessingLocation = Properties.Settings.Default.ProcessingLocation;
        }
    }
}
