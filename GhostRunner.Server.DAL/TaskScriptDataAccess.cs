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
    public class TaskScriptDataAccess : ITaskScriptDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskScriptDataAccess(IContext context)
        {
            _context = context;
        }

        public TaskScript Get(long taskScriptId)
        {
            try
            {
                return _context.TaskScripts.SingleOrDefault(ts => ts.ID == taskScriptId);
            }
            catch (Exception ex)
            {
                _log.Error("Get(): Unable to find task script", ex);

                return null;
            }
        }

        public TaskScript Insert(TaskScript taskScript)
        {
            try
            {
                _context.TaskScripts.Add(taskScript);
                Save();

                return taskScript;
            }
            catch (Exception ex)
            {
                _log.Error("Insert(): Unable to add new task script", ex);

                return null;
            }
        }

        public Boolean UpdateTaskScriptLog(long taskScriptId, string log)
        {
            TaskScript taskScript = null;

            try
            {
                taskScript = _context.TaskScripts.SingleOrDefault(ts => ts.ID == taskScriptId);
            }
            catch (Exception ex)
            {
                _log.Info("UpdateTaskScriptLog(" + taskScriptId + "): unable to retrieve task script", ex);
            }

            if (taskScript != null)
            {
                taskScript.Log = log;

                try
                {
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Info("UpdateTaskScriptLog(" + taskScriptId + "): unable to update task script", ex);

                    return false;
                }
            }
            else return false;
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
