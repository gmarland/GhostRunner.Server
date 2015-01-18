using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace GhostRunner.Server
{
    public class ScheduleController
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ScheduleTasks()
        {
            ScheduleService scheduleService = new ScheduleService(new GhostRunnerContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString));

            _log.Debug("Scheduling tasks");

            scheduleService.ScheduleTasks();
        }
    }
}
