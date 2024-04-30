using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IEventRepository : IBaseRepository<Events>
    {

    }
    #endregion

    #region class
    public class EventRepository : BaseRepository<Events>, IEventRepository
    {
        #region Constructor
        public EventRepository(DataContext dataContext) : base(dataContext)
        {
                
        }
        #endregion
    }
    #endregion
}
