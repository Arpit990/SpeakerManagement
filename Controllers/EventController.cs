using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Infrastructure;
using SpeakerManagement.Repository;
using SpeakerManagement.ViewModels.Event;
using System.Text.Json;

namespace SpeakerManagement.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class EventController : Controller
    {
        #region Private
        private readonly IEventRepository _eventRepository;
        private readonly IEventTaskRepository _eventTaskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISpeakerEventRepository _speakerEventRepository;
        private readonly ISpeakerTaskRepository _speakerTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public EventController(
            IEventRepository eventRepository,
            IEventTaskRepository eventTaskRepository,
            IHttpContextAccessor httpContextAccessor,
            ISpeakerEventRepository speakerEventRepository,
            ISpeakerTaskRepository speakerTaskRepository,
            ITaskRepository taskRepository,
            IUserRepository userRepository
        )
        {
            _eventRepository = eventRepository;
            _eventTaskRepository = eventTaskRepository;
            _httpContextAccessor = httpContextAccessor;
            _speakerEventRepository = speakerEventRepository;
            _speakerTaskRepository = speakerTaskRepository;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }
        #endregion

        #region Public
        public async Task<IActionResult> Index()
        {
            ViewBag.SpeakerList = await _userRepository.GetUsersForDropDown();
            return View();
        }

        public async Task<IActionResult> GetEventList(GridSearch gridSearch)
        {
            var orgs = await _eventRepository.GetEventList();
            return Json(orgs.SearchGrid(gridSearch));
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.TaskList = await _taskRepository.GetTaskForDropDown();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventTasksViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _eventRepository.AddEventTasks(model);

                if (result.IsSuccess)
                    return Json(FormResult.Success(true, Params.Save));
                else
                    return Json(FormResult.Success(false, Params.SaveErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(int id)
        {
            var events = await _eventRepository.Get(x => x.Id == id);

            if (events != null)
                return Json(FormResult.Success(true, Params.Get, events));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Events model)
        {
            if (ModelState.IsValid)
            {
                _ = await _eventRepository.Update(model);
                var result = await _eventRepository.SaveChangesAsync();

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
            await _eventTaskRepository.RemoveAll(x => x.EventId == id);
            var result = await _eventTaskRepository.SaveChangesAsync();

            _ = await _eventRepository.Remove(x => x.Id == id);
            result = await _eventRepository.SaveChangesAsync();

            if (result > 0)
                return Json(FormResult.Success(true, Params.Delete));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }

        public async Task<IActionResult> AssignEventToSpeaker(AssignEventViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<int> eventIds = JsonSerializer.Deserialize<List<int>>(model.EventIds);
                    string userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");

                    List<SpeakerEvents> speakerEvents = new List<SpeakerEvents>();
                    foreach (int eventId in eventIds)
                    {
                        speakerEvents.Add(new SpeakerEvents
                        {
                            EventId = eventId,
                            SpeakerId = model.SpeakerId,
                            AssignBy = userId,
                            AssignDate = DateTime.Now,
                            IsCompleted = false,
                        });
                    }
                    await _speakerEventRepository.AddRange(speakerEvents);
                    _ = await _speakerEventRepository.SaveChangesAsync();

                    var tasks = await _eventTaskRepository.GetList(x => eventIds.Contains(x.EventId));

                    if (tasks != null && tasks.Count() > 0)
                    {
                        List<SpeakerTasks> speakerTasks = new List<SpeakerTasks>();
                        foreach (var task in tasks)
                        {
                            speakerTasks.Add(new SpeakerTasks
                            {
                                EventTaskId = task.Id,
                                SpeakerId = model.SpeakerId,
                                IsCompleted = false,
                                Status = ""
                            });
                        }
                        await _speakerTaskRepository.AddRange(speakerTasks);
                        _ = await _speakerTaskRepository.SaveChangesAsync();

                        return Json(FormResult.Success(true, "Speaker Assign Successfully"));
                    }
                }
                return Json(FormResult.Error(ModelState));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
