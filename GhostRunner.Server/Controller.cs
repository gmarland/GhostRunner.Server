using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server
{
    public class Controller
    {
        TaskService _taskService;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Controller()
        {
            if (Properties.Settings.Default.CommandWindowMinuteTimeout != null) 

            _log.Debug("Setting command timeout to " + TaskService.CommandWindowMinuteTimeout);

            TaskService.PhantomJSLocation = Properties.Settings.Default.PhantomJSLocation;

            _log.Debug("Setting PhantomJS location to " + TaskService.PhantomJSLocation);

            TaskService.CommandWorkingDirectory = Properties.Settings.Default.CommandWorkingDirectory;

            _log.Debug("Setting command working direction location to " + TaskService.CommandWorkingDirectory);

            TaskService.ProcessingLocation = Properties.Settings.Default.ProcessingLocation;

            _log.Debug("Setting processing location to " + TaskService.ProcessingLocation);

            _taskService = new TaskService();
        }

        public void Start()
        {
            ClaimTasks();
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
    }
}
