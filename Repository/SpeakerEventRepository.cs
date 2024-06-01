using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface ISpeakerEventRepository : IBaseRepository<SpeakerEvents>
    {

    }
    #endregion

    #region class
    public class SpeakerEventRepository : BaseRepository<SpeakerEvents>, ISpeakerEventRepository
    {
        #region Constructor
        public SpeakerEventRepository(DataContext dataContext) : base(dataContext)
        {
                
        }
        #endregion
    }
    #endregion
}
