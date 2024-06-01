using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IEventTaskRepository : IBaseRepository<EventTasks>
    {
    }
    #endregion

    #region class
    public class EventTaskRepository : BaseRepository<EventTasks>, IEventTaskRepository
    {
        #region Constructor
        public EventTaskRepository(DataContext dataContext) : base(dataContext)
        {
            
        }
        #endregion
    }
    #endregion
}
