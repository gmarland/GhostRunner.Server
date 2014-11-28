using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GhostRunner.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            
            ServicesToRun = new ServiceBase[] 
			{ 
				new GhostRunnerService() 
			};

            ServiceBase.Run(ServicesToRun);
        }
    }
}
