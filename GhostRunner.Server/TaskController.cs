using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace GhostRunner.Server
{
    public class TaskController
    {
        private TaskService _taskService;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskController()
        {
            _taskService = new TaskService(new GhostRunnerContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString), Properties.Settings.Default.ProcessingLocation, Properties.Settings.Default.PackageCacheLocation, Properties.Settings.Default.NodeLocation, Properties.Settings.Default.PhantomJSLocation);
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

        public void ClearTasks()
        {
            _log.Debug("Clearing hung tasks");

            _taskService.ClearHungTasks();
        }
    }
}
