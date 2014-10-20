using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server
{
    public class Controller
    {
        TaskService _taskService;

        public Controller()
        {
            TaskService.PhantomJSLocation = Properties.Settings.Default.PhantomJSLocation;
            TaskService.CommandWorkingDirectory = Properties.Settings.Default.CommandWorkingDirectory;
            TaskService.ProcessingLocation = Properties.Settings.Default.ProcessingLocation;

            _taskService = new TaskService();
        }

        public void Start()
        {
            ClaimTasks();
        }

        public void ClaimTasks()
        {
            Task task = null;

            do
            {
                task = _taskService.GetNextTask();

                if (task != null)
                {
                    _taskService.ProcessTask(task);
                }
            }
            while (task != null);
        }
    }
}
