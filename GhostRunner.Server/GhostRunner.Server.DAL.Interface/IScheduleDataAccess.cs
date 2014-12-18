using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.DAL.Interface
{
    public interface IScheduleDataAccess
    {
        IList<Schedule> GetAllSchedules();

        Boolean UpdateLastScheduled(int scheduleId, DateTime lastScheduled);
    }
}
