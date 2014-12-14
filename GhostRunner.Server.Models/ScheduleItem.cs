using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Models
{
    public class ScheduleItem
    {
        private Schedule _schedule;

        public ScheduleItem(Schedule schedule)
        {
            _schedule = schedule;
        }

        public String ExternalId
        {
            get
            {
                return _schedule.ExternalId;
            }
        }

        public Project Project
        {
            get
            {
                return _schedule.Project;
            }
        }

        public ScheduleType ScheduleType
        {
            get
            {
                return _schedule.ScheduleType;
            }
        }

        public IList<ScheduleDetail> ScheduleDetails
        { 
            get
            {
                return _schedule.ScheduleDetails.ToList();
            }
        }
    }
}
