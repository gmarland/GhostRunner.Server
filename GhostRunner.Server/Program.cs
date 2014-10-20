using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Controller controller = new Controller();
            controller.Start();
        }
    }
}
