using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.DAL.Interface
{
    public interface ITaskDataAccess
    {
        IList<Task> GetProcessingTasks();

        Task GetNextUnprocessed();

        Boolean SetTaskProcessing(long taskId);

        Task Insert(Task task);

        Boolean SetTaskComplete(long taskId, Status status);

        Boolean Delete(long taskId);
    }
}
