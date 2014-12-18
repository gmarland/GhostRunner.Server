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
    public class ScriptDataAccess : IScriptDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ScriptDataAccess(IContext context)
        {
            _context = context;
        }

        public Script Get(int scriptId)
        {
            try
            {
                return _context.Scripts.SingleOrDefault(s => s.ID == scriptId);
            }
            catch (Exception ex)
            {
                _log.Error("Get(" + scriptId + "): Error retrieving script", ex);

                return null;
            }
        }
    }
}
