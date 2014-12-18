using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.DAL.Interface
{
    public interface ITaskDataAccess
    {
        Task GetNextUnprocessed();

        Boolean SetTaskProcessing(int taskId);

        Boolean SetTaskComplete(int taskId, Status status);

        Task Insert(Task initializationTask);
    }
}
