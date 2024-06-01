using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.Data;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using System.Xml;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface ITaskRepository : IBaseRepository<Tasks>
    {
        GridResult GetTaskList(GridSearch gridSearch);
        Task<List<SelectListItem>> GetTaskForDropDown();
    }
    #endregion

    #region class
    public class TaskRepository : BaseRepository<Tasks>, ITaskRepository
    {
        #region Constructor
        public TaskRepository(DataContext dataContext) : base(dataContext)
        {
            
        }
        #endregion

        #region public
        public GridResult GetTaskList(GridSearch gridSearch)
        {
            var query = from task in _context.Tasks
                        select task;

            var result = PredicateSearch(gridSearch, query);

            return result;
        }

        public async Task<List<SelectListItem>> GetTaskForDropDown()
        {
            return await (from x_Task in _context.Tasks
                          select new SelectListItem
                          {
                              Value = x_Task.Id.ToString(),
                              Text = x_Task.TaskName
                          }).OrderBy(x => x.Text).ToListAsync();
        }
        #endregion
    }
    #endregion
}
