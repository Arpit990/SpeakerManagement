using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Infrastructure;
using SpeakerManagement.Repository;

namespace SpeakerManagement.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class EventController : Controller
    {
        #region Private
        private readonly IEventRepository _eventRepository;
        #endregion

        #region Constructor
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        #endregion

        #region Public
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetEventList(GridSearch gridSearch)
        {
            var orgs = await _eventRepository.GetAll();
            return Json(orgs.SearchGrid(gridSearch));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Events model)
        {
            if (ModelState.IsValid)
            {
                _ = await _eventRepository.Add(model);
                var result = await _eventRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var org = await _eventRepository.Get(x => x.Id == id);
            if (org != null)
                return Json(org);
            else
                return Json(FormResult.Success("Record Not Found"));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Events model)
        {
            if (ModelState.IsValid)
            {
                _ = await _eventRepository.Update(model);
                var result = await _eventRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _ = await _eventRepository.Remove(x => x.Id == id);
            var result = await _eventRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success());
            else
                return Json(FormResult.Success("Record Not Found"));
        }
        #endregion
    }
}
