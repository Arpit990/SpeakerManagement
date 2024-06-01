using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.ViewModels.Speaker;
using System.Runtime.CompilerServices;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface ISpeakerTaskRepository : IBaseRepository<SpeakerTasks>
    {
        Task<List<AssignTaskViewModel>> GetSpeakerTaskList();
        List<object> GetSpeakerTasksStatus();
    }
    #endregion

    #region class
    public class SpeakerTaskRepository : BaseRepository<SpeakerTasks>, ISpeakerTaskRepository
    {
        #region Constructor
        public SpeakerTaskRepository(
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor
        ) : base(dataContext, httpContextAccessor)
        {

        }
        #endregion

        #region Public
        public async Task<List<AssignTaskViewModel>> GetSpeakerTaskList()
        {
            string userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");

            var query = from speakerTask in _context.SpeakerTasks
                        join eventTask in _context.EventTasks on speakerTask.EventTaskId equals eventTask.Id
                        join task in _context.Tasks on eventTask.TaskId equals task.Id
                        where speakerTask.SpeakerId == userId
                        select new AssignTaskViewModel
                        {
                            SpeakerTaskId = speakerTask.Id,
                            TaskName = task.TaskName,
                            InputType = task.InputType,
                            Instruction = task.Instructions,
                            Data = speakerTask.Data,
                            Deadline = eventTask.Deadline,
                            IsCompleted = speakerTask.IsCompleted
                        };

            var result = await query.ToListAsync();
            return result;
        }

        public List<object> GetSpeakerTasksStatus()
        {
            try
            {

                string query = @"
            DECLARE @cols AS NVARCHAR(MAX), @query AS NVARCHAR(MAX);

            -- Generate dynamic column list
            SELECT @cols = STUFF((SELECT DISTINCT ',' +
                                        QUOTENAME(TaskName)
                                FROM Task
                                FOR XML PATH(''), TYPE
                                ).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

            -- Add Event columns to the dynamic column list
            SELECT @cols = @cols + ', ' + STUFF((SELECT DISTINCT ',' +
                                        QUOTENAME(EventName)
                                FROM [Event]
                                FOR XML PATH(''), TYPE
                                ).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

            -- Build dynamic SQL query
            SET @query = 'SELECT UserName, ' + @cols + '
                            FROM 
                            (
                                SELECT u.UserName, t.TaskName, e.EventName,
                                CASE WHEN st.IsCompleted = 1 THEN ''1'' ELSE ''0'' END AS TaskStatus
                                FROM SpeakerTasks st
                                INNER JOIN [User] u ON u.Id = st.SpeakerId
                                INNER JOIN EventTask et ON st.EventTaskId = et.Id
                                INNER JOIN Task t ON et.TaskId = t.Id
                                INNER JOIN [Event] e ON et.EventId = e.Id
                            ) AS SourceTable
                            PIVOT
                            (
                                MAX(TaskStatus) FOR TaskName IN (' + @cols + ')
                            ) AS PivotTable;';

            -- Execute dynamic SQL query
            EXECUTE(@query);
        ";

                FormattableString data = FormattableStringFactory.Create(query);
                var result = _context.Database.SqlQuery<object>(data).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }
        #endregion
    }
    #endregion
}
