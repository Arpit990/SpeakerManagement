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
    public class TaskController : Controller
    {
        #region Private
        private readonly ITaskRepository _taskRepository;
        #endregion

        #region Constructor
        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        #endregion

        #region Public
        public IActionResult Index()
        {
            ViewBag.InputTypes = Common.InputTypeList();
            return View();
        }

        public async Task<IActionResult> GetTaskList(GridSearch gridSearch)
        {
            var orgs = await _taskRepository.GetAll();
            return Json(orgs.SearchGrid(gridSearch));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tasks model)
        {
            if (ModelState.IsValid)
            {
                _ = await _taskRepository.Add(model);
                var result = await _taskRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var org = await _taskRepository.Get(x => x.Id == id);
            if (org != null)
                return Json(org);
            else
                return Json(FormResult.Success("Record Not Found"));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tasks model)
        {
            if (ModelState.IsValid)
            {
                _ = await _taskRepository.Update(model);
                var result = await _taskRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _ = await _taskRepository.Remove(x => x.Id == id);
            var result = await _taskRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success());
            else
                return Json(FormResult.Success("Record Not Found"));
        }
        #endregion
    }
}
