using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.Models
{
    public interface IScheduleItem
    {
        ItemType Type { get; }

        String ExternalId { get; }

        Project Project { get; }

        String Name { get; }

        ScheduleType ScheduleType { get; }

        IList<ScheduleDetail> ScheduleDetails { get; }
    }
}
