using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.DAL
{
    public class TaskDataAccess : ITaskDataAccess
    {
        protected IContext _context;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskDataAccess(IContext context)
        {
            _context = context;
        }

        public IList<Task> GetProcessingTasks()
        {
            try
            {
                return _context.Tasks.Where(it => it.Status == Status.Processing).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("GetProcessingTasks(): An error occured retrieving processing tasks", ex);

                return new List<Task>();
            }
        }

        public Task GetNextUnprocessed()
        {
            List<Task> tasks = new List<Task>();

            try
            {
                tasks = _context.Tasks.Where(it => it.Status == Status.Unprocessed).ToList();
            }
            catch (Exception ex)
            {
                _log.Error("GetNextUnprocessed(): An error occured retrieving tasks", ex);

                return null;
            }

            _log.Debug(tasks.Count + " tasks found");

            if (tasks.Count > 0)
            {
                return tasks.OrderBy(it => it.Created).First();
            }
            else
            {
                return null;
            }
        }

        public Task Insert(Task task)
        {
            try
            {
                _context.Tasks.Add(task);
                Save();

                return task;
            }
            catch (Exception ex)
            {
                _log.Error("Insert(): Unable to add new task", ex);

                return null;
            }
        }

        public Boolean SetTaskProcessing(long taskId)
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

        public Boolean SetTaskComplete(long taskId, Status status)
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
