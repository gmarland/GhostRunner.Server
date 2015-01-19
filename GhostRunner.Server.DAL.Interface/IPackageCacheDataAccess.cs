using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.DAL.Interface
{
    public interface IPackageCacheDataAccess
    {
        PackageCache Get(String packageName);

        PackageCache Insert(PackageCache packageCache);

        Boolean Update(String packageCacheId, String version);

        Boolean Delete(String packageCacheId);
    }
}
