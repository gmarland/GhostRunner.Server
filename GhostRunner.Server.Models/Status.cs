using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Models
{
    public enum Status
    {
        Unprocessed,
        Processing,
        Completed,
        Errored,
        Unknown
    }
}
