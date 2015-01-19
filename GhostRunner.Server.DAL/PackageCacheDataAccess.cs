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
    public class PackageCacheDataAccess : IPackageCacheDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PackageCacheDataAccess(IContext context)
        {
            _context = context;
        }

        public PackageCache Get(String packageName)
        {
            try
            {
                return _context.PackageCaches.SingleOrDefault(s => s.Name.Trim().ToLower() == packageName.Trim().ToLower());
            }
            catch (Exception ex)
            {
                _log.Error("Get(" + packageName + "): Error retrieving package", ex);

                return null;
            }
        }

        public PackageCache Insert(PackageCache packageCache)
        {
            try
            {
                _context.PackageCaches.Add(packageCache);
                Save();

                return packageCache;
            }
            catch (Exception ex)
            {
                _log.Error("Insert(): Unable to add new package cache", ex);

                return null;
            }
        }

        public Boolean Update(String packageCacheId, String version)
        {
            PackageCache packageCache = null;

            try
            {
                packageCache = _context.PackageCaches.SingleOrDefault(i => i.ExternalId == packageCacheId);
            }
            catch (Exception ex)
            {
                _log.Error("Update(" + packageCacheId + "): Unable to get package cache", ex);

                return false;
            }

            if (packageCache != null)
            {
                packageCache.Version = version;

                try
                {
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("Update(" + packageCacheId + "): Unable to save package cache", ex);

                    return false;
                }
            }
            else
            {
                _log.Error("Update(" + packageCacheId + "): Unable to get find package cache");

                return false;
            }
        }

        public Boolean Delete(String packageCacheId)
        {
            PackageCache packageCache = null;

            try
            {
                packageCache = _context.PackageCaches.SingleOrDefault(i => i.ExternalId == packageCacheId);
            }
            catch (Exception ex)
            {
                _log.Error("Delete(" + packageCacheId + "): Unable to get package cache", ex);

                return false;
            }

            if (packageCache != null)
            {
                _context.PackageCaches.Remove(packageCache);

                try
                {
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("Delete(): An error occured deleting the package cache", ex);

                    return false;
                }
            }
            else
            {
                _log.Info("Unable to find package cache");

                return false;
            }
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
