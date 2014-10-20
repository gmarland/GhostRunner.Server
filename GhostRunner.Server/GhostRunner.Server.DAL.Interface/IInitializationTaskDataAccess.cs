using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.DAL.Interface
{
    public interface IInitializationTaskDataAccess
    {
        Task GetNextUnprocessed();

        Boolean UpdateTaskLog(int taskId, String log);

        Boolean SetTaskProcessing(int taskId);

        Boolean SetTaskComplete(int taskId, Status status, String log, String script);
    }
}
