﻿using GhostRunner.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostRunner.Server.DAL.Interface
{
    public interface ITaskScriptDataAccess
    {
        TaskScript Get(long taskScriptId);

        TaskScript Insert(TaskScript taskScript);

        Boolean UpdateTaskScriptLog(long taskId, String log);
    }
}
