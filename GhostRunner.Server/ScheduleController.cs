using GhostRunner.Server.Models;
using GhostRunner.Server.SL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server
{
    public class ScheduleController
    {
        ScheduleService _scheduleService;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ScheduleController()
        {
            _scheduleService = new ScheduleService();
        }

        public void ScheduleTasks()
        {
            _log.Debug("Scheduling tasks");

            _scheduleService.ScheduleTasks();
        }
    }
}
