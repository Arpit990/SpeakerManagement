using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Repository;
using System.Net;

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

        public IActionResult GetOrganizationList(GridSearch gridSearch)
        {
            var result = _organizationRepository.GetOrganizationList(gridSearch);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Organization model)
        {
            if(ModelState.IsValid)
            {
                _ = await _organizationRepository.Add(model);
                var result = await _organizationRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success(true, Params.Save));
                else
                    return Json(FormResult.Success(false, Params.SaveErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var org = await _organizationRepository.Get(x => x.Id == id);

            if(org != null)
                return Json(FormResult.Success(true, Params.Get, org));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Organization model)
        {
            if (ModelState.IsValid)
            {
                _ = await _organizationRepository.Update(model);
                var result = await _organizationRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success(true, Params.Update));
                else
                    return Json(FormResult.Success(false, Params.UpdateErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _ = await _organizationRepository.Remove(x => x.Id == id);
            var result = await _organizationRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success(true, Params.Delete));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }
        #endregion
    }
}
