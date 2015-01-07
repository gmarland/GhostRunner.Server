using GhostRunner.Server.DAL;
using GhostRunner.Server.DAL.Interface;
using GhostRunner.Server.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GhostRunner.Server.SL
{
    public class ScheduleService
    {
        private ITaskDataAccess _taskDataAccess;
        private ISequenceDataAccess _sequenceDataAccess;
        private IScriptDataAccess _scriptDataAccess;
        private IScheduleDataAccess _scheduleDataAccess;
        private ITaskScriptDataAccess _taskScriptDataAccess;
        private ITaskScriptParameterDataAccess _taskScriptParameterDataAccess;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ScheduleService()
        {
            InitializeDataAccess(new GhostRunnerContext("DatabaseConnectionString"));
        }

        public void ScheduleTasks()
        {
            _log.Debug("Retrieving schedules");

            IList<Schedule> allSchedules = _scheduleDataAccess.GetAllSchedules();

            _log.Debug(allSchedules.Count + " schedules found");

            foreach (Schedule schedule in allSchedules)
            {
                DateTime? scheduleDateTime = null;

                try
                {
                    scheduleDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Int32.Parse(schedule.ScheduleDetails.SingleOrDefault(sd => sd.Name.Trim().ToLower() == "hour").Value), Int32.Parse(schedule.ScheduleDetails.SingleOrDefault(sd => sd.Name.Trim().ToLower() == "minute").Value), 0);
                }
                catch (Exception ex)
                {
                    _log.Error("GetOutstandingSchedules(): Unable to generate date time", ex);

                    continue;
                }

                switch (schedule.ScheduleType)
                {
                    case ScheduleType.Daily:
                        _log.Debug("Daily schedule found");

                        CreateTask(schedule, scheduleDateTime.Value);
                        break;
                    case ScheduleType.Weekly:
                        _log.Debug("Weekly schedule found");

                        foreach (ScheduleDetail scheduleDetail in schedule.ScheduleDetails.Where(sd => sd.Name.Trim().ToLower() == "day" && sd.Value.Trim().ToLower() == DateTime.Now.DayOfWeek.ToString().ToLower()))
                        {
                            CreateTask(schedule, scheduleDateTime.Value);
                        }
                        break;
                    case ScheduleType.Monthly:
                        _log.Debug("Monthly schedule found");

                        foreach (ScheduleDetail scheduleDetail in schedule.ScheduleDetails.Where(sd => sd.Name.Trim().ToLower() == "date" && sd.Value.Trim().ToLower() == DateTime.Now.Date.Day.ToString()))
                        {
                            CreateTask(schedule, scheduleDateTime.Value);
                        }
                        break;
                }
            }
        }

        public Boolean UpdateLastScheduled(int scheduleId, DateTime lastScheduled)
        {
            return _scheduleDataAccess.UpdateLastScheduled(scheduleId, lastScheduled);
        }

        public Task InsertScriptTask(int scriptId, IList<ScheduleParameter> scheduleParameters)
        {
            Script script = _scriptDataAccess.Get(scriptId);

            if (script != null)
            {
                Task task = new Task();
                task.ExternalId = System.Guid.NewGuid().ToString();
                task.Project = script.Project;
                task.ParentId = script.ID;
                task.ParentType = ItemType.Script;
                task.Name = script.Name;
                task.Status = Status.Unprocessed;
                task.Created = DateTime.UtcNow;

                task = _taskDataAccess.Insert(task);

                TaskScript taskScript = new TaskScript();
                taskScript.Task = task;
                taskScript.Content = script.Content;

                taskScript = _taskScriptDataAccess.Insert(taskScript);

                if (scheduleParameters != null)
                {
                    foreach (ScheduleParameter scheduleParameter in scheduleParameters)
                    {
                        InsertTaskScriptParameter(taskScript.ID, scheduleParameter.Name, scheduleParameter.Value);
                    }
                }

                return task;
            }
            else
            {
                _log.Info("InsertScriptTask(" + scriptId + "): Unable to find script");

                return null;
            }
        }

        public Task InsertSequenceTask(int sequenceId)
        {
            Sequence sequence = _sequenceDataAccess.Get(sequenceId);

            if (sequence != null)
            {
                Task task = new Task();
                task.ExternalId = System.Guid.NewGuid().ToString();
                task.Project = sequence.Project;
                task.ParentId = sequence.ID;
                task.ParentType = ItemType.Sequence;
                task.Name = sequence.Name;
                task.Status = Status.Unprocessed;
                task.Created = DateTime.UtcNow;

                task = _taskDataAccess.Insert(task);

                foreach (SequenceScript sequenceScript in sequence.SequenceScripts)
                {
                    TaskScript taskScript = new TaskScript();
                    taskScript.Task = task;
                    taskScript.Content = sequenceScript.Content;

                    _taskScriptDataAccess.Insert(taskScript);
                }

                return task;
            }
            else
            {
                _log.Info("InsertSequenceTask(" + sequenceId + "): Unable to find script");

                return null;
            }
        }

        public TaskScriptParameter InsertTaskScriptParameter(int taskScriptId, String name, String value)
        {
            TaskScript taskScript = _taskScriptDataAccess.Get(taskScriptId);

            if (taskScript != null)
            {
                TaskScriptParameter taskScriptParameter = new TaskScriptParameter();
                taskScriptParameter.TaskScript = taskScript;
                taskScriptParameter.Name = name;
                taskScriptParameter.Value = value;

                return _taskScriptParameterDataAccess.Insert(taskScriptParameter);
            }
            else
            {
                _log.Info("InsertTaskScriptParameter(" + taskScriptId + "): Unable to find script task");

                return null;
            }
        }

        #region Private Methods

        private void CreateTask(Schedule schedule, DateTime scheduleDateTime)
        {
            _log.Debug("Schedule: " + schedule.ID + " - Schedule time : " + scheduleDateTime);
            
            if (_log.IsDebugEnabled)
            {
                if (schedule.LastScheduled.HasValue) _log.Debug("Schedule: " + schedule.ID + " - Schedule last scheduled: " + schedule.LastScheduled.Value);
                else _log.Debug("Schedule: " + schedule.ID + " - Not previously scheduled");
            }

            if (((!schedule.LastScheduled.HasValue) && ((DateTime.Now.Hour >= scheduleDateTime.Hour) && (DateTime.Now.Minute >= scheduleDateTime.Minute))) ||
                ((schedule.LastScheduled.HasValue) && (schedule.LastScheduled.Value < scheduleDateTime)))
            {
                _log.Debug("Task should be created");

                if (schedule.ScheduleItemType == ItemType.Script) InsertScriptTask(schedule.ScheduleItemId, schedule.ScheduleParameters.ToList());
                else if (schedule.ScheduleItemType == ItemType.Sequence) InsertSequenceTask(schedule.ScheduleItemId);

                UpdateLastScheduled(schedule.ID, scheduleDateTime);
            }
        }

        private void InitializeDataAccess(IContext context)
        {
            _taskDataAccess = new TaskDataAccess(context);
            _sequenceDataAccess = new SequenceDataAccess(context);
            _scriptDataAccess = new ScriptDataAccess(context);
            _scheduleDataAccess = new ScheduleDataAccess(context);
            _taskScriptDataAccess = new TaskScriptDataAccess(context);
            _taskScriptParameterDataAccess = new TaskScriptParameterDataAccess(context);
        }

        #endregion
    }
}
