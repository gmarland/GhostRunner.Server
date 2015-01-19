using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.Processor.Interface
{
    public interface IProcessor
    {
        String Process();

        IList<String> GetRequiredPackages();
    }
}
