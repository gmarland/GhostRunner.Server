using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GhostRunner.Server
{
    public class Program
    {
        private static Timer _timer;

        public static void Main(string[] args)
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 5000;
            _timer.Enabled = true;

            StartClaimingTasks();
        }

        private static void StartClaimingTasks()
        {
            _timer.Start();
        }

        private static void StopClaimingTasks()
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
