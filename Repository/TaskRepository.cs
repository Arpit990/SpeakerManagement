using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface ITaskRepository : IBaseRepository<Tasks>
    {

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
    }
    #endregion
}
