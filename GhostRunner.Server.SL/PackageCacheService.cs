using GhostRunner.Server.DAL;
using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.SL
{
    public class PackageCacheService
    {
        private String _packageCacheLocation = String.Empty;
        private String _processingLocation = String.Empty;

        private IPackageCacheDataAccess _packageCacheDataAccess;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PackageCacheService(String packageCacheLocation, String processingLocation, IContext context)
        {
            InitializeDataAccess(context);
        }

        private void InitializeDataAccess(IContext context)
        {
            _packageCacheDataAccess = new PackageCacheDataAccess(context);
        }
    }
}
