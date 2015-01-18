using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.DAL
{
    public class ScheduleDataAccess : IScheduleDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ScheduleDataAccess(IContext context)
        {
            _context = context;
        }

        public IList<Schedule> GetAllSchedules()
        {
            try
            {
                return this._context.Schedules.ToList();
            }
            catch (Exception ex)
            {
                _log.Error("GetAllSchedules(): Unable to retrieve schedules", ex);

                return new List<Schedule>();
            }
        }


        public Boolean UpdateLastScheduled(long scheduleId, DateTime lastScheduled)
        {
            Schedule schedule = null;

            try
            {
                schedule = _context.Schedules.SingleOrDefault(s => s.ID == scheduleId);
            }
            catch (Exception ex)
            {
                _log.Error("UpdateLastScheduled(" + scheduleId + "): Error retrieving schedule", ex);
            }

            if (schedule != null)
            {
                schedule.LastScheduled = lastScheduled;

                try
                {
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("UpdateLastScheduled(" + scheduleId + "): Error updating schedule", ex);

                    return false;
                }
            }
            return false;
        }

        private void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
