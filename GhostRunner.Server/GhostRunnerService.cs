using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static Timer _timer;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GhostRunnerService()
        {
            InitializeComponent();

            _log.Debug("Service initialized");

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 5000;
            _timer.Enabled = true;

            _log.Debug("Timer set up");
        }

        protected override void OnStart(string[] args)
        {
            _timer.Start();

            _log.Debug("Timer started");
        }

        protected override void OnStop()
        {
            _timer.Stop();

            _log.Debug("Timer stopped");
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _log.Debug("Setting up controller");

            Controller controller = new Controller();

            _log.Debug("Controller started");

            controller.Start();
        }
    }
}
