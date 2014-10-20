using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.DAL
{
    public class InitializationTaskDataAccess : IInitializationTaskDataAccess
    {
        protected GhostRunnerContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InitializationTaskDataAccess(GhostRunnerContext context)
        {
            _context = context;
        }

        public Task GetNextUnprocessed()
        {
            List<Task> initializationTasks = new List<Task>();

            try
            {
                initializationTasks = _context.Tasks.Where(it => it.Status == Status.Unprocessed).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("GetNextUnprocessed(): An error occured retrieving tasks", ex);

                return null;
            }

            if (initializationTasks.Count > 0)
            {
                return initializationTasks.OrderBy(it => it.Created).First();
            }
            else
            {
                return null;
            }
        }

        public Boolean UpdateTaskLog(int taskId, String log)
        {
            Task task = null;

            try
            {
                task = _context.Tasks.SingleOrDefault(it => it.ID == taskId);
            }
            catch (Exception ex)
            {
                _log.Error("UpdateTaskLog(" + taskId + "): An error occured retrieving task", ex);

                return false;
            }

            if (task != null)
            {
                try
                {
                    task.Log += log;
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("UpdateTaskLog(" + taskId + "): An error occured saving task", ex);

                    return false;
                }
            }
            else
            {
                _log.Info("UpdateTaskLog(" + taskId + "): unable to retrieve task");

                return false;
            }
        }

        public Boolean SetTaskProcessing(int taskId)
        {
            Task task = null;
            
            try
            {
                task = _context.Tasks.SingleOrDefault(it => it.ID == taskId);
            }
            catch (Exception ex)
            {
                _log.Error("SetTaskProcessing(" + taskId + "): An error occured retrieving task", ex);

                return false;
            }

            if (task != null)
            {
                try
                {
                    task.Status = Status.Processing;
                    task.Started = DateTime.UtcNow;
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("SetTaskProcessing(" + taskId + "): An error occured saving task", ex);

                    return false;
                }
            }
            else
            {
                _log.Info("SetTaskProcessing(" + taskId + "): unable to retrieve task");

                return false;
            }
        }

        public Boolean SetTaskComplete(int taskId, Status status, String log, String script)
        {
            Task task = null;

            try
            {
                task = _context.Tasks.SingleOrDefault(it => it.ID == taskId);
            }
            catch (Exception ex)
            {
                _log.Error("SetTaskComplete(" + taskId + "): An error occured retrieving task", ex);

                return false;
            }

            if (task != null)
            {
                try
                {
                    task.Status = status;
                    task.Log = log;
                    task.PhantomScript = script;
                    task.Completed = DateTime.UtcNow;
                    Save();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("SetTaskComplete(" + taskId + "): An error occured saving task", ex);

                    return false;
                }
            }
            else
            {
                _log.Info("SetTaskComplete(" + taskId + "): unable to retrieve task");

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
