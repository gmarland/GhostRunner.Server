using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GhostRunner.Server
{
    partial class GhostRunnerService : ServiceBase
    {
        private static Boolean _processingTasks = false;

        private static Timer _taskTimer;
        private static TaskController _taskController = null;

        private static Timer _scheduleTimer;
        private static ScheduleController _scheduleController = null;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GhostRunnerService()
        {
            InitializeComponent();

            _log.Debug("Service initialized");

            _taskTimer = new Timer();
            _taskTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent_Task);
            _taskTimer.Interval = 5000;
            _taskTimer.Enabled = true;

            _log.Debug("Task timer set up");

            _scheduleTimer = new Timer();
            _scheduleTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent_Schedule);
            _scheduleTimer.Interval = 3000;
            _scheduleTimer.Enabled = true;

            _log.Debug("scheule timer set up");
        }

        protected override void OnStart(string[] args)
        {
            GhostRunnerContext.Initialize(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);

            _taskTimer.Start();

            _log.Debug("Task timer started");

            _scheduleTimer.Start();

            _log.Debug("Schedule timer started");
        }

        protected override void OnStop()
        {
            _taskTimer.Stop();

            _log.Debug("Timer stopped");

            _scheduleTimer.Stop();

            _log.Debug("Schedule timer stopped");
        }

        private static void OnTimedEvent_Task(object source, ElapsedEventArgs e)
        {
            if (!_processingTasks)
            {
                _processingTasks = true;

                if (_taskController == null)
                {
                    _log.Debug("Setting up task controller");

                    _taskController = new TaskController();

                    _log.Debug("Task controller started");

                    _taskController.ClaimTasks();

                    _taskController.ClearTasks();

                    _log.Debug("clearing task controller");

                    _taskController = null;
                }

                _processingTasks = false;
            }
        }

        private static void OnTimedEvent_Schedule(object source, ElapsedEventArgs e)
        {
            if (_scheduleController == null)
            {
                _log.Debug("Setting up schedule controller");

                _scheduleController = new ScheduleController();

                _log.Debug("Schedule controller started");

                _scheduleController.ScheduleTasks();

                _log.Debug("clearing schedule controller");

                _scheduleController = null;
            }
        }
    }
}
