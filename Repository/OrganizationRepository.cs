using Microsoft.AspNetCore.Mvc.Rendering;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        Task<List<SelectListItem>> GetOrganizationDropDown();
    }
    #endregion

    #region class
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        #region Constructor
        public OrganizationRepository(DataContext dataContext) : base(dataContext)
        {
            
        }
        #endregion

        #region Public
        public async Task<List<SelectListItem>> GetOrganizationDropDown() =>
            (await GetList()).Select(u =>
                new SelectListItem
                {
                    Text = u.OrganizationName,
                    Value = Convert.ToString(u.Id)
                })
            .OrderBy(x => x.Text).ToList();
        #endregion
    }
    #endregion
}
