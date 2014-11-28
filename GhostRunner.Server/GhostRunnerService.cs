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

        public GhostRunnerService()
        {
            InitializeComponent();

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 5000;
            _timer.Enabled = true;

        }

        protected override void OnStart(string[] args)
        {
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Controller controller = new Controller();
            controller.Start();
        }
    }
}
