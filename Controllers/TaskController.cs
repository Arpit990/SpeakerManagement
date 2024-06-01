using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
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
            ViewBag.ValidationType = Common.ValidationList();
            return View();
        }

        public IActionResult GetTaskList(GridSearch gridSearch)
        {
            var result = _taskRepository.GetTaskList(gridSearch);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tasks model)
        {
            if (ModelState.IsValid)
            {
                _ = await _taskRepository.Add(model);
                var result = await _taskRepository.SaveChangesAsync();

                if (result > 0)
                    return Json(FormResult.Success(true, Params.Save));
                else
                    return Json(FormResult.Success(false, Params.SaveErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var task = await _taskRepository.Get(x => x.Id == id);

            if (task != null)
                return Json(FormResult.Success(true, Params.Get, task));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tasks model)
        {
            if (ModelState.IsValid)
            {
                _ = await _taskRepository.Update(model);
                var result = await _taskRepository.SaveChangesAsync();

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
            _ = await _taskRepository.Remove(x => x.Id == id);
            var result = await _taskRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success(true, Params.Delete));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }
        #endregion
    }
}
