using Microsoft.AspNetCore.Mvc.Rendering;
using SpeakerManagement.Data;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        GridResult GetOrganizationList(GridSearch gridSearch);
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
        public GridResult GetOrganizationList(GridSearch gridSearch)
        {
            var query = from organization in _context.Organizations
                        select organization;

            var result = PredicateSearch(gridSearch, query);

            return result;
        }

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
