using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Infrastructure;
using SpeakerManagement.Repository;

namespace SpeakerManagement.Controllers
{
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public class OrganizationController : Controller
    {
        #region Private
        private readonly IOrganizationRepository _organizationRepository;
        #endregion

        #region Constructor
        public OrganizationController(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }
        #endregion

        #region Public
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetOrganizationList(GridSearch gridSearch)
        {
            var orgs = await _organizationRepository.GetAll();
            return Json(orgs.SearchGrid(gridSearch));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Organization model)
        {
            if(ModelState.IsValid)
            {
                _ = await _organizationRepository.Add(model);
                var result = await _organizationRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var org = await _organizationRepository.Get(x => x.Id == id);
            if(org != null)
                return Json(org);
            else
                return Json(FormResult.Success("Record Not Found"));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Organization model)
        {
            if (ModelState.IsValid)
            {
                _ = await _organizationRepository.Update(model);
                var result = await _organizationRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _ = await _organizationRepository.Remove(x => x.Id == id);
            var result = await _organizationRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success());
            else
                return Json(FormResult.Success("Record Not Found"));
        }
        #endregion
    }
}
